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
    [SerializeField] private GameObject _confirmPanel;
    [SerializeField] private GameObject _darkScreen;

    [Inject] private Canvas _mainCanvas;
    [Inject] private DebrisGeneratorController _debrisGenerator;

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

        var darkScreen = Instantiate(_darkScreen, _mainCanvas.transform);
        var blackout = darkScreen.GetComponent<CanvasGroup>();
        DG.Tweening.Sequence seq = DOTween.Sequence();

        SaveService.instance.Save();

        seq.AppendCallback(() => {
            GameStateService.instance.SetState(GameStateService.GameState.Pending);
        });
        seq.Append(blackout.DOFade(1f, 2f));
        seq.AppendCallback(() => { blackout.GetComponent<Image>().raycastTarget = true; });

        seq.AppendCallback(() => {  
            GameTimeService.instance.AdvanceDay();
            GameTimeService.instance.currentHour = 6;
            GameTimeService.instance.currentMinute = 0;
        });
        seq.AppendCallback(() => _debrisGenerator.GenerateDebris(8f));

        seq.AppendCallback(() => NotificationService.DisplayNotification(NotificationService.NotificationColor.Green, "The game is saved..."));
        seq.AppendInterval(3f);
        seq.AppendCallback(() => { blackout.GetComponent<Image>().raycastTarget = false; });
        seq.AppendCallback(() => {
            GameStateService.instance.SetState(GameStateService.GameState.World);
        });
        seq.Append(blackout.DOFade(0f, 2f));
        seq.AppendCallback(() => { 
            Destroy(darkScreen);
        });
    }

}
