using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Newtonsoft.Json;

public class FarmManager : MonoBehaviour
{
    public static FarmManager instance;

    [Inject(Id = "FarmTiles")] public TileContainer farmTiles;
    [Inject] private DebrisGenerator _debrisGenerator;
    [Inject] private CropHandler _cropHandler;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        _cropHandler.InitTiles();
        LoadFarmTiles(SaveNameInputHolder.saveName);
    }

    [ContextMenu("SaveFarmData")]
    public void SaveFarmData()
    {
        List<string> jsonContents = new List<string>();
        jsonContents.Add(farmTiles.SaveToJson());
        SaveManager.Save(new SaveFileDataList() { saveName = "TestSave", jsonContents = jsonContents });
    }

    private void LoadFarmTiles(string saveName)
    {
        List<string> jsonContents = SaveManager.LoadListBySaveName(saveName);
        farmTiles.LoadFromJson(jsonContents[0]);
        _cropHandler.VisualiseTiles();
    }
}
