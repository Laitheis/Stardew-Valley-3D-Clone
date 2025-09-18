using UnityEngine;
using Core;
using Zenject;

public class TradeTrigger : MonoBehaviour
{
    [SerializeField] private string _traderName;
    [Inject] private TradingHandler _tradingHandler;

    private void OnMouseDown()
    {
        _tradingHandler.SetCurrentTrader(_traderName);
        GameStateHandler.Instance.ChangeState(GameStateHandler.GameState.Trade);
    }
}