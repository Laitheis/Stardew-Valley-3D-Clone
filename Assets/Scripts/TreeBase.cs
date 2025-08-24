using System.Collections.Generic;
using UnityEngine;
using Zenject;


public abstract class TreeBase : MonoBehaviour, IHarvestable, IDestructible, IStats
{
    [Inject] private SignalBus _signalBus;

    private StatContainter _statContainer;

    public TreeBase()
    {
        InitializeStats();
    }

    public StatContainter StatContainer => _statContainer;

    public virtual void InitializeStats()
    {
        List<Stat> stats = new List<Stat>();

        stats.Add(new Stat(StatTypes.Durability, 0));

        _statContainer = new StatContainter(stats);
    }

    public virtual void OnHarvest()
    {
        //HACK
        int lootTableId = 1;
        _signalBus.Fire(new LootDropSignal(transform.position, lootTableId));
    }

    public virtual void TakeDamage(int amount)
    {

    }

    public virtual void OnDestroyed()
    {

    }

}
