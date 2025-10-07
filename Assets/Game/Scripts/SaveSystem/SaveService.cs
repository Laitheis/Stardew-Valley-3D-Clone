using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[Serializable]
public class SaveFileDataList
{
    public Guid farmGuid;
    public string playerName;
    public string farmName;
    public int currentDay;
    public string dateTime;
    public List<string> jsonContents;
}

public class SaveService : MonoBehaviour
{
    public static SaveService instance;

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
    }

    public void Save()
    {
        if (!Directory.Exists(SavesFolder))
            Directory.CreateDirectory(SavesFolder);

        string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"{PlayerData.playerName}_{PlayerData.farmName}_{PlayerData.farmGuid}.json";
        string fullPath = Path.Combine(SavesFolder, fileName);

        List<string> jsonContents = new List<string>();
        jsonContents.Add(FarmManager.instance.farmTiles.SaveToJson());

        SaveFileDataList data = new SaveFileDataList
        {
            farmGuid = PlayerData.farmGuid,
            playerName = PlayerData.playerName,
            farmName = PlayerData.farmName,
            dateTime = dateTime,
            currentDay = GameTimeService.instance.currentDay,
            jsonContents = jsonContents
        };

        string fileJson = JsonUtility.ToJson(data, true);
        File.WriteAllText(fullPath, fileJson);

        Debug.Log($"Saved in: {fullPath}");
    }

    public List<string> LoadByFarmGuid(Guid farmGuid)
    {
        if (!Directory.Exists(SavesFolder))
            return null;

        string[] files = Directory.GetFiles(SavesFolder, "*.json");
        foreach (string file in files)
        {
            string fileJson = File.ReadAllText(file);

            SaveFileDataList data = JsonUtility.FromJson<SaveFileDataList>(fileJson);
            if (data != null && data.farmGuid == farmGuid)
            {
                Debug.Log($"Loaded: {file}");
                return data.jsonContents;
            }
        }

        Debug.LogWarning($"Save not found.");
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
}
