using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DropItemToWorld : Zenject.IInitializable
{
    [Inject] private SignalBus _signalBus;

    private DiContainer _container;
    private GameObject _itemPrefab;

    private Vector3 _defaultSpawnOffset = new Vector3(1, 1, 1);

    public DropItemToWorld(DiContainer container, [Inject(Id = "WorldItem")]GameObject itemPrefab)
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

        Vector3 _spawnOffset = _defaultSpawnOffset;
        foreach (var item in itemInstances)
        {
            var itemGO = _container.InstantiatePrefab(
                _itemPrefab,
                signal.Position + _spawnOffset,
                Quaternion.identity,
                null);

            _spawnOffset += new Vector3(0, 1, 0);

            itemGO.GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);

            var itemInstance = itemGO.GetComponent<ItemInstanceHolder>();

            itemInstance.Item = item;
        }

        Debug.Log($"Loot named {signal.Item} drops at position {signal.Position} with quantity {signal.Item.Count}");
    }
}

