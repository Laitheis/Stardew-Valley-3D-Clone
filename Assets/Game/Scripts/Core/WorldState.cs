public class WorldState : GameStateBase
{
    public override void EnterState()
    {
        base.UnpausePhisycs();
        base.UnpauseTime();
        base.ShowMainUIElements();
        base.ShowStatusPanel();
        base.EnablePlayerMovement();
        base.ShowPlayerInv();
    }

    public override void ExitState()
    {
    }
}
