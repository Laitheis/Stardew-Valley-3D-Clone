using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Zenject;
using Random = UnityEngine.Random;

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager instance;

    [Inject(Id = "FarmTiles")] public TileContainer farmTiles;
    [Inject] private DebrisGeneratorController _debrisGenerator;
    [Inject] private CropController _cropHandler;
    [Inject] private DefinitionDatabase _definitionDatabase;
    [Inject] private DiContainer _diContainer;
    [Inject(Id ="PlayerInv")] private InventoryHandler _playerInv;

    private Vector2Int lowerLeftCorner = new Vector2Int(30, 30);
    private Vector2Int widthAndHeight = new Vector2Int(15, 20);

    public void Init()
    {
        Debug.Log("IsPlayerMale - " + PlayerData.isPlayerMale);

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        _cropHandler.InitTiles();

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

    public void Load()
    {
        List<string> jsonContents = null;
        if (SaveDataHolder.instance.saveGuid != Guid.Empty)
        {
            var save = SaveService.instance.LoadByFarmGuid(SaveDataHolder.instance.saveGuid, out jsonContents);

            PlayerData.farmName = save.farmName;
            PlayerData.playerName = save.playerName;
            GameTimeHandler.instance.totalDays = save.totalDays;
            PlayerData.farmGuid = Guid.Parse(save.farmGuid);
            farmTiles.LoadFromJson(jsonContents[0]);
            _playerInv.LoadFromJson(jsonContents[1]);
            _cropHandler.VisualiseTiles();
            VisualizeDebris();
        }
    }

    private void VisualizeDebris()
    {
        foreach (var tile in farmTiles)
        {
            if (tile.Value.objectOnTile is DebrisState d)
            {
                d.debrisVisualInstance = Instantiate(
                    _definitionDatabase.debrisModels.FirstOrDefault(model => model.debrisId == d.debrisModelId).worldPrefab, 
                    new Vector3(tile.Key.x, tile.Key.z, tile.Key.y),
                    Quaternion.Euler(new Vector3(0, Random.Range(0f, 360f),0))
                    );

                var destrObjBase = d.debrisVisualInstance.GetComponent<DestructibleObjectBase>();
                _diContainer.Inject(destrObjBase);
                destrObjBase.Init(tile.Key);
            }
        }
    }

    public void ClearAllFarmTiles()
    {
        foreach (var tile in farmTiles)
        {
            if(tile.Value.objectOnTile is DebrisState d)
            {
                Destroy(d.debrisVisualInstance);
            }
            if (tile.Value.objectOnTile is CropState c)
            {
                Destroy(c.cropVisualInstance);
                Destroy(c.soilVisualInstance);
            }
        }
        farmTiles.TilesCollection.Clear();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToGame()
    {
        GameStateService.instance.SetState(GameStateService.GameState.World);
    }
}
