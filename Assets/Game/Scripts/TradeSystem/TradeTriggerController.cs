using UnityEngine;
using Core;
using Zenject;

public class TradeTriggerController : MonoBehaviour, IClickConsumer
{
    [SerializeField] private string _traderName;
    [Inject] private TraderHandler _tradingHandler;
    [Inject] private InputHandler _inputHandler;

    public int ClickPriority => 100;

    void OnEnable() => _inputHandler.RegisterConsumer(this);
    void OnDisable() => _inputHandler.UnregisterConsumer(this);

    public bool OnClick()
    {
        bool isMouseOver = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            isMouseOver = hit.collider.gameObject == gameObject;
        }
    
        if (isMouseOver && GameStateService.instance.CurrentState is WorldState)
        {
            _tradingHandler.SetCurrentTrader(_traderName);
            GameStateService.instance.SetState(GameStateService.GameState.Trade);
            return true;
        }
        return false;
    }

    public bool OnRightClick()
    {
        return false;
    }

    public void OnEndClick()
    {
        //
    }

    public bool OnHold()
    {
        return false;
    }
}