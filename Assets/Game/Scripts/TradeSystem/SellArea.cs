using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class SellArea : MonoBehaviour, IPointerDownHandler
{
    [Inject] private UIDragController _dragController;
    [Inject] private TraderHandler _traderHandler;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_dragController.ItemInstance != null)
        {
            _traderHandler.Sell(_dragController.ItemInstance);
            _dragController.IsDragging = false;
            _dragController.ClearDraggedRect();
        }
    }
}

