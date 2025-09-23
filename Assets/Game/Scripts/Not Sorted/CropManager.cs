using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using Zenject;

public class CropManager : MonoBehaviour
{
    public static CropManager Instance;

    // Пользовательские коллекции по требованию:
    public Dictionary<Vector3Int, TileState> tileToState = new Dictionary<Vector3Int, TileState>();

    // Все доступные модели (подвесить в инспекторе или загружать динамически)
    public CropModel[] availableCropModels;

    // Настройка для сохранения
    [SerializeField] private string saveFileName = "cropsave.json";

    [Inject(Id = ("Soil"))] private GameObject _soilPrefab; 
    [Inject(Id = ("SoilWet"))] private GameObject _soilWetPrefab;
    [Inject] private ItemDatabase _itemDatabase;
    [Inject] private SignalBus _signalBus;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #region Public API

    // Вспомог: округляем позицию до целого тайла (важно — единый контракт: Y=0)
    public static Vector3Int TilePosFromWorld(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x);
        int z = Mathf.RoundToInt(worldPos.z);
        return new Vector3Int(x, 0, z);
    }

    public void PlowTile(Vector3 tile)
    {
        Vector3Int crdsInt = Vector3Int.CeilToInt(tile);
        if (tileToState.ContainsKey(crdsInt)) { Debug.Log($"[CropManager] Can't plow {crdsInt}: already occupied"); return; }

        var cropState = new TileState();
        cropState.soilVisualInstance = Instantiate(_soilPrefab, crdsInt, Quaternion.identity);
        cropState.soilVisualInstance.GetComponent<Animator>().SetTrigger("Plow");

        Instance.tileToState.Add(crdsInt, cropState);
        
        Debug.Log($"Tile {crdsInt} plowed");
    }

    public bool IsPlowed(Vector3Int tile)
    {
        return tileToState.ContainsKey(tile);
    }

    public void UnplowTile(Vector3Int tile)
    {
        Vector3Int crdsInt = Vector3Int.CeilToInt(tile);
        tileToState.Remove(crdsInt);
    }

    public bool PlantSeed(Vector3Int tile, CropModel model)
    {
        Vector3Int crdsInt = Vector3Int.CeilToInt(tile);

        if (!IsPlowed(crdsInt)) { Debug.Log($"[CropManager] Can't plant {model.cropId} at {crdsInt}: tile not plowed"); return false; }
        if (tileToState.ContainsKey(crdsInt) && tileToState[crdsInt].crop != null) { Debug.Log($"[CropManager] Can't plant {model.cropId} at {crdsInt}: already occupied"); return false; }

        TileState state = tileToState[tile];
        state.crop = model;
        state.defCropId = model.cropId;

        tileToState[crdsInt] = state;

        SpawnVisualFor(crdsInt, model, state);

        OnCropPlanted?.Invoke(crdsInt, state);
        Debug.Log($"[CropManager] Planted {model.displayName} at {tile}");
        return true;
    }

    public void WaterTile(Vector3 tile)
    {
        Vector3Int crdsInt = Vector3Int.CeilToInt(tile);

        if (tileToState.TryGetValue(crdsInt, out TileState state))
        {
            state.wateredToday = true;
            OnCropWatered?.Invoke(crdsInt, state);
            var soil = state.soilVisualInstance;
            Destroy(soil);
            state.soilVisualInstance = Instantiate(_soilWetPrefab, crdsInt, Quaternion.identity);
            Debug.Log($"[CropManager] Watered {state.defCropId} at {crdsInt}");
        }
        else
        {
            Debug.Log($"[CropManager] Tried watering {crdsInt}, but no crop here");
        }
    }

    public bool IsWatered(Vector3Int tile)
    {
        if (tileToState.ContainsKey(tile) && tileToState[tile].wateredToday)
            return true;
        return false;
    }

    // Harvest (return true if succsess)
    public bool HarvestTile(Vector3 tile, out int quantity, out int quality)
    {
        Vector3Int crdsInt = Vector3Int.CeilToInt(tile);

        quantity = 0; 
        quality = 0;
        if (!tileToState.TryGetValue(crdsInt, out TileState state)) { Debug.Log($"[CropManager] Nothing to harvest at {crdsInt}"); return false; }
        //if (state.isWithered) { Debug.Log($"[CropManager] Crop at {crdsInt} is withered, can't harvest"); return false; }
        if (!state.isReadyToHarvest) { Debug.Log($"[CropManager] Crop at {crdsInt} not ready"); return false; }

        CropModel model = tileToState[crdsInt].crop;

        quality = CalculateQuality(state, model);
        quantity = CalculateQuantity(state, model);

        //TODO выбрасывание собранного растения в мир
        OnCropHarvested?.Invoke(crdsInt, state, quantity, quality);

        ItemDefinition itemDef = GetItemByModel(state.crop);
        _signalBus.Fire(new ItemDropEvent(tile, new ItemInstance(itemDef, 1), false));

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
            state.soilVisualInstance = Instantiate(_soilPrefab, crdsInt, Quaternion.identity);

            state.isWithered = false;
            state.wateredToday = false;
            state.wateredStreak = 0;
        }

        tileToState.Remove(crdsInt);
        Debug.Log($"[CropManager] Harvested {model.cropId} at {tile} → {quantity} items, quality {quality}");
        return true;
    }

    public void FertilizeTile(Vector3 tile)
    {
        Vector3Int crdsInt = Vector3Int.CeilToInt(tile);

        if (!tileToState.TryGetValue(crdsInt, out TileState state))
        {
            Debug.Log($"[CropManager] Can't fertilize {crdsInt}: no crop");
            return;
        }

        // например, напрямую уменьшаем daysInStage (аккуратно с минимумом)
        state.daysInStage = Mathf.Max(0, state.daysInStage - 1);
        OnCropFertilized?.Invoke(crdsInt, state);
        Debug.Log($"[CropManager] Fertilized {state.defCropId} at {crdsInt}");
    }

    #endregion

    #region Day / Growth logic

    // Внешне вызывается при конце дня (или при смене дня)
    // currentSeason можно брать из GameTimeManager (здесь передаём параметром)
    public void OnDayEnd(Season currentSeason, HashSet<Vector3Int> irrigatedBySprinklers = null)
    {
        irrigatedBySprinklers = irrigatedBySprinklers ?? new HashSet<Vector3Int>();

        // Сначала применяем спринклеры:
        foreach (var irrig in irrigatedBySprinklers)
        {
            if (tileToState.TryGetValue(irrig, out TileState s))
                s.wateredToday = true;
        }

        // Копируем ключи, т.к. внутри цикла словарь может модифицироваться
        var tiles = tileToState.Keys.ToArray();

        foreach (var tile in tiles)
        {
            if (!tileToState.TryGetValue(tile, out TileState state)) continue;
            //if (!tileToModel.TryGetValue(tile, out CropModel model)) continue;
            if (state.crop == null) goto dropWateredState;
            
            // Сезонное увядание
            if (state.crop.withersIfNotInSeason && state.crop.seasons != null && state.crop.seasons.Length > 0)
            {
                if (!state.crop.seasons.Contains(currentSeason))
                {
                    state.isWithered = true;
                    ReplaceVisualWithWithered(tile, state);
                    continue;
                }
            }

            // Полив: если было полито — продвигаем прогресс, иначе сухой день
            if (state.wateredToday)
            {
                state.daysInStage += 1;
                state.daysSincePlanted += 1;
                state.wateredStreak += 1;
                state.dryDays = 0;
            }
            else
            {
                state.wateredStreak = 0;
                state.dryDays += 1;
                // пример правила увядания при N сухих дней:
                if (state.dryDays >= 3) // настройка по вкусу
                {
                    state.isWithered = true;
                    ReplaceVisualWithWithered(tile, state);
                    continue;
                }
            }

            // Переход стадии
            int needed = state.crop.daysPerStage[Mathf.Clamp(state.currentStage, 0, state.crop.daysPerStage.Length - 1)];
            if (state.daysInStage >= needed)
            {
                state.currentStage++;
                state.daysInStage = 0;
                OnCropGrown?.Invoke(tile, state);
                UpdateVisualFor(tile, state.crop, state);
            }

            // Готовность к сбору
            state.isReadyToHarvest = state.currentStage == state.crop.stagePrefabs.Length - 1;

        dropWateredState:
            // Сброс флага полива для следующего дня
            state.wateredToday = false;

            Destroy(state.soilVisualInstance);
            state.soilVisualInstance = Instantiate(_soilPrefab, tile, Quaternion.identity);
        }
    }

    #endregion

    #region Visuals

    private void SpawnVisualFor(Vector3Int tile, CropModel model, TileState state)
    {
        RemoveCropVisual(state); // на всякий
        int stageIndex = Mathf.Clamp(state.currentStage, 0, model.stagePrefabs.Length - 1);
        GameObject pref = model.stagePrefabs.Length > 0 ? model.stagePrefabs[stageIndex] : null;
        if (pref == null) return;

        Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        state.cropVisualInstance = Instantiate(pref, tile + model.offset, rotation, this.transform);
    }

    private void UpdateVisualFor(Vector3 tile, CropModel model, TileState state)
    {
        if (state.isWithered)
        {
            ReplaceVisualWithWithered(tile, state);
            return;
        }
        // смена префаба при смене стадии
        if (state.cropVisualInstance != null)
            Destroy(state.cropVisualInstance);

        int stageIndex = Mathf.Clamp(state.currentStage, 0, model.stagePrefabs.Length - 1);
        GameObject pref = model.stagePrefabs.Length > 0 ? model.stagePrefabs[stageIndex] : null;
        if (pref == null) return;
        state.cropVisualInstance = Instantiate(pref, tile, Quaternion.identity, this.transform);
    }

    private void ReplaceVisualWithWithered(Vector3 tile, TileState state)
    {
        RemoveCropVisual(state);
        if (state.crop.witheredPrefab != null)
        {
            state.cropVisualInstance = Instantiate(state.crop.witheredPrefab, tile, Quaternion.identity, this.transform);
        }
    }

    private void RemoveCropVisual(TileState state)
    {
        if (state == null) return;
        if (state.cropVisualInstance != null)
        {
            Destroy(state.cropVisualInstance);
            state.cropVisualInstance = null;
        }
    }

    #endregion

    #region Quality & Quantity

    private int CalculateQuality(TileState state, CropModel model)
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

    private int CalculateQuantity(TileState state, CropModel model)
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
        public Dictionary<Vector3, TileState> cropTiles;
    }

    public void SaveToDisk()
    {
        SaveData s = new SaveData();
        //s.cropTiles = tileToState;

        string json = JsonUtility.ToJson(s, true);
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        File.WriteAllText(path, json);
        Debug.Log("CropManager: saved to " + path);
    }

    public class HarvestResult
    {
        public bool success;

        public CropModel harvested;
    }
    public void TryHarvestByHand(Vector3Int tile)
    {
        HarvestTile(tile, out int q, out int q2);
    }
    //public void LoadFromDisk()
    //{
    //    string path = Path.Combine(Application.persistentDataPath, saveFileName);
    //    if (!File.Exists(path)) { Debug.Log("No save found: " + path); return; }
    //    string json = File.ReadAllText(path);
    //    SaveData s = JsonUtility.FromJson<SaveData>(json);
    //    // очистить текущие
    //    foreach (var st in tileToState.Values) RemoveCropVisual(st);
    //    tileToState.Clear();
    //    tileToModel.Clear();
    //    //plowedTiles.Clear();

    //    // восстановить модели по cropId
    //    var modelLookup = availableCropModels.ToDictionary(m => m.cropId, m => m);

    //    if (s.cropTiles != null)
    //        plowedTiles = new HashSet<Vector3>(s.cropTiles);

    //    if (s.models != null)
    //    {
    //        foreach (var pair in s.models)
    //        {
    //            if (modelLookup.TryGetValue(pair.cropId, out CropModel model))
    //            {
    //                tileToModel[pair.pos] = model;
    //            }
    //        }
    //    }

    //    if (s.states != null)
    //    {
    //        foreach (var st in s.states)
    //        {
    //            tileToState[st.tilePos] = st;
    //            // восстановить визуалку
    //            if (tileToModel.TryGetValue(st.tilePos, out CropModel model))
    //            {
    //                SpawnVisualFor(st.tilePos, model, st);
    //            }
    //        }
    //    }

        //Debug.Log("CropManager: loaded from " + path);
    //}
    ItemDefinition GetItemByModel(CropModel crop)
    {
        return _itemDatabase._itemDefinitions.Find(i => ((SeedDefinition)i).cropModel == crop);
    }
    #endregion

    #region Events

    public event Action<Vector3Int, TileState> OnCropPlanted;
    public event Action<Vector3Int, TileState> OnCropWatered;
    public event Action<Vector3Int, TileState> OnCropGrown;
    public event Action<Vector3Int, TileState> OnCropFertilized;
    public event Action<Vector3Int, TileState, int, int> OnCropHarvested;

    #endregion
}
