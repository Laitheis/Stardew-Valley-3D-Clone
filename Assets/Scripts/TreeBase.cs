using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;


public abstract class TreeBase : MonoBehaviour, IHarvestable, IDestructible, IStats
{
    [Inject] private SignalBus _signalBus;

    protected StatContainter _statContainer;

    private void Start()
    {
        InitializeStats();
    }

    public StatContainter StatContainer => _statContainer;

    public virtual void InitializeStats()
    {
        //TODO Инкапсулировать в СтатКонтейнер
        List<Stat> stats = new List<Stat>();

        var durability = new Stat(StatTypes.Durability, 0);

        durability.OnMinValueReached += OnHarvest;
        durability.OnMinValueReached += OnDestroyed;

        stats.Add(durability);

        _statContainer = new StatContainter(stats);
    }

    public virtual void OnHarvest()
    {
        //HACK
        int lootTableId = 1;
        _signalBus.Fire(new ItemDropSignal(new Vector3(1,2,3), lootTableId));
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
