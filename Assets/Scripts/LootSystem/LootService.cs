using System;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class LootService : Zenject.IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;

    public LootService(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<ItemDropSignal>(OnLootDrop);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<ItemDropSignal>(OnLootDrop);
    }

    private void OnLootDrop(ItemDropSignal signal)
    {
        Debug.Log($"Выпадает лут из таблицы {signal.LootTableId} на позиции {signal.Position}");

        // Тут логика генерации:
        // 1. выбираем предметы из таблицы (по id)
        // 2. спавним префабы через Object.Instantiate
        // 3. запускаем анимацию "выпадения"
    }
}



public class ItemDropSignal
{
    public Vector3 Position;
    //TODO
    public int LootTableId;

    public ItemDropSignal(Vector3 position, int lootTableId)
    {
        Position = position;
        LootTableId = lootTableId;
    }
}

