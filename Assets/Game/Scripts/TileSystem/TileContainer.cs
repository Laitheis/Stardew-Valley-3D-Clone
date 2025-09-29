using System;
using System.Collections.Generic;
using UnityEngine;

public class TileData
{
    public bool isFarm;
    public object objectOnTile;

    public TileState CropState
    {
        get => objectOnTile as TileState;
        set => objectOnTile = value as TileState;
    }
}

public class TileContainer : MonoBehaviour
{
    #region for saving
    [Serializable]
    public class TileSaveData
    {
        public Vector3Int position;
        public TileData tile;
    }

    [Serializable]
    public class TileCollectionData
    {
        public List<TileSaveData> tiles = new List<TileSaveData>();
    }
    #endregion

    private Dictionary<Vector3Int, TileData> _tilesCollection;

    public Dictionary<Vector3Int, TileData> TilesCollection { get => _tilesCollection; set => _tilesCollection = value; }

    public void Combine(Dictionary<Vector3Int, TileData> kvp)
    {
        foreach (var tile in kvp)
        {
            if(_tilesCollection.ContainsKey(tile.Key))
            {
                Debug.Log("Dictionary merge error!");
                continue;
            }
            _tilesCollection.Add(tile.Key, tile.Value);
        }
    }

    public string SaveToJson()
    {
        TileCollectionData data = new TileCollectionData();

        foreach (var kvp in TilesCollection)
        {
            data.tiles.Add(new TileSaveData
            {
                position = kvp.Key,
                tile = kvp.Value
            });
        }

        string json = JsonUtility.ToJson(data, true);
        return json;
    }

    public void LoadFromJson(string json)
    {
        TileCollectionData data = JsonUtility.FromJson<TileCollectionData>(json);
        TilesCollection = new Dictionary<Vector3Int, global::TileData>();

        foreach (var tileData in data.tiles)
        {
            TilesCollection[tileData.position] = tileData.tile;
        }
    }

    public Dictionary<Vector3Int, global::TileData> GetFreeTiles()
    {
        Dictionary<Vector3Int, global::TileData> freeTiles = new Dictionary<Vector3Int, global::TileData>(); 
        foreach (var tile in TilesCollection)
        {
            if (tile.Value.objectOnTile != null) freeTiles.Add(tile.Key, tile.Value);
        }
        return freeTiles;
    }
}
