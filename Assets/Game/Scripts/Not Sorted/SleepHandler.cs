using Core;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using Zenject;


public class SleepHandler : MonoBehaviour
{
    [SerializeField] private GameObject _confirmPanel;
    [SerializeField] private GameObject _darkScreen;

    [Inject] private Canvas _mainCanvas;

    private void Start()
    {
        GameStateHandler.instance.OnChange += Back;
    }

    private void OnMouseDown()
    {
        GameStateHandler.instance.SetState(GameStateHandler.GameState.CloseAllUI);
        _confirmPanel.SetActive(true);
        ScaleAnimator.PlayScale(_confirmPanel.transform);
    }

    public void Back()
    {
        if (_confirmPanel.activeSelf)
        {
            GameStateHandler.instance.SetState(GameStateHandler.GameState.World);
            _confirmPanel.SetActive(false);
        }
    }

    public void Apply()
    {
        GameTimeManager.instance.AdvanceDay();
        GameTimeManager.instance.currentHour = 6;
        GameTimeManager.instance.currentMinute = 0;
        _confirmPanel.SetActive(false);
        var darkScreen = Instantiate(_darkScreen, _mainCanvas.transform);
        var cg = darkScreen.GetComponent<CanvasGroup>();
        DG.Tweening.Sequence seq = DOTween.Sequence();
        seq.Append(cg.DOFade(1f, 1f));
        seq.AppendCallback(() => {
            GameStateHandler.instance.SetState(GameStateHandler.GameState.World);
        });
        seq.Append(cg.DOFade(0f, 1f));
        seq.AppendCallback(() => { 
            Destroy(darkScreen);
        });
    }

}
