using UnityEngine;
using Core;
using Zenject;

public class TradeTrigger : MonoBehaviour
{
    [SerializeField] private string _traderName;
    [Inject] private TraderHandler _tradingHandler;

    private void OnMouseDown()
    {
        _tradingHandler.SetCurrentTrader(_traderName);
        GameStateService.instance.SetState(GameStateService.GameState.Trade);
    }
}