using UnityEngine;
using Zenject;

public class BuyHandler : MonoBehaviour
{
    [Inject] DefinitionDatabase _database;
    [Inject] UIDragController _dragController;
    [Inject] SignalBus _signalBus;

    private bool _isStacked;

    void Start()
    {
        _signalBus.Subscribe<CurrencyEventArgs>(OnCurrencyEvent);
    }

    void OnCurrencyEvent(CurrencyEventArgs e)
    {
        if (!e.purchaseSuccess)
            return;

        ItemDefinition def = _database.itemDefinitions.Find(i => i.Name == e.purchasedItem);
        if (def == null)
            return;

        if (_dragController.IsDragging)
        {
            if (def == _dragController.ItemInstance.ItemDefinition)
            {
                _dragController.ItemInstance.Add(1, out int overflow);
                return;
            }

            // TODO: floating notif
            return;
        }

        // If don't drag - crate new ItemInstance
        ItemInstance itemInstance = new ItemInstance(def, e.purchasedCount);
        _dragController.SetDraggedItem(itemInstance);
        _dragController.SetDraggedSprite(def.Sprite);
        _dragController.IsDragging = true;
    }
}
