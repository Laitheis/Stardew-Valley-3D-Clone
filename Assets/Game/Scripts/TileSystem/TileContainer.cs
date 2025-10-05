using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public interface IObjectOnTile { }

[Serializable]
public class TileState
{
    public bool isFarm;
    public object objectOnTile;
}

public class TileContainer : MonoBehaviour
{
    #region for saving
    [Serializable]
    public class TileSaveData
    {
        public Vector3Int position;
        public TileState tile;
    }

    [Serializable]
    public class TileCollectionData
    {
        public List<TileSaveData> tiles = new List<TileSaveData>();
    }
    #endregion

    private Dictionary<Vector3Int, TileState> _tilesCollection = new();

    public Dictionary<Vector3Int, TileState> TilesCollection { get => _tilesCollection; set => _tilesCollection = value; }

    public void Combine(Dictionary<Vector3Int, TileState> kvp)
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

        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        string json = JsonConvert.SerializeObject(data, Formatting.Indented, settings);
        return json;
    }

    public void LoadFromJson(string json)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        //TileCollectionData data = JsonUtility.FromJson<TileCollectionData>(json);
        TileCollectionData data = JsonConvert.DeserializeObject<TileCollectionData>(json, settings);
         
        //TilesCollection = new Dictionary<Vector3Int, TileState>();

        foreach (var tileData in data.tiles)
        {
            TilesCollection[tileData.position] = tileData.tile;
        }
    }

    public Dictionary<Vector3Int, TileState> GetFreeTiles()
    {
        Dictionary<Vector3Int, TileState> freeTiles = new Dictionary<Vector3Int, TileState>(); 
        foreach (var tile in TilesCollection)
        {
            if (tile.Value.objectOnTile != null) freeTiles.Add(tile.Key, tile.Value);
        }
        return freeTiles;
    }
}
