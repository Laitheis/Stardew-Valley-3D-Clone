public class PendingState : GameStateBase
{
    public override void EnterState()
    {
        _inputHandler.gameObject.SetActive(false);
    }

    public override void ExitState()
    {
        _inputHandler.gameObject.SetActive(true);
    }
}
