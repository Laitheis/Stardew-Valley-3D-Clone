using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FarmManager : MonoBehaviour, IBootstrapLoad
{
    public TileContainer farmTiles;

    [Inject] private DebrisGenerator _debrisGenerator;
    [Inject] private CropManager _cropManager;

    public void Init()
    {
        LoadFarmTiles(SaveNameHolder.saveName);
    }

    public void SaveFarmData()
    {
        List<string> jsonContents = new List<string>();
        jsonContents.Add(farmTiles.SaveToJson());
        SaveManager.Save(new SaveFileDataList() { saveName = "TestSave", jsonContents = jsonContents });
    }

    private void LoadFarmTiles(string saveName)
    {
        List<string> jsonContents = SaveManager.LoadListBySaveName(saveName);
        _cropManager.LoadFromJson(jsonContents[0]);
    }
}
