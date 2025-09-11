using Inventory.UI;
using InventorySystem;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class SlotTooltipNotifier : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemTooltipView _tooltipView;

    [Inject]
    public void Constructor(ItemTooltipView tooltipView)
    {
        _tooltipView = tooltipView;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ItemInstance itemInstance = transform.parent.GetComponent<ItemSlot>().ItemInstance;
        _tooltipView.ShowTooltip(itemInstance);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltipView.CloseTooltip();
    }
}