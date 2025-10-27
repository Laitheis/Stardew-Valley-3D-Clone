public class MenuState : GameStateBase
{
    public override void EnterState()
    {
        base.PauseTime();
        base.DisablePlayerMovement();
        base.HideMainUIElements();
        base.HideStatusPanel();
        base.HidePlayerInv();
        base.HideUITooltip();

        base.EnableMenuPanel();
    }

    public override void ExitState()
    {
        base.DisableMenuPanel();
    }
}
