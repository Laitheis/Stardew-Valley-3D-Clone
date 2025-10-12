using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class DestructibleObjectBase : MonoBehaviour, IDestructible, IStats
{
    protected StatContainter _statContainer;
    protected List<ItemInstance> _pendingLoot;

    protected GameObject _smokeEffect;
    protected Animator _animator;

    protected bool _isFalling;

    public ItemType _acceptableTool;
    [HideInInspector] public Vector3Int _gridPos;

    public StatContainter StatContainer => _statContainer;

    protected bool _inited = false;

    [Inject] protected LootGeneratorHandler _lootGenerator;
    [Inject] private SignalBus _signalBus;

    public void Init(Vector3Int gridPos)
    {
        _gridPos = gridPos;

        _animator = GetComponent<Animator>();

        InitializeStats();
        InitializeLoot();

        _inited = true;
    }

    public virtual void InitializeStats()
    {
        if (_inited)
        {
            return;
        }

        _statContainer = new StatContainter();

        var durability = _statContainer.Add(StatTypes.Durability, 100);

        durability.OnMinValueReached += OnDestroyed;
    }

    protected virtual void InitializeLoot()
    {
        if (_inited)
        {
            return;
        }

        // Example _pendingLoot = _lootGenerator.GenerateLoot("Oak", 0);
    }

    public virtual async void OnDestroyed()
    {
        Debug.Log($"{gameObject} has been destroyed");

        UnsubscribeFromDurability();
        StartFalling();

        await PlayDestructionAnimation();

        HarvestAndCleanup();

        FarmManager.instance.farmTiles.TilesCollection[_gridPos].objectOnTile = null;
    }

    public virtual void TakeDamage(int amount, ItemType tool)
    {
        if (_isFalling) return;
        if (tool != _acceptableTool) return;

        Debug.Log($"{gameObject} has {amount} damage.");

        _statContainer.GetStat(StatTypes.Durability).Value -= amount;

        _animator.SetTrigger("Damage");

        Debug.Log($"new {gameObject} durability is {_statContainer.GetStat(StatTypes.Durability).Value}");
    }

    protected void UnsubscribeFromDurability()
    {
        var durability = _statContainer.GetStat(StatTypes.Durability);
        durability.OnMinValueReached -= OnDestroyed;
    }

    protected async UniTask PlayDestructionAnimation()
    {
        _animator.enabled = true;
        _animator.SetTrigger("Dis");
        await UniTask.Delay(TimeSpan.FromSeconds(_animator.GetCurrentAnimatorStateInfo(0).length));
    }

    protected void HarvestAndCleanup()
    {
        Harvest();
        Destroy(gameObject);
    }

    protected virtual void StartFalling()
    {
        _isFalling = true;
    }

    protected virtual void Harvest()
    {
        var durability = _statContainer.GetStat(StatTypes.Durability);

        if (_pendingLoot == null) return;

        foreach (var item in _pendingLoot)
        {
            _signalBus.Fire(new ItemDropEvent(transform.position, item, false));
        }
    }
}
