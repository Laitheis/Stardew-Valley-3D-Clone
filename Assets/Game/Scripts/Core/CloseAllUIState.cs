using InventorySystem;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Core
{
    public class CloseAllUIState : GameStateBase
    {
        public override void EnterState()
        {
            base.PauseTime();
            base.DisablePlayerMovement();
            base.HideMainUIElements();
            base.HidePlayerInv();
            base.HideUITooltip();
        }

        public override void ExitState()
        {
            
        }
    }
}