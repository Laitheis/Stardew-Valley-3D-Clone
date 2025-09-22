using InventorySystem;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Core
{
    public class TradeState : MonoBehaviour
    {
        [Inject] private TraderHandler _tradingHandler;
        [Inject] private HintVisualizer _hintVisualizer;
        [Inject] private PlayerToolHandler _toolHandler;
        [Inject] private SelectedSlotHandler _slotHandler;
        [Inject(Id = "Dimming")] private GameObject _dimmingScreen;
        [Inject] private PlayerController _playerController; 

        public void EnterState()
        {
            GameTimeManager.Instance.pauseTime = true;
            _tradingHandler.OpenTrade();

            _toolHandler.enabled = false;

            _slotHandler.SelectionFrame.gameObject.SetActive(false);

            _playerController.enabled = false;

            _dimmingScreen.SetActive(true);

            _hintVisualizer.Hide();
            _hintVisualizer.gameObject.SetActive(false);
        }
    }
}