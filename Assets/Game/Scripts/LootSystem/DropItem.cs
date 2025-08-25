using UnityEngine;
using Zenject;

public class DropItem : Zenject.IInitializable
{
    [Inject] private SignalBus _signalBus;
    private DiContainer _container;
    private GameObject _itemPrefab;

    public DropItem(DiContainer container, GameObject itemPrefab)
    {
        _container = container;
        _itemPrefab = itemPrefab;
    }


    public void Initialize()
    {
        _signalBus.Subscribe<ItemDropSignal>(OnItemDrop);
    }

    private void OnItemDrop(ItemDropSignal signal)
    {
        var itemGO = _container.InstantiatePrefab(_itemPrefab, signal.Position, Quaternion.identity, null);

        var itemWorldObject = itemGO.GetComponent<ItemWorldObject>();

        //TODO сделать выбрасывание предметов
        //ItemInstance newItem = new ItemInstance(signal.ItemDefinition, signal.Count);
        //itemWorldObject.Constructor(new ItemInstance );

        foreach (var item in signal.Items)
        {
            Debug.Log($"Выпадает лут под названием {item.Name} на позиции {signal.Position}  в размере {signal.Count}");
        }
    }

}

