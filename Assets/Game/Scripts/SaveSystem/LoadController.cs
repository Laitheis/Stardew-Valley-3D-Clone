using System.Collections.Generic;
using UnityEngine;


public class LoadController : MonoBehaviour
{
    public int currentSaveSlot;
    [SerializeField] private GameObject _savePanel;
    [SerializeField] private Transform _saveListContent;
    private List<SaveFileDataList> _saves;

    private void Start()
    {
        _saves = SaveService.instance.GetAllSavesList();
        foreach (var save in _saves)
        {
            var panel = Instantiate(_savePanel, _saveListContent);
            var refs =  panel.GetComponent<SavePanelHolder>();
            refs.farmName.text = "Farm name: " + save.farmName;
            refs.playerName.text = "Player name: " + save.playerName;
            refs.daysLived.text = "Days lived: " + save.currentDay;
            refs.saveGuid = save.farmGuid;
        }
    }

    public void Load()
    {
        SaveGuidHolder.saveGiud = _saveListContent.GetChild(currentSaveSlot).GetComponent<SavePanelHolder>().saveGuid;
    }
}
