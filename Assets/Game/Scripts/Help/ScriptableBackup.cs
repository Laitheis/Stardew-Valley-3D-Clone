using UnityEditor;
using UnityEngine;
using System.IO;

public static class ScriptableBackup
{
    [MenuItem("Tools/Backup Scriptables (Resources Only)")]
    public static void BackupResourcesScriptables()
    {
        string backupDir = "Assets/Backups";
        if (!Directory.Exists(backupDir))
            Directory.CreateDirectory(backupDir);

        ScriptableObject[] scriptables = Resources.LoadAll<ScriptableObject>("");

        int count = 0;
        foreach (var so in scriptables)
        {
            string path = AssetDatabase.GetAssetPath(so);
            if (string.IsNullOrEmpty(path))
                continue;

            string fileName = Path.GetFileName(path);
            string dest = Path.Combine(backupDir, fileName);

            File.Copy(path, dest, true);
            Debug.Log($"[Backup] {path} → {dest}");
            count++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"Backup complete! Saved {count} ScriptableObjects from Resources/");
    }
}
