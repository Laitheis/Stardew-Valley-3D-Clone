public class TradeState : GameStateBase
{
    public override void EnterState()
    {
        base.PauseTime();

        _tradingHandler.OpenTrade();
        _tradingHandler.TradeWindow.gameObject.SetActive(true);

        base.HideMainUIElements();
        base.DisablePlayerMovement();
    }

    public override void ExitState()
    {
        _tradingHandler.TradeWindow.gameObject.SetActive(false);
    }
}
