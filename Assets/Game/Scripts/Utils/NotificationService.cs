using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class NotificationService : MonoBehaviour
{
    public enum NotificationColor { Green, Yellow, Red }
    [Inject(Id = "Notif")] private GameObject _notif;
    [Inject] private Canvas _canvas;

    public static NotificationService instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public static void DisplayNotification(NotificationColor color, string text)
    {
        Vector2 startPos = new Vector2(135, -65);
        Vector2 targetPos = startPos + Vector2.up * 150;

        var notif = Instantiate(NotificationService.instance._notif, NotificationService.instance._canvas.transform);
        var cg = notif.GetComponent<CanvasGroup>();
        var rect = notif.GetComponent<RectTransform>();
        var tmp = notif.GetComponent<TMPro.TextMeshProUGUI>();
        notif.transform.SetAsLastSibling();
        rect.anchoredPosition = startPos;
        tmp.text = text;
        switch (color)
        {
            case NotificationColor.Green:
                tmp.color = Color.green;
                break;
            case NotificationColor.Yellow:
                tmp.color = Color.yellow;
                break;
            case NotificationColor.Red:
                tmp.color = Color.red;
                break;
            default:
                break;
        }

        Sequence seq = DOTween.Sequence();
        seq.Append(rect.DOAnchorPos(targetPos, 1f));
        seq.AppendInterval(1f);
        seq.Append(rect.DOAnchorPos(startPos, 1f));
        seq.AppendCallback(() => Destroy(notif));
    }
}
