using UnityEngine;

namespace Core
{
    public class GameStateHandler : MonoBehaviour
    {
        public enum GameState { World, Pause, Trade, Mine, Dialogue, Cutscene }

        private GameState _currentState;

        public void ChangeState(GameState newState)
        {
            if (newState == _currentState) return;

            _currentState = newState;


        }

        public void LifeCycle()
        {

        }
    }
}

