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
        LoadFarmTiles();
    }

    public void SaveFarmData()
    {
        List<string> jsonContents = new List<string>();
        _farmTiles.Combine(_cropManager.CropTiles);
        jsonContents.Add(_farmTiles.SaveToJson());
        SaveManager.Save(new SaveFileDataList() { saveName = "TestSave", jsonContents =})
    }

    private void LoadFarmTiles()
    {
        foreach (var tile in _farmTiles.TilesCollection)
        {
            tile.Key.
        }
    }
}
