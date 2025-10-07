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
        LoadFarmTiles();
    }

    private void LoadFarmTiles()
    {
        List<string> jsonContents = null;
        if (SaveGuidHolder.saveGiud != Guid.Empty)
        {
            jsonContents = SaveService.instance.LoadByFarmGuid(SaveGuidHolder.saveGiud);
        }
        if (jsonContents != null)
        {
            farmTiles.LoadFromJson(jsonContents[0]);
            _cropHandler.VisualiseTiles();
        }
    }
}
