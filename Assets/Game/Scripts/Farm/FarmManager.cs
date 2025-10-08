using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FarmManager : MonoBehaviour
{
    public static FarmManager instance;

    [Inject(Id = "FarmTiles")] public TileContainer farmTiles;
    [Inject] private DebrisGeneratorController _debrisGenerator;
    [Inject] private CropController _cropHandler;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        _cropHandler.InitTiles();

    }
    private void Start()
    {
        if (SaveDataHolder.instance != null)
        {
            if (SaveDataHolder.instance.isFirstLaunch)
                PlayerData.farmGuid = SaveDataHolder.instance.saveGuid;
            else
                Load();
        }
        else 
            PlayerData.farmGuid = Guid.Empty;

    }
    private void Load()
    {
        List<string> jsonContents = null;
        if (SaveDataHolder.instance.saveGuid != Guid.Empty)
        {
            var save = SaveService.instance.LoadByFarmGuid(SaveDataHolder.instance.saveGuid, out jsonContents);

            PlayerData.farmName = save.farmName;
            PlayerData.playerName = save.playerName;
            GameTimeService.instance.currentDay = save.currentDay;
            PlayerData.farmGuid = Guid.Parse(save.farmGuid);
            farmTiles.LoadFromJson(jsonContents[0]);
            _cropHandler.VisualiseTiles();
        }
    }
}
