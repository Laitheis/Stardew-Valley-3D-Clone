using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputHandler : MonoBehaviour
{
    // Called from Unity Event on object PlayerInput
    public void OnExit(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        if (GameStateService.instance.CurrentState is TradeState)
        {
            GameStateService.instance.SetState(GameStateService.GameState.World);
            return;
        }

        if(GameStateService.instance.CurrentState is WorldState)
        {
            GameStateService.instance.SetState(GameStateService.GameState.Menu);
            return;
        }

        if (GameStateService.instance.CurrentState is MenuState)
        {
            GameStateService.instance.SetState(GameStateService.GameState.World);
            return;
        }

        if (GameStateService.instance.CurrentState is CloseAllUIState)
        {
            GameStateService.instance.SetState(GameStateService.GameState.World);
            return;
        }

        if (GameStateService.instance.CurrentState is PlayerInterfaceState)
        {
            GameStateService.instance.SetState(GameStateService.GameState.World);
            return;
        }
    }

    // Called from Unity Event on object PlayerInput
    public void OnOpenInterface(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        if (GameStateService.instance.CurrentState is WorldState)
        {
            GameStateService.instance.SetState(GameStateService.GameState.Interface);
            return;
        }

        if (GameStateService.instance.CurrentState is PlayerInterfaceState)
        {
            GameStateService.instance.SetState(GameStateService.GameState.World);
            return;
        }
    }
}
