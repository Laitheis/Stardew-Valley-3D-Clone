using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputHandler : MonoBehaviour
{
    //private Controls _controls;
    //public InputAction escAction;

    //public PlayerInput playerInput;

    //private void Awake()
    //{
    //    _controls = new Controls();
    //}

    //private void OnEnable()
    //{
    //    _controls.UI.Enable();
    //    escAction.Enable();
    //}

    //private void OnDisable()
    //{
    //    _controls.UI.Disable();
    //    escAction.Disable();

    //}
    private void Update()
    {

        //var action = playerInput.actions["OpenMenu"];
        //if (action != null && action.triggered)
        //{
        //    Debug.Log("OpenMenu сработал!");
        //}
    }
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
    }
}
