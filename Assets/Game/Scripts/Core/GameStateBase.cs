using InventorySystem;
using UnityEngine;
using Zenject;
using static UnityEngine.Rendering.DebugUI;

public abstract class GameStateBase : MonoBehaviour
{
    [Inject] protected TraderHandler _tradingHandler;
    [Inject] protected HintVisualizer _hintVisualizer;
    [Inject] protected PlayerToolController _toolHandler;
    [Inject] protected SelectedSlotController _slotHandler;
    [Inject(Id = "Dimming")] protected GameObject _dimmingScreen;
    [Inject] protected PlayerController _playerController;
    [Inject] protected UIDragController _dragController;
    [Inject(Id = "PlayerInv")] protected InventoryHandler _playerInv;
    [Inject(Id = "StatusPanel")] protected GameObject _statusPanel;
    [Inject] protected TooltipView _tooltipView;
    [Inject] protected UIInputHandler _inputHandler;
    [Inject(Id = "MenuPanel")] protected GameObject _menuPanel;
    [Inject(Id = "InterfacePanel")] protected GameObject _interfacePanel;
    [Inject] protected StatusInfoController _statusInfo;
 
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
    }

    public void HideUITooltip()
    {
        _tooltipView.CloseTooltip();
    }

    public void HideStatusPanel()
    {
        SetHideStatusPanel(false);
    }

    public void ShowStatusPanel()
    {
        SetHideStatusPanel(true);
    }

    public void SetHideStatusPanel(bool value)
    {
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
        GameTimeHandler.instance.pauseTime = true;
    }

    public void UnpauseTime()
    {
        GameTimeHandler.instance.pauseTime = false;
    }

    public void PausePhisycs()
    {
        Time.timeScale = 0;
    }

    public void UnpausePhisycs()
    {
        Time.timeScale = 1;
    }

    public void DisableMenuPanel()
    {
        _menuPanel.SetActive(false);
    }

    public void EnableMenuPanel()
    {
        _menuPanel.SetActive(true);
    }

    public void DisableInterfacePanel()
    {
        _interfacePanel.SetActive(false);
    }

    public void EnableInterfacePanel()
    {
        _interfacePanel.SetActive(true);
    }
}

