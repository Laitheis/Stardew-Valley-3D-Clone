using Inventory.UI;
using InventorySystem;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class SlotTooltipNotifier : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Inject] private ItemTooltipView _tooltipView;

    private Vector2 _offset = new Vector2(45, 45);

    public void OnPointerEnter(PointerEventData eventData)
    {
        ItemInstance itemInstance = transform.parent.GetComponent<SlotItemHolder>().ItemInstance;
        _tooltipView.ShowTooltip(itemInstance.ItemDefinition, transform.GetComponent<RectTransform>(), _offset, false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltipView.CloseTooltip();
    }
}