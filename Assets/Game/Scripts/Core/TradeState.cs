using InventorySystem;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Core
{
    public class TradeState : GameStateBase
    {
        public override void EnterState()
        {
            GameTimeManager.Instance.pauseTime = true;
            _tradingHandler.OpenTrade();
            _tradingHandler.TradeWindow.gameObject.SetActive(true);

            _toolHandler.enabled = false;

            _slotHandler.SelectionFrame.gameObject.SetActive(false);

            _playerController.enabled = false;

            _dimmingScreen.SetActive(true);

            _hintVisualizer.Hide();
            _hintVisualizer.gameObject.SetActive(false);
        }

        public override void ExitState()
        {
            _tradingHandler.TradeWindow.gameObject.SetActive(false);
            _tradingHandler.Close();

            _dimmingScreen.SetActive(false);

            _dragController.IsMouseOverTraderPanel = false;
        }
    }
}