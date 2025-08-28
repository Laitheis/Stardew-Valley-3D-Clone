using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class TreeBase : MonoBehaviour, IHarvestable, IDestructible, IStats
{
    [Inject] private SignalBus _signalBus;

    protected LootGenerator _lootGenerator;
    protected StatContainter _statContainer;
    protected List<ItemInstance> _pendingLoot;

    protected Animator _animator;

    public StatContainter StatContainer => _statContainer;

    [Inject]
    public void Construct(LootGenerator lootGenerator)
    {
        _lootGenerator = lootGenerator;

        _animator = GetComponent<Animator>();

        InitializeStats();
        InitializeLoot();
    }

    public virtual void InitializeStats()
    {
        _statContainer = new StatContainter();

        var durability = _statContainer.Add(StatTypes.Durability, 100);

        durability.OnMinValueReached += OnHarvest;
        durability.OnMinValueReached += OnDestroyed;
    }

    public virtual void InitializeLoot()
    {
        _pendingLoot = _lootGenerator.GenerateLoot("Oak", 0);
    }

    public virtual void OnHarvest()
    {
        if (_pendingLoot == null) return;

        foreach (var item in _pendingLoot)
        {
            _signalBus.Fire(new ItemDropSignal(transform.position, item));
        }
    }

    public virtual void TakeDamage(int amount)
    {
        Debug.Log($"{gameObject} has {amount} damage.");

        _statContainer.GetStat(StatTypes.Durability).Value -= amount;

        _animator.SetTrigger("Cut");

        Debug.Log($"new {gameObject} durability is {_statContainer.GetStat(StatTypes.Durability).Value}");
    }

    public virtual void OnDestroyed()
    {
        Debug.Log($"{gameObject} has been destroyed");

        //Destroy(gameObject);
    }
}
