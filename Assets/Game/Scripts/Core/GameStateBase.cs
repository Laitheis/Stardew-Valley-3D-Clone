using InventorySystem;
using UnityEngine;
using Zenject;
using static UnityEngine.Rendering.DebugUI;

public abstract class GameStateBase : MonoBehaviour
{
    [Inject] protected TraderHandler _tradingHandler;
    [Inject] protected HintVisualizer _hintVisualizer;
    [Inject] protected PlayerToolHandler _toolHandler;
    [Inject] protected SelectedSlotHandler _slotHandler;
    [Inject(Id = "Dimming")] protected GameObject _dimmingScreen;
    [Inject] protected PlayerController _playerController;
    [Inject] protected UIDragController _dragController;
    [Inject(Id = "PlayerInv")] protected InventoryHandler _playerInv;
    [Inject(Id = "StatusPanel")] protected GameObject _statusPanel;

    public abstract void EnterState();
    public abstract void ExitState();

    public void HideMainUIElements()
    {
        SetMainUIElements(false);
    }

    public void ShowMainUIElements()
    {
        SetMainUIElements(true);
    }

    public void SetMainUIElements(bool value)
    {
        _toolHandler.enabled = value;
        _slotHandler.SelectionFrame.gameObject.SetActive(value);
        _dimmingScreen.SetActive(!value);
        if (!value)
            _hintVisualizer.Hide();
        _hintVisualizer.gameObject.SetActive(value);
        _statusPanel.SetActive(value);
    }

    public void ShowPlayerInv()
    {
        _playerInv.gameObject.SetActive(true);
    }

    public void HidePlayerInv()
    {
        _playerInv.gameObject.SetActive(false);
    }

    public void DisablePlayerMovement()
    {
        SetPlayerMovement(false);
    }

    public void EnablePlayerMovement()
    {
        SetPlayerMovement(true);
    }

    public void SetPlayerMovement(bool value)
    {
        _playerController.enabled = value;
    }

    public void PauseTime()
    {
        GameTimeManager.instance.pauseTime = true;
    }

    public void UnpauseTime()
    {
        GameTimeManager.instance.pauseTime = false;
    }

    public void PausePhisycs()
    {
        Time.timeScale = 0;
    }

    public void UnpausePhisycs()
    {
        Time.timeScale = 1;
    }
}

