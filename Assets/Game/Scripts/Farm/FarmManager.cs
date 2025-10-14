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

    private Vector2Int lowerLeftCorner = new Vector2Int(30, 30);
    private Vector2Int widthAndHeight = new Vector2Int(15, 20);

    public void Init()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        farmTiles.FillSquare(lowerLeftCorner, widthAndHeight);

        if (SaveDataHolder.instance != null)
        {
            // First launch settings
            if (SaveDataHolder.instance.isFirstLaunch)
            {
                PlayerData.farmGuid = SaveDataHolder.instance.saveGuid;
                _debrisGenerator.GenerateDebris(30f);
            }
            else
                Load();
        }
        else // Debuging launch Gameplay scene 
        {
            PlayerData.farmGuid = Guid.Empty;
            _debrisGenerator.GenerateDebris(30f);
        }

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
