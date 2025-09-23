using InventorySystem;
using UnityEngine;
using Zenject;

namespace Core
{
    public abstract class GameStateBase : MonoBehaviour
    {
        [Inject] protected TraderHandler _tradingHandler;
        [Inject] protected HintVisualizer _hintVisualizer;
        [Inject] protected PlayerToolHandler _toolHandler;
        [Inject] protected SelectedSlotHandler _slotHandler;
        [Inject(Id = "Dimming")] protected GameObject _dimmingScreen;
        [Inject] protected PlayerController _playerController;
        [Inject] protected UIDragController _dragController;

        public abstract void EnterState();
        public abstract void ExitState();
    }
}
