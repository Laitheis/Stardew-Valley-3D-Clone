using InventorySystem;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Core
{
    public class WorldState : GameStateBase
    {
        public override void EnterState()
        {
            Time.timeScale = 1;
            GameTimeManager.Instance.pauseTime = false;
            _toolHandler.enabled = true;

            _slotHandler.SelectionFrame.gameObject.SetActive(true);

            _playerController.enabled = true;
            
            _hintVisualizer.gameObject.SetActive(true);
        }

        public override void ExitState()
        {

        }
    }
}