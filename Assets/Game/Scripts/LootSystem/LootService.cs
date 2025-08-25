using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class LootService //: Zenject.IInitializable, IDisposable
{
    //private readonly SignalBus _signalBus;

    //public LootService(SignalBus signalBus)
    //{
    //    _signalBus = signalBus;
    //}

    //public void Initialize()
    //{
    //    _signalBus.Subscribe<ItemDropSignal>(OnLootDrop);
    //}

    //public void Dispose()
    //{
    //    _signalBus.Unsubscribe<ItemDropSignal>(OnLootDrop);
    //}

    //private void OnLootDrop(ItemDropSignal signal)
    //{
        
    //}
}



public class ItemDropSignal
{
    public Vector3 Position;

    public List<ItemDefinition> Items;

    public int Count;

    public ItemDropSignal(Vector3 position, List<ItemDefinition> items, int count)
    {
        Position = position;
        Items = items;
        Count = count;
    }
}

