using UnityEngine;

public class StatusPanelController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI dataText;
    public TMPro.TextMeshProUGUI seasonText;

    private void Update()
    {
        dataText.text = $"{GameTimeHandler.instance.currentDay} Day, {GameTimeHandler.instance.currentHour:D2}:{GameTimeHandler.instance.currentMinute:D2}";
        seasonText.text = $"{GameTimeHandler.instance.currentSeason}";
    }
}

