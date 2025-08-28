using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DropItemToWorld : Zenject.IInitializable
{
    [Inject] private readonly SignalBus _signalBus;
    private readonly DiContainer _container;
    private readonly GameObject _itemPrefab;

    public DropItemToWorld(DiContainer container, GameObject itemPrefab)
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
        ItemDefinition itemDefinition = signal.Item.ItemDefinition;

        List<ItemInstance> itemInstances = new List<ItemInstance>();
        for (int i = 0; i < signal.Item.Count; i++)
        {
            ItemInstance _itemInstance = new ItemInstance(itemDefinition);

            _itemInstance.Properties = signal.Item.Properties;
            _itemInstance.SetCount(1);

            itemInstances.Add(_itemInstance);
        }

        foreach (var item in itemInstances)
        {
            var itemGO = _container.InstantiatePrefab(_itemPrefab, signal.Position, Quaternion.identity, null);

            var itemInstance = itemGO.GetComponent<ItemInstanceHolder>();

            itemInstance.Item = item;
        }

        Debug.Log($"Loot named {signal.Item} drops at position {signal.Position} with quantity {signal.Item.Count}");
    }

}

