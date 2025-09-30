using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DebrisGenerator : MonoBehaviour
{
    [Inject(Id = "FarmTiles")] private TileContainer _farmTiles;

    [SerializeField] private List<GameObject> _debrisObjects;

    public void GenerateDebris()
    {
        Dictionary<Vector3Int, TileState> freeTiles =  _farmTiles.GetFreeTiles();

        foreach (var tile in freeTiles)
        {
            GameObject randomObject = _debrisObjects[Random.Range(0, _debrisObjects.Count - 1)];
            Vector3 randomRotation = new Vector3(0, Random.Range(0f, 360f), 0);
            Quaternion rotation = Quaternion.Euler(randomRotation);
            Vector3 position = new Vector3(tile.Key.x, tile.Key.y, 0);
            Vector3 spawnOffset = new Vector3(0, 0, 0);
            Instantiate(randomObject, position + spawnOffset, rotation);
        }
    }
}
