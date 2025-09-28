using Inventory.UI;
using InventorySystem;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class TraderSlotTooltipNotifier : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Inject] private ItemTooltipView _tooltipView;
    [Inject] private DefinitionDatabase _definitionDatabase;

    private Vector2 _offset = new Vector2(0, 0);

    private void Awake()
    {
        FindObjectOfType<SceneContext>().Container.Inject(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ItemDefinition itemDef = _definitionDatabase.itemDefinitions.First( i => i.Name == transform.GetComponent<PurchaseButton>().name);
        _tooltipView.ShowTooltip(itemDef, transform.GetComponent<RectTransform>(), _offset, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltipView.CloseTooltip();
    }
}