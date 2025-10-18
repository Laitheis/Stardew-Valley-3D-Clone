using UnityEngine;

public class StatusInfoController : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _charName;
    [SerializeField] private TMPro.TextMeshProUGUI _farmName;
    [SerializeField] private TMPro.TextMeshProUGUI _daysLived;

    public void SetStatus()
    {
        _charName.text = $"Character: {PlayerData.playerName}";
        _farmName.text = $"Farm: {PlayerData.farmName}";
        _daysLived.text = $"Total days: {GameTimeHandler.instance.totalDays.ToString()}";
    }
}

