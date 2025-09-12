using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

public class CropManager : MonoBehaviour
{
    public static CropManager Instance;

    // Пользовательские коллекции по требованию:
    public Dictionary<Vector3, CropState> tileToState = new Dictionary<Vector3, CropState>();
    public Dictionary<Vector3, CropModel> tileToModel = new Dictionary<Vector3, CropModel>();

    // Все доступные модели (подвесить в инспекторе или загружать динамически)
    public CropModel[] availableCropModels;

    // Тильный набор вскопанных/подготовленных тайлов
    private HashSet<Vector3> tilledTiles = new HashSet<Vector3>();

    // Настройка для сохранения
    [SerializeField] private string saveFileName = "cropsave.json";

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    #region Public API

    // Вспомог: округляем позицию до целого тайла (важно — единый контракт: Y=0)
    public static Vector3 TilePosFromWorld(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x);
        int z = Mathf.RoundToInt(worldPos.z);
        return new Vector3(x, 0f, z);
    }

    public bool IsTilled(Vector3 tile)
    {
        return tilledTiles.Contains(tile);
    }

    public void TillTile(Vector3 tile)
    {
        tilledTiles.Add(tile);
        Debug.Log($"Tile {tile} tilled");
    }

    public void UntillTile(Vector3 tile)
    {
        tilledTiles.Remove(tile);
    }

    // Посадка: model может быть выбран из inventory, seed и т.д.
    public bool PlantSeed(Vector3 tile, CropModel model)
    {
        if (!IsTilled(tile)) { Debug.Log($"[CropManager] Can't plant {model.cropId} at {tile}: tile not tilled"); return false; }
        if (tileToState.ContainsKey(tile)) { Debug.Log($"[CropManager] Can't plant {model.cropId} at {tile}: already occupied"); return false; }

        CropState state = new CropState()
        {
            defCropId = model.cropId,
            tilePos = tile,
            currentStage = 0,
            daysInStage = 0,
            wateredToday = false,
            wateredStreak = 0,
            daysSincePlanted = 0,
            isReadyToHarvest = false,
            isWithered = false,
            quality = 0
        };

        tileToState[tile] = state;
        tileToModel[tile] = model;

        SpawnVisualFor(tile, model, state);

        OnCropPlanted?.Invoke(tile, model, state);
        Debug.Log($"[CropManager] Planted {model.cropId} at {tile}");
        return true;
    }

    public void WaterTile(Vector3 tile)
    {
        if (tileToState.TryGetValue(tile, out CropState state))
        {
            state.wateredToday = true;
            OnCropWatered?.Invoke(tile, tileToModel[tile], state);
            Debug.Log($"[CropManager] Watered {state.defCropId} at {tile}");
        }
        else
        {
            Debug.Log($"[CropManager] Tried watering {tile}, but no crop here");
        }
    }

    // Сбор урожая (возвращает true если успешен)
    public bool HarvestTile(Vector3 tile, out int quantity, out int quality)
    {
        quantity = 0; quality = 0;
        if (!tileToState.TryGetValue(tile, out CropState state)) { Debug.Log($"[CropManager] Nothing to harvest at {tile}"); return false; }
        if (state.isWithered) { Debug.Log($"[CropManager] Crop at {tile} is withered, can't harvest"); return false; }
        if (!state.isReadyToHarvest) { Debug.Log($"[CropManager] Crop at {tile} not ready"); return false; }

        CropModel model = tileToModel[tile];

        quality = CalculateQuality(state, model);
        quantity = CalculateQuantity(state, model);

        // выдача предмета в inventory — обязано делать внешний код (подпиской на событие) или вызовом Inventory API
        OnCropHarvested?.Invoke(tile, model, state, quantity, quality);

        if (model.regrows)
        {
            // вернуть на стадию регроу (частая реализация: оставить на последней несобранной стадии и ждать regrowDays)
            state.currentStage = Mathf.Max(0, state.currentStage - 1); // простая модель: остаётся немного назад
            state.daysInStage = 0;
            state.isReadyToHarvest = false;
            state.wateredStreak = 0;
            state.daysSincePlanted = 0;
        }
        else
        {
            // удаляем растение
            RemoveCropVisual(state);
            tileToState.Remove(tile);
            tileToModel.Remove(tile);
        }

        Debug.Log($"[CropManager] Harvested {model.cropId} at {tile} → {quantity} items, quality {quality}");
        return true;
    }

    public void FertilizeTile(Vector3 tile)
    {
        if (!tileToState.TryGetValue(tile, out CropState state))
        {
            Debug.Log($"[CropManager] Can't fertilize {tile}: no crop");
            return;
        }

        // например, напрямую уменьшаем daysInStage (аккуратно с минимумом)
        state.daysInStage = Mathf.Max(0, state.daysInStage - 1);
        OnCropFertilized?.Invoke(tile, tileToModel[tile], state);
        Debug.Log($"[CropManager] Fertilized {state.defCropId} at {tile}");
    }

    #endregion

    #region Day / Growth logic

    // Внешне вызывается при конце дня (или при смене дня)
    // currentSeason можно брать из GameTimeManager (здесь передаём параметром)
    public void OnDayEnd(Season currentSeason, HashSet<Vector3> irrigatedBySprinklers = null)
    {
        irrigatedBySprinklers = irrigatedBySprinklers ?? new HashSet<Vector3>();

        // Сначала применяем спринклеры:
        foreach (var irrig in irrigatedBySprinklers)
        {
            if (tileToState.TryGetValue(irrig, out CropState s))
                s.wateredToday = true;
        }

        // Копируем ключи, т.к. внутри цикла словарь может модифицироваться
        var tiles = tileToState.Keys.ToArray();

        foreach (var tile in tiles)
        {
            if (!tileToState.TryGetValue(tile, out CropState crop)) continue;
            if (!tileToModel.TryGetValue(tile, out CropModel model)) continue;

            // Сезонное увядание
            if (model.withersIfNotInSeason && model.seasons != null && model.seasons.Length > 0)
            {
                if (!model.seasons.Contains(currentSeason))
                {
                    crop.isWithered = true;
                    ReplaceVisualWithWithered(tile, model, crop);
                    continue;
                }
            }

            // Полив: если было полито — продвигаем прогресс, иначе сухой день
            if (crop.wateredToday)
            {
                crop.daysInStage += 1;
                crop.daysSincePlanted += 1;
                crop.wateredStreak += 1;
                crop.dryDays = 0;
            }
            else
            {
                crop.wateredStreak = 0;
                crop.dryDays += 1;
                // пример правила увядания при N сухих дней:
                if (crop.dryDays >= 3) // настройка по вкусу
                {
                    crop.isWithered = true;
                    ReplaceVisualWithWithered(tile, model, crop);
                    continue;
                }
            }

            // Переход стадии
            int needed = model.daysPerStage[Mathf.Clamp(crop.currentStage, 0, model.daysPerStage.Length - 1)];
            if (crop.daysInStage >= needed)
            {
                crop.currentStage++;
                crop.daysInStage = 0;
                OnCropGrown?.Invoke(tile, model, crop);
                UpdateVisualFor(tile, model, crop);
            }

            // Готовность к сбору
            crop.isReadyToHarvest = model.harvestStages.Contains(crop.currentStage);

            // Сброс флага полива для следующего дня
            crop.wateredToday = false;
        }
    }

    #endregion

    #region Visuals

    private void SpawnVisualFor(Vector3 tile, CropModel model, CropState state)
    {
        RemoveCropVisual(state); // на всякий
        int stageIndex = Mathf.Clamp(state.currentStage, 0, model.stagePrefabs.Length - 1);
        GameObject pref = model.stagePrefabs.Length > 0 ? model.stagePrefabs[stageIndex] : null;
        if (pref == null) return;
        state.visualInstance = Instantiate(pref, tile + model.offset, Quaternion.identity, this.transform);
        // настраиваем позиционирование (Y по модели)
    }

    private void UpdateVisualFor(Vector3 tile, CropModel model, CropState state)
    {
        if (state.isWithered)
        {
            ReplaceVisualWithWithered(tile, model, state);
            return;
        }
        // смена префаба при смене стадии
        if (state.visualInstance != null)
            Destroy(state.visualInstance);

        int stageIndex = Mathf.Clamp(state.currentStage, 0, model.stagePrefabs.Length - 1);
        GameObject pref = model.stagePrefabs.Length > 0 ? model.stagePrefabs[stageIndex] : null;
        if (pref == null) return;
        state.visualInstance = Instantiate(pref, tile, Quaternion.identity, this.transform);
    }

    private void ReplaceVisualWithWithered(Vector3 tile, CropModel model, CropState state)
    {
        RemoveCropVisual(state);
        if (model.witheredPrefab != null)
        {
            state.visualInstance = Instantiate(model.witheredPrefab, tile, Quaternion.identity, this.transform);
        }
    }

    private void RemoveCropVisual(CropState state)
    {
        if (state == null) return;
        if (state.visualInstance != null)
        {
            Destroy(state.visualInstance);
            state.visualInstance = null;
        }
    }

    #endregion

    #region Quality & Quantity

    private int CalculateQuality(CropState state, CropModel model)
    {
        // простой стохастический подсчёт качества
        float chanceSilver = model.baseSilverChance + state.wateredStreak * model.waterStreakSilverMultiplier;
        float chanceGold = model.baseGoldChance + state.wateredStreak * model.waterStreakGoldMultiplier;

        chanceSilver = Mathf.Clamp01(chanceSilver);
        chanceGold = Mathf.Clamp01(chanceGold);

        float r = UnityEngine.Random.value;
        if (r < chanceGold) return 2; // gold
        if (r < chanceGold + chanceSilver) return 1; // silver
        return 0; // normal
    }

    private int CalculateQuantity(CropState state, CropModel model)
    {
        // базово 1, модификаторы по качеству/модели
        int q = 1;
        if (model.multiHarvest) q += UnityEngine.Random.Range(0, 2); // 0..1 additional
        return q;
    }

    #endregion

    #region Save / Load (простой JSON)

    [Serializable]
    private class SaveData
    {
        public List<CropState> states;
        public List<TileModelPair> models;
        public List<Vector3> tilledTiles;
    }

    [Serializable]
    private class TileModelPair
    {
        public Vector3 pos;
        public string cropId;
    }

    public void SaveToDisk()
    {
        SaveData s = new SaveData();
        s.states = tileToState.Values.ToList();
        s.models = tileToModel.Select(kv => new TileModelPair() { pos = kv.Key, cropId = kv.Value.cropId }).ToList();
        s.tilledTiles = tilledTiles.ToList();

        string json = JsonUtility.ToJson(s, true);
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        File.WriteAllText(path, json);
        Debug.Log("CropManager: saved to " + path);
    }

    public void LoadFromDisk()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        if (!File.Exists(path)) { Debug.Log("No save found: " + path); return; }
        string json = File.ReadAllText(path);
        SaveData s = JsonUtility.FromJson<SaveData>(json);
        // очистить текущие
        foreach (var st in tileToState.Values) RemoveCropVisual(st);
        tileToState.Clear();
        tileToModel.Clear();
        tilledTiles.Clear();

        // восстановить модели по cropId
        var modelLookup = availableCropModels.ToDictionary(m => m.cropId, m => m);

        if (s.tilledTiles != null)
            tilledTiles = new HashSet<Vector3>(s.tilledTiles);

        if (s.models != null)
        {
            foreach (var pair in s.models)
            {
                if (modelLookup.TryGetValue(pair.cropId, out CropModel model))
                {
                    tileToModel[pair.pos] = model;
                }
            }
        }

        if (s.states != null)
        {
            foreach (var st in s.states)
            {
                tileToState[st.tilePos] = st;
                // восстановить визуалку
                if (tileToModel.TryGetValue(st.tilePos, out CropModel model))
                {
                    SpawnVisualFor(st.tilePos, model, st);
                }
            }
        }

        Debug.Log("CropManager: loaded from " + path);
    }

    #endregion

    #region Events

    public event Action<Vector3, CropModel, CropState> OnCropPlanted;
    public event Action<Vector3, CropModel, CropState> OnCropWatered;
    public event Action<Vector3, CropModel, CropState> OnCropGrown;
    public event Action<Vector3, CropModel, CropState> OnCropFertilized;
    public event Action<Vector3, CropModel, CropState, int, int> OnCropHarvested;

    #endregion
}
