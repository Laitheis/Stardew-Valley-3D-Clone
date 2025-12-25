using Core;
using System;
using System.Linq;
using UnityEngine;
using Zenject;

public class GameStateService : MonoBehaviour
{
    public event Action OnChange;

    [Inject] SignalBus _sb;

    public enum GameState { World, Menu, Trade, Mine, Dialogue, Cutscene, CloseAllUI, Pending, Interface }

    private GameStateBase _currentState;

    public static GameStateService instance;

    public GameStateBase CurrentState { get => _currentState; set => _currentState = value; }

    private GameStateBase[] _gameStates;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        _gameStates = FindObjectsOfType<GameStateBase>(true);
    }

    private void Start()
    {
        SetState(GameState.World);
    }

    public void SetState(GameState stateNum)
    {
        GameStateBase newState = null;
        switch (stateNum)
        {
            case GameState.World:
                newState = _gameStates.OfType<WorldState>().FirstOrDefault();
                break;
            case GameState.Menu:
                newState = _gameStates.OfType<MenuState>().FirstOrDefault();
                break;
            case GameState.Trade:
                newState = _gameStates.OfType<TradeState>().FirstOrDefault();
                break;
            case GameState.Mine:
                break;
            case GameState.Dialogue:
                break;
            case GameState.Cutscene:
                break;
            case GameState.CloseAllUI:
                newState = _gameStates.OfType<CloseAllUIState>().FirstOrDefault();
                break;
            case GameState.Pending:
                newState = _gameStates.OfType<PendingState>().FirstOrDefault();
                break;
            case GameState.Interface:
                newState = _gameStates.OfType<PlayerInterfaceState>().FirstOrDefault();
                break;
        }

        if (newState == CurrentState) return;
        _currentState?.ExitState();
        _currentState = newState;
        _currentState.EnterState();
        OnChange?.Invoke();
    }

    public void SetStateByInt(int state)
    {
        SetState((GameState)state);
    }
}


