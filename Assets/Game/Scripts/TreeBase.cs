using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class TreeBase : MonoBehaviour, IHarvestable, IDestructible, IStats
{
    [Inject] private SignalBus _signalBus;
    protected LootTable _loot;

    protected StatContainter _statContainer;

    public StatContainter StatContainer => _statContainer;

    [Inject]
    public void Construct(LootTable loot)
    {
        _loot = loot;
        InitializeStats();
        InitializeLoot();
    }

    public virtual void InitializeStats()
    {
        var stats = new List<Stat>();

        var durability = new Stat(StatTypes.Durability, 0);
        durability.OnMinValueReached += OnHarvest;
        durability.OnMinValueReached += OnDestroyed;

        stats.Add(durability);

        _statContainer = new StatContainter(stats);
    }

    public virtual void InitializeLoot()
    {
        
    }

    //HACK
    public List<ItemDefinition> DropLoot()
    {
        //HACK 
        EntityLoot entityLoot = _loot.FindByName("Oak");
        LootByGrade lootByGrade = entityLoot.GetByGrade(0);

        //TODO инкапсулировать
        List<ItemDefinition> droppedItems = new List<ItemDefinition>();

        foreach (var item in lootByGrade.Loot)
        {
            float roll = UnityEngine.Random.Range(0f, 100f);
            if (roll <= item.Chance)
            {
                droppedItems.Add(item.Item);
                Debug.Log($"Выпал предмет: {item.Item.name} (шанс {item.Chance}%)");
                return droppedItems;
            }
        }

        return null;
    }

    public virtual void OnHarvest()
    {
        int lootTableId = 1; // заглушка
        _signalBus.Fire(new ItemDropSignal(new Vector3(1, 2, 3), DropLoot(), lootTableId));
    }

    public virtual void TakeDamage(int amount)
    {
        Debug.Log($"{gameObject} has {amount} damage.");
        _statContainer.GetStat(StatTypes.Durability).Value -= amount;
        Debug.Log($"new {gameObject} durability is {_statContainer.GetStat(StatTypes.Durability).Value}");
    }

    public virtual void OnDestroyed()
    {
        Debug.Log($"{gameObject} has been destroyed");
    }
}
