using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInput : MonoBehaviour
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
        if (GameStateHandler.instance.CurrentState is TradeState)
        {
            GameStateHandler.instance.SetState(GameStateHandler.GameState.World);
            return;
        }

        if(GameStateHandler.instance.CurrentState is WorldState)
        {
            GameStateHandler.instance.SetState(GameStateHandler.GameState.Menu);
            return;
        }

        if (GameStateHandler.instance.CurrentState is MenuState)
        {
            GameStateHandler.instance.SetState(GameStateHandler.GameState.World);
            return;
        }
    }
}
