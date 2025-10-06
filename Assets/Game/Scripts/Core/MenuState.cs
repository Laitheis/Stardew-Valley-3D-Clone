public class MenuState : GameStateBase
{
    public override void EnterState()
    {
        base.PauseTime();
        base.DisablePlayerMovement();
        base.HideMainUIElements();
        base.HidePlayerInv();
    }

    public override void ExitState()
    {

    }
}
