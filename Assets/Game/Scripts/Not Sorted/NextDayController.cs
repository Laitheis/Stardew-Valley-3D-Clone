using Core;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using Zenject;
using UnityEngine.UI;


public class NextDayController : MonoBehaviour
{
    [SerializeField] private GameObject _skillUpgradeNotificationPanel;

    [SerializeField] private GameObject _confirmPanel;
    [SerializeField] private GameObject _darkScreen;

    [Inject] private Canvas _mainCanvas;
    [Inject] private DebrisGeneratorController _debrisGenerator;

    private GameObject darkScreen;

    private void Start()
    {
        GameStateService.instance.OnChange += Back;
    }

    private void OnMouseDown()
    {
        GameStateService.instance.SetState(GameStateService.GameState.CloseAllUI);
        _confirmPanel.SetActive(true);
        ScaleAnimator.PlayScale(_confirmPanel.transform);
    }

    public void Back()
    {
        if (_confirmPanel.activeSelf)
        {
            GameStateService.instance.SetState(GameStateService.GameState.World);
            _confirmPanel.SetActive(false);
        }
    }

    public void Apply()
    {
        _confirmPanel.SetActive(false);

        darkScreen = Instantiate(_darkScreen, _mainCanvas.transform);
        var blackout = darkScreen.GetComponent<CanvasGroup>();
        DG.Tweening.Sequence seq = DOTween.Sequence();
        seq = DOTween.Sequence();

        seq.AppendCallback(() => {
            GameStateService.instance.SetState(GameStateService.GameState.Pending);
        });
        seq.Append(blackout.DOFade(1f, 2f));
        seq.AppendCallback(() => { blackout.GetComponent<Image>().raycastTarget = true; });

        seq.AppendCallback(() => {  
            GameTimeHandler.instance.AdvanceDay();
            GameTimeHandler.instance.currentHour = 6;
            GameTimeHandler.instance.currentMinute = 0;
        });
        seq.AppendCallback(() => _debrisGenerator.GenerateDebris(8f));

        seq.AppendCallback(() => NotificationService.DisplayNotification(NotificationService.NotificationColor.Green, "The game is saved..."));
        seq.AppendCallback(() => SaveService.instance.Save());

        if (SkillsManager.instance.isSkillUpgraded)
        {
            SkillsManager.instance.needSleepNotification.SetActive(false);
            SkillsManager.instance.isSkillUpgraded = false;

            _skillUpgradeNotificationPanel.transform.localScale = new Vector3(0, 0, 0);
            _skillUpgradeNotificationPanel.gameObject.SetActive(true);
            _skillUpgradeNotificationPanel.gameObject.transform.SetAsLastSibling();
            seq.Append(_skillUpgradeNotificationPanel.transform.DOScale(Vector3.one, 1.1f).SetEase(Ease.OutBack));
            seq.AppendInterval(2f);
            return;
        }

        seq.AppendInterval(3f);
        seq.Append(_skillUpgradeNotificationPanel.transform.DOScale(Vector3.zero, 1.1f).SetEase(Ease.OutBack));
        seq.AppendCallback(() => _skillUpgradeNotificationPanel.gameObject.SetActive(false));

        seq.AppendCallback(() => { blackout.GetComponent<Image>().raycastTarget = false; });
        seq.AppendCallback(() => {
            GameStateService.instance.SetState(GameStateService.GameState.World);
        });
        seq.Append(blackout.DOFade(0f, 2f));
        seq.AppendCallback(() => {
            Destroy(darkScreen);
        });
    }


    public void FinishAnimation()
    {
        var blackout = darkScreen.GetComponent<CanvasGroup>();

        DG.Tweening.Sequence seq = DOTween.Sequence();

        seq.Append(_skillUpgradeNotificationPanel.transform.DOScale(Vector3.zero, 1.1f).SetEase(Ease.OutBack));
        seq.AppendCallback(() => _skillUpgradeNotificationPanel.gameObject.SetActive(false));

        seq.AppendCallback(() => { blackout.GetComponent<Image>().raycastTarget = false; });
        seq.AppendCallback(() => {
            GameStateService.instance.SetState(GameStateService.GameState.World);
        });
        seq.AppendInterval(1.5f);
        seq.Append(blackout.DOFade(0f, 2f));
        seq.AppendCallback(() => {
            Destroy(darkScreen);
        });
    }
}
