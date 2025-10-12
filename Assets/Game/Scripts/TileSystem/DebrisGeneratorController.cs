using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DebrisGeneratorController : MonoBehaviour
{
    [SerializeField, Range(0, 100)] private float density = 30f;

    [Inject(Id = "FarmTiles")] private TileContainer _farmTiles;
    [Inject] private DefinitionDatabase _definitionDatabase;
    [Inject] private DiContainer _diContainer;

    private List<DebrisModel> _debrisModels;

    public void Init()
    {
        _debrisModels = _definitionDatabase.debrisModels;
    }
    public void GenerateDebris()
    {
        Dictionary<Vector3Int, TileState> freeTiles =  _farmTiles.GetFreeTiles();

        foreach (var tile in freeTiles)
        {
            float result = Random.Range(0f, 100f);
            if (result > density) continue;

            DebrisModel debrisModel = _debrisModels[Random.Range(0, _debrisModels.Count)];
            Vector3 randomRotation = new Vector3(0, Random.Range(0f, 360f), 0);
            Quaternion rotation = Quaternion.Euler(randomRotation);
            Vector3 position = new Vector3(tile.Key.x, 0, tile.Key.y);
            Vector3 spawnOffset = new Vector3(0, 0, 0);
            var GO = Instantiate(debrisModel.worldPrefab, position + spawnOffset, rotation);
            _farmTiles[tile.Key] = new TileState(new DebrisState() { model = debrisModel, debrisModelId = debrisModel.debrisId, debrisVisualInstance = GO, tilePos = tile.Key});
            var destrObjBase =  GO.GetComponent<DestructibleObjectBase>();
            destrObjBase.Init(tile.Key);
            _diContainer.Inject(destrObjBase);
        }
    }
}
