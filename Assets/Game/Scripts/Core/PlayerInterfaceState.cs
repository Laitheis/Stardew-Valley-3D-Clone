public class PlayerInterfaceState : GameStateBase
{
    public override void EnterState()
    {
        base.PauseTime();
        base.HidePlayerInv();
        base.HideMainUIElements();
        base.DisablePlayerMovement();

        base.EnableInterfacePanel();
        _statusInfo.SetStatus();
    }

    public override void ExitState()
    {
        base.DisableInterfacePanel();
    }
}
