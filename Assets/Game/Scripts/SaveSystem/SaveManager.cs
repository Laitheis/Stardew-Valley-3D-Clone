using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveFileDataList
{
    public string saveName;
    public string dateTime;
    public List<string> jsonContents;
}

public static class SaveManager
{
    private static string SavesFolder => Path.Combine(Application.persistentDataPath, "Saves");

    public static void Save(SaveFileDataList saveData)
    {
        if (!Directory.Exists(SavesFolder))
            Directory.CreateDirectory(SavesFolder);

        string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"{dateTime}_{saveData.saveName}.json";
        string fullPath = Path.Combine(SavesFolder, fileName);

        SaveFileDataList data = new SaveFileDataList
        {
            saveName = saveData.saveName,
            dateTime = dateTime,
            jsonContents = saveData.jsonContents
        };

        string fileJson = JsonUtility.ToJson(data, true);
        File.WriteAllText(fullPath, fileJson);

        Debug.Log($"Saved in: {fullPath}");
    }

    public static List<string> LoadListBySaveName(string saveName)
    {
        if (!Directory.Exists(SavesFolder))
            return null;

        string[] files = Directory.GetFiles(SavesFolder, "*.json");
        foreach (string file in files)
        {
            string fileJson = File.ReadAllText(file);

            SaveFileDataList data = JsonUtility.FromJson<SaveFileDataList>(fileJson);
            if (data != null && data.saveName == saveName)
            {
                Debug.Log($"Loaded: {file}");
                return data.jsonContents;
            }
        }

        Debug.LogWarning($"Сохранение с именем {saveName} не найдено.");
        return null;
    }

    public static List<SaveFileDataList> GetAllSavesList()
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
