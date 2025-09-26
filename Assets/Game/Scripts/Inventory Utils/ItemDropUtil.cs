using InventorySystem;
using UnityEngine;
using Zenject;

public class ItemDropUtil
{
    public static ItemDropUtil Instance;

    private GameObject _player;

    [Inject] SignalBus _signalBus;

    [Inject]
    private void Constructor([Inject(Id = "Player")] GameObject player)
    {
        if (Instance != null)
            return;
        Instance = this;
        _player = player;
    }

    public void AddWithDropToWorld(ItemsCollection itemsCollection, Vector3 impulse, ItemInstance item)
    {
        int overflow = itemsCollection.AddRange(item, 10);

        var overflowItem = new ItemInstance(item.ItemDefinition, overflow);

        _signalBus.Fire(new ItemDropEvent(_player.transform.position, overflowItem, true));
    }
}