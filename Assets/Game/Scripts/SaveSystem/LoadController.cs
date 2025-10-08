using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadController : MonoBehaviour
{
    public int currentSaveSlot;
    [SerializeField] private GameObject _savePanel;
    [SerializeField] private Transform _saveListContent;
    [SerializeField] private GameObject _confirmDestroySavePanel;
    private List<SaveFileDataList> _saves;
    private Guid _currentGuidToDelete;
    private GameObject _currentPanelToDelete;
    private string _saveDir;

    private void Start()
    {
        _saveDir = Path.Combine(Application.persistentDataPath, "Saves");
        if (!Directory.Exists(_saveDir))
        {
            Directory.CreateDirectory(_saveDir);
        }
        _saves = SaveService.instance.GetAllSavesList();
        foreach (var save in _saves)
        {
            var panel = Instantiate(_savePanel, _saveListContent);
            var refs = panel.GetComponent<SavePanelHolder>();
            refs.farmName.text = "Farm name: " + save.farmName;
            refs.playerName.text = "Player name: " + save.playerName;
            refs.daysLived.text = "Days lived: " + save.currentDay;
            refs.saveGuid = Guid.Parse(save.farmGuid);
        }
    }

    public void Load()
    {
        SaveDataHolder.instance.saveGuid = _saveListContent.GetChild(currentSaveSlot).GetComponent<SavePanelHolder>().saveGuid;
        SaveDataHolder.instance.isFirstLaunch = false;
        SceneManager.LoadScene("Gameplay");
    }

    public void ShowDestroySaveConfirm(Guid saveGuid, GameObject panelToDelete)
    {
        _confirmDestroySavePanel.SetActive(true);
        _currentGuidToDelete = saveGuid;
        _currentPanelToDelete = panelToDelete;
    }

    public void ApplyToDeleteSave()
    {
        Destroy(_currentPanelToDelete);
        DeleteSaveByGuid();
    }

    public bool DeleteSaveByGuid()
    {
        _confirmDestroySavePanel.SetActive(false);
        try
        {
            string[] files = Directory.GetFiles(_saveDir, "*.json");

            foreach (string file in files)
            {
                string json = File.ReadAllText(file);

                var data = JsonConvert.DeserializeObject<SaveFileDataList>(json);
                if (data == null || string.IsNullOrEmpty(data.farmGuid))
                    continue;

                if (Guid.TryParse(data.farmGuid, out Guid parsedGuid) && parsedGuid == _currentGuidToDelete)
                {
                    File.Delete(file);
                    Debug.Log($"Save file deleted: {Path.GetFileName(file)}");
                    return true;
                }
            }

            Debug.LogWarning($"Save with GUID {_currentGuidToDelete} not found.");
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error deleting save: {ex.Message}");
            return false;
        }
    }

    public void CloseConfirmDeleteSavePanel()
    {
        _confirmDestroySavePanel.SetActive(false);
    }
}

