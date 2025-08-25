using System;
using UnityEngine;
using Zenject;

public class DropItem : Zenject.IInitializable
{
    private readonly SignalBus _signalBus;

    public DropItem(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<ItemDropSignal>(OnItemDrop);
    }

    private void OnItemDrop(ItemDropSignal signal)
    {
        Debug.Log($"Выбрасываем предмет {signal.LootTableId} на позиции {signal.Position}");
    }

}

