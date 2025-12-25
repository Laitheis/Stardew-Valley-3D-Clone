using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class CropController : MonoBehaviour
{
    [Inject(Id = ("Soil"))] private GameObject _soilPrefab;
    [Inject(Id = ("SoilWet"))] private GameObject _soilWetPrefab;
    [Inject] private DefinitionDatabase _itemDatabase;
    [Inject(Id = "StarParticles")] private GameObject _particles;
    [Inject] private SignalBus _signalBus;

    private Dictionary<Vector3Int, TileState> _farmTiles;

    public void InitTiles()
    {
        _farmTiles = MainGameManager.instance.farmTiles.TilesCollection;
    }

    public void PlowTile(Vector3Int tile)
    {
        if (!_farmTiles.ContainsKey(tile))
        {
            Debug.Log($"[CropManager] Can't plow {tile}: empty tile"); return;
        }
        if (_farmTiles[tile].objectOnTile != null)
        {
            Debug.Log($"[CropManager] Can't plow {tile}: already occupied"); return;
        }
        if (!_farmTiles[tile].isFarm)
        {
            Debug.Log($"[CropManager] Can't plow {tile}: off-farm tile"); return;
        }

        var cropState = new CropState();
        _farmTiles[tile] = new TileState(cropState);

        SetSoilVisual(tile, false);
        cropState.soilVisualInstance.GetComponent<Animator>().SetTrigger("Plow");

        Debug.Log($"Tile {tile} plowed");
    }

    public bool IsPlowed(Vector3Int tile)
    {
        return _farmTiles.ContainsKey(tile) && _farmTiles[tile].objectOnTile is CropState;
    }

    public void UnplowTile(Vector3Int tile)
    {
        if (_farmTiles.ContainsKey(tile) && _farmTiles[tile].objectOnTile is CropState state)
        {
            if (state.model != null)
            {
                if (state.currentStage == state.model.daysPerStage.Length - 1)
                {
                    // Drop plant
                    HarvestTile(tile);
                }
                else
                {
                    // Drop seed
                    SeedDefinition seed = (SeedDefinition)_itemDatabase.itemDefinitions.Find(i => i is SeedDefinition seed && seed.cropModel == state.model);
                    ItemInstance item = new ItemInstance(seed, 1);
                    _signalBus.Fire(new ItemDropEvent(tile, item, false));
                }
            }

            // Remove soil and cropVisual
            if (state.soilVisualInstance != null)
            {
                Destroy(state.soilVisualInstance);
                Destroy(state.cropVisualInstance);
            }

            _farmTiles[tile].objectOnTile = null;
        }
    }

    public bool PlantSeed(Vector3Int tile, CropModel model)
    {
        if (!IsPlowed(tile))
        {
            Debug.Log($"[CropManager] Can't plant {model.cropId} at {tile}: tile not plowed"); return false;
        }
        if (IsPlanted(tile))
        {
            Debug.Log($"[CropManager] Can't plant {model.cropId} at {tile}: already occupied by another plant"); return false;
        }

        CropState state = _farmTiles[tile].objectOnTile as CropState;
        state.model = model;
        state.cropModelId = model.cropId;

        UpdateCropVisual(tile);

        Debug.Log($"[CropManager] Planted {model.displayName} at {tile}");
        return true;
    }

    public bool IsPlanted(Vector3Int tile)
    {
        return _farmTiles.TryGetValue(tile, out TileState s) && (s.objectOnTile is CropState st && st.model != null);
    }

    public void WaterTile(Vector3Int tile)
    {
        if (_farmTiles.TryGetValue(tile, out TileState s) && (s.objectOnTile is CropState state))
        {
            state.wateredToday = true;
            SetSoilVisual(tile, true);
            Debug.Log($"[CropManager] Watered {state.cropModelId} at {tile}");
        }
        else
        {
            Debug.Log($"[CropManager] Tried watering {tile}, but no soil here");
        }
    }

    public bool IsWatered(Vector3Int tile)
    {
        if (_farmTiles.TryGetValue(tile, out TileState state) && state.objectOnTile is CropState s && s.wateredToday)
            return true;
        return false;
    }

    public bool HarvestTile(Vector3Int tile)
    {
        int quantity = 0;
        int quality = 0;

        if (!_farmTiles.TryGetValue(tile, out TileState s))
        {
            Debug.Log($"[CropManager] Nothing to harvest at {tile}"); return false;
        }
        if (!(s.objectOnTile is CropState st))
        {
            Debug.Log($"[CropManager] Object on {tile} is not crop"); return false;
        }
        if (!(st.isReadyToHarvest))
        {
            Debug.Log($"[CropManager] Crop at {tile} is not ready"); return false;
        }

        CropState state = _farmTiles[tile].objectOnTile as CropState;

        quality = CalculateQuality(state);
        quantity = CalculateQuantity(state);

        SkillsManager.instance.skills.First(s => s.name == "Farming").XP += 21;

        ItemDefinition itemDef = GetItemByModel(state.model);
        Vector3 pos = new Vector3(tile.x, tile.z, tile.y);
        _signalBus.Fire(new ItemDropEvent(pos, new ItemInstance(itemDef, 1), false));

        if (state.model.regrows)
        {
            state.currentStage = Mathf.Max(0, state.currentStage - 1);
            state.daysInStage = 0;
            state.isReadyToHarvest = false;
            state.daysSincePlanted = 0;
            UpdateCropVisual(tile);
        }
        else
        {
            // Remove plant

            RemoveCropVisual(state);

            bool isWatered = state.wateredToday;
            var soilVisual = state.soilVisualInstance;

            _farmTiles[tile] = new TileState(new CropState() { soilVisualInstance = soilVisual, wateredToday = isWatered });
        }

        Debug.Log($"[CropManager] Harvested {state.model.cropId} at {tile} → {quantity} items, quality {quality}");
        return true;
    }

    public void FertilizeTile(Vector3Int tile, string name)
    {
        if (!_farmTiles.TryGetValue(tile, out TileState s))
        {
            Debug.Log($"[CropManager] Can't fertilize {tile}: nothing here"); return;
        }
        if (!(s.objectOnTile is CropState state))
        {
            Debug.Log($"[CropManager] Can't fertilize {tile}: object on tile is not plowed"); return;
        }

        switch (name)
        {
            case "Basic":
                state.isFertilizedQualityBasic = true;
                break;
            case "BasicPro":
                state.isFertilizedQualityPro = true;
                break;
            case "Speed":
                state.isFertilizedSpeed = true;
                break;
            case "SpeedPro":
                state.isFertilizedSpeedPro = true;
                break;
            case "Retaining":
                state.isFertilizedRetaining = true;
                break;
            case "Tree":
                state.isFertilizedTree = true;
                break;
            default:
                break;
        }

        Debug.Log($"[CropManager] Fertilized {state.cropModelId} at {tile}");
    }

    public void TryHarvestByHand(Vector3Int tile)
    {
        HarvestTile(tile);
    }

    public void OnDayEnd(Season currentSeason, HashSet<Vector3Int> irrigatedBySprinklers = null)
    {
        irrigatedBySprinklers = irrigatedBySprinklers ?? new HashSet<Vector3Int>();

        // Copy keys
        var tiles = _farmTiles.Keys.ToArray();
        List<Vector3Int> tilesToRemove = new List<Vector3Int>();
        foreach (var tile in tiles)
        {
            // Continue if tile is not crop
            if (!(_farmTiles.TryGetValue(tile, out TileState s) && s.objectOnTile is CropState state)) continue;

            // Continue and destroy soil if crop is empty
            if (state.model == null)
            {
                Destroy(state.soilVisualInstance);
                tilesToRemove.Add(tile);
                continue;
            }

            // Seasonal wilt
            if (state.model.withersIfNotInSeason && state.model.seasons != null && state.model.seasons.Length > 0)
            {
                if (!state.model.seasons.Contains(currentSeason))
                {
                    state.isWithered = true;
                    UpdateCropVisual(tile);
                    continue;
                }
            }

            if (state.isReadyToHarvest) continue;

            // If it was watered - advance progress, else dry day
            bool waterToNextDay = false;
            if (state.wateredToday)
            {
                state.daysInStage++;
                state.daysSincePlanted++;
                state.dryDays = 0;
                if (state.isFertilizedSpeed) state.daysInStage++;
                if (state.isFertilizedSpeedPro) state.daysInStage += 2;
                if (state.isFertilizedRetaining)
                {
                    bool result = UnityEngine.Random.Range(0, 2) == 1 ? true : false;
                    if (result) waterToNextDay = true;
                }
            }
            else
            {
                state.dryDays += 1;

                if (state.dryDays >= 3)
                {
                    state.isWithered = true;
                    UpdateCropVisual(tile);
                    goto SetDrySoil;
                }
            }

            // Stage transition
            int needed = state.model.daysPerStage[Mathf.Clamp(state.currentStage, 0, state.model.daysPerStage.Length - 1)];
            if (state.daysInStage >= needed && state.wateredToday)
            {
                float diff = state.daysInStage - needed;
                float lerpDiff = Mathf.InverseLerp(0, 3, diff);
                float random = UnityEngine.Random.Range(lerpDiff, 1);
                if (random > 0.5f) // Check for next stage with random (every day chance grow)
                {
                    state.currentStage++;
                    state.daysInStage = 0;
                    UpdateCropVisual(tile);
                }
            }

        SetDrySoil:
            if (!waterToNextDay)
            {
                state.wateredToday = false;
                SetSoilVisual(tile, false);
            }

            // Ready to harvest
            state.isReadyToHarvest = state.currentStage == state.model.stagePrefabs.Length - 1;
        }

        // Use sprinklers
        foreach (var irrig in irrigatedBySprinklers)
        {
            if (_farmTiles.TryGetValue(irrig, out TileState s) && s.objectOnTile is CropState state)
                state.wateredToday = true;
        }

        // Clear empty tiles
        foreach (var tile in tilesToRemove)
        {
            _farmTiles[tile].objectOnTile = null;
        }
    }

    private void UpdateCropVisual(Vector3Int tile)
    {
        Vector3Int pos = new Vector3Int(tile.x, tile.z, tile.y);
        _farmTiles.TryGetValue(tile, out TileState s);
        CropState state = s.objectOnTile as CropState;
        if (state == null) return;

        if (state.isWithered)
        {
            RemoveCropVisual(state);

            if (state.model.witheredPrefab != null)
            {
                state.cropVisualInstance = Instantiate(state.model.witheredPrefab, pos, Quaternion.identity, this.transform);
            }
            return;
        }

        RemoveCropVisual(state);

        int stageIndex = Mathf.Clamp(state.currentStage, 0, state.model.stagePrefabs.Length - 1);
        GameObject pref = state.model.stagePrefabs.Length > 0
            ? state.model.stagePrefabs[stageIndex]
            : null;
        if (pref == null) return;

        state.cropVisualInstance = Instantiate(pref, pos, Quaternion.identity);
        state.cropVisualInstance.AddComponent<TilePosHolder>().pos = tile;

        if (state.currentStage == state.model.daysPerStage.Length - 1)
            Instantiate(_particles, state.cropVisualInstance.transform);
    }

    private void RemoveCropVisual(CropState state)
    {
        if (state == null) return;
        if (state.cropVisualInstance != null)
        {
            Destroy(state.cropVisualInstance);
            state.cropVisualInstance = null;
        }
    }

    private void SetSoilVisual(Vector3Int tile, bool wet)
    {
        CropState state = _farmTiles[tile].objectOnTile as CropState;
        var soil = state.soilVisualInstance;
        Destroy(soil);
        Vector3Int pos = new Vector3Int(tile.x, tile.z, tile.y);
        state.soilVisualInstance = wet ? Instantiate(_soilWetPrefab, pos, Quaternion.identity) : Instantiate(_soilPrefab, pos, Quaternion.identity);
    }

    private int CalculateQuality(CropState state)
    {
        float chanceSilver = state.model.baseSilverChance + state.model.waterStreakSilverMultiplier;
        float chanceGold = state.model.baseGoldChance + state.model.waterStreakGoldMultiplier;

        chanceSilver = Mathf.Clamp01(chanceSilver);
        chanceGold = Mathf.Clamp01(chanceGold);

        float r = UnityEngine.Random.value;
        if (r < chanceGold) return 2; // gold
        if (r < chanceGold + chanceSilver) return 1; // silver
        return 0; // normal
    }

    private int CalculateQuantity(CropState state)
    {
        int q = 1;
        if (state.model.multiHarvest) q += UnityEngine.Random.Range(0, 2); // 0..1 additional
        return q;
    }

    public bool CheckCropOnTile(Vector3Int tile)
    {
        return _farmTiles.TryGetValue(tile, out TileState s) && s.objectOnTile is CropState state && state.model != null;
    }

    private ItemDefinition GetItemByModel(CropModel cropModel)
    {
        return _itemDatabase.itemDefinitions.Find(i => i is CropDefinition c && c.cropModel == cropModel);
    }

    public void VisualiseTiles()
    {
        // Load
        foreach (var tile in MainGameManager.instance.farmTiles)
        {
            if (tile.Value.objectOnTile == null || !(tile.Value.objectOnTile is CropState)) continue;
            CropState cropState = tile.Value.objectOnTile as CropState;
            if (cropState.cropModelId != null)
            {
                cropState.model = _itemDatabase.cropModels.First(c => c.cropId == cropState.cropModelId);
                UpdateCropVisual(tile.Key);
            }
            SetSoilVisual(tile.Key, cropState.wateredToday);
        }

        Debug.Log("CropManager: loaded crop tiles");
    }

    [ContextMenu("Test Cabbage")]
    public void TestCabbage()
    {
        ItemInstance item = new ItemInstance();
        item.SetItemDefById(1);
        item.SetCount(1);
        _signalBus.Fire(new ItemDropEvent(new Vector3(32, 0, 35), item, false));
    }
}
