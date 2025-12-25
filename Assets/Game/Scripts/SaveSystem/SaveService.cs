using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

[Serializable]
public class SaveFileDataList
{
    public string farmGuid;
    public string playerName;
    public string farmName;
    public int totalDays;
    public string dateTime;
    public List<string> jsonContents;
}

public class SaveService : MonoBehaviour
{
    public static SaveService instance;

    private InventoryHandler _playerInv;

    private string SavesFolder => Path.Combine(Application.persistentDataPath, "Saves");

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if(scene.name == "Gameplay")
        {
            SceneContext sceneContext = FindAnyObjectByType<SceneContext>();
            DiContainer container = sceneContext.Container;
            _playerInv = container.ResolveId<InventoryHandler>("PlayerInv");
        }
    }

    public void Save()
    {
        if (!Directory.Exists(SavesFolder))
            Directory.CreateDirectory(SavesFolder);

        string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"{PlayerData.playerName}_{PlayerData.farmName}_{PlayerData.farmGuid}.json";
        string fullPath = Path.Combine(SavesFolder, fileName);

        List<string> jsonContents = new List<string>();
        jsonContents.Add(MainGameManager.instance.farmTiles.SaveToJson());
        jsonContents.Add(_playerInv.SaveToJson());

        SaveFileDataList data = new SaveFileDataList
        {
            farmGuid = PlayerData.farmGuid.ToString(),
            playerName = PlayerData.playerName,
            farmName = PlayerData.farmName,
            dateTime = dateTime,
            totalDays = GameTimeHandler.instance.totalDays,
            jsonContents = jsonContents
        };

        string fileJson = JsonUtility.ToJson(data, true);
        File.WriteAllText(fullPath, fileJson);

        Debug.Log($"Saved in: {fullPath}");
    }

    public SaveFileDataList LoadByFarmGuid(Guid farmGuid, out List<string> jsonContents)
    {
        if (!Directory.Exists(SavesFolder))
        {
            jsonContents = null;
            return null;
        }

        string[] files = Directory.GetFiles(SavesFolder, "*.json");
        foreach (string file in files)
        {
            string fileJson = File.ReadAllText(file);

            SaveFileDataList data = JsonUtility.FromJson<SaveFileDataList>(fileJson);
            if (data != null && Guid.Parse(data.farmGuid) == farmGuid)
            {
                Debug.Log($"Loaded: {file}");
                jsonContents = data.jsonContents;
                return data;
            }
        }

        Debug.LogWarning($"Save not found.");
        jsonContents = null;
        return null;
    }

    public List<SaveFileDataList> GetAllSavesList()
    {
        List<SaveFileDataList> saves = new List<SaveFileDataList>();
        if (!Directory.Exists(SavesFolder))
            return saves;

        string[] files = Directory.GetFiles(SavesFolder, "*.json");
        foreach (string file in files)
        {
            string fileJson = File.ReadAllText(file);
            SaveFileDataList data = JsonUtility.FromJson<SaveFileDataList>(fileJson);
            saves.Add(data);
        }

        return saves;
    }

    [ContextMenu("UseTestSave")]
    public void UseTestSave()
    {
        MainGameManager.instance.ClearAllFarmTiles();
        SaveDataHolder.instance.saveGuid = Guid.Parse("f3b0a4cb-02e5-4e0f-9f7d-9cb0e0f40f8d");
        MainGameManager.instance.Load();
    }

    [ContextMenu("CreateTestSave")]
    public void CreateTestSave()
    {
        PlayerData.playerName = "TestSavePlayer";
        PlayerData.farmName = "TestSaveFarm";
        PlayerData.farmGuid = Guid.Parse("f3b0a4cb-02e5-4e0f-9f7d-9cb0e0f40f8d");
        PlayerData.isPlayerMale = true;
        SaveDataHolder.instance.saveGuid = PlayerData.farmGuid;
        Save();
    }
}
