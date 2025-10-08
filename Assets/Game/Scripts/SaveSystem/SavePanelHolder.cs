using System;
using UnityEngine;

public class SavePanelHolder : MonoBehaviour
{
    public TMPro.TextMeshProUGUI farmName;
    public TMPro.TextMeshProUGUI playerName;
    public TMPro.TextMeshProUGUI daysLived;

    [HideInInspector] public Guid saveGuid;

    private LoadController _loadController;

    private void Start()
    {
        _loadController = FindObjectOfType<LoadController>();
    }

    public void AddSelfToCurrentLoad()
    {
        _loadController.currentSaveSlot = transform.GetSiblingIndex();
    }

    public void SuggestToDestroySelf()
    {
        _loadController.ShowDestroySaveConfirm(saveGuid, gameObject);
    }
}
