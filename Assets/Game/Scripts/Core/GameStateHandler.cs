using System;
using UnityEngine;

namespace Core
{
    public class GameStateHandler : MonoBehaviour
    {
        public enum GameState { World, Pause, Trade, Mine, Dialogue, Cutscene }

        [SerializeField] private TradeState _tradeState;
        [SerializeField] private WorldState _worldState;

        private GameState _currentState;

        public static GameStateHandler Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        private void Start()
        {
            _currentState = GameState.World;
            EnterState(_currentState);
        }

        private void Update()
        {
            LifeCycle();
        }

        private void EnterState(GameState state)
        {
            switch (state)
            {
                case GameState.World:
                    _worldState.EnterState();
                    break;
                case GameState.Pause:
                    break;
                case GameState.Trade:
                    _tradeState.EnterState();
                    break;
                case GameState.Mine:
                    break;
                case GameState.Dialogue:
                    break;
                case GameState.Cutscene:
                    break;
                default:
                    break;
            }
        }

        private void ExitState(GameState state)
        {
            switch (state)
            {
                case GameState.World:
                    _worldState.ExitState();
                    break;
                case GameState.Pause:
                    break;
                case GameState.Trade:
                    _tradeState.ExitState();
                    break;
                case GameState.Mine:
                    break;
                case GameState.Dialogue:
                    break;
                case GameState.Cutscene:
                    break;
                default:
                    break;
            }
        }

        public void ChangeState(GameState newState)
        {
            if (newState == _currentState) return;
            ExitState(_currentState);
            _currentState = newState;
            EnterState(_currentState);
        }

        private void LifeCycle()
        {
            switch (_currentState)
            {
                case GameState.World:
                    break;
                case GameState.Pause:
                    break;
                case GameState.Trade:
                    break;
                case GameState.Mine:
                    break;
                case GameState.Dialogue:
                    break;
                case GameState.Cutscene:
                    break;
                default:
                    break;
            }
        }

        public void ToWorldSate()
        {
            ChangeState(GameState.World);
        }

        public void ToTradeSate()
        {
            ChangeState(GameState.Trade);
        }
    }
}

