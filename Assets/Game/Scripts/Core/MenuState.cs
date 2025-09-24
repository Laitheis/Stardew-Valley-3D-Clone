using InventorySystem;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Core
{
    public class MenuState : GameStateBase
    {
        public override void EnterState()
        {
            GameTimeManager.Instance.pauseTime = true;
            Time.timeScale = 0;

            _toolHandler.enabled = false;

            _playerInv.gameObject.SetActive(false);

            _slotHandler.SelectionFrame.gameObject.SetActive(false);

            _playerController.enabled = false;

            _dimmingScreen.SetActive(true);

            _hintVisualizer.Hide();
            _hintVisualizer.gameObject.SetActive(false);
        }

        public override void ExitState()
        {
            Time.timeScale = 1;
            _dimmingScreen.SetActive(false);
        }
    }
}