using System;
using UnityEngine;
using Zenject;

namespace Core
{
    public class GameStateHandler : MonoBehaviour
    {
        [Inject] SignalBus _sb;

        public enum GameState { World, Menu, Trade, Mine, Dialogue, Cutscene }

        [SerializeField] private TradeState _tradeState;
        [SerializeField] private WorldState _worldState;
        [SerializeField] private MenuState _menuState;

        private GameStateBase _currentState;

        public static GameStateHandler instance;

        public GameStateBase CurrentState { get => _currentState; set => _currentState = value; }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
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
                    newState = _worldState;
                    break;
                case GameState.Menu:
                    newState = _menuState;
                    break;
                case GameState.Trade:
                    newState = _tradeState;
                    break;
                case GameState.Mine:
                    break;
                case GameState.Dialogue:
                    break;
                case GameState.Cutscene:
                    break;
            }

            _currentState?.ExitState();
            _currentState = newState;
            _currentState.EnterState();
        }

        public void ToState(GameState stateNum)
        {
            SetState(GameState.World);
        }

        public void ToState(string stateName)
        {
            if (Enum.TryParse(typeof(GameState), stateName, true, out object state))
            {
                GameState stateType = (GameState)state;
                SetState(stateType);
            }
        }
    }
}

