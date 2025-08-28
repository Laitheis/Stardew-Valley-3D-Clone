using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class TreeBase : MonoBehaviour, IHarvestable, IDestructible, IStats
{
    [Inject] private SignalBus _signalBus;

    protected LootGenerator _lootGenerator;
    protected GameObject _smokeEffect;

    protected StatContainter _statContainer;
    protected List<ItemInstance> _pendingLoot;

    protected Animator _animator;

    protected bool _isFalling;

    public StatContainter StatContainer => _statContainer;

    [Inject]
    public void Construct(LootGenerator lootGenerator, [Inject(Id = "SmokeEffect")] GameObject smokeEffect)
    {
        _lootGenerator = lootGenerator;
        _smokeEffect = smokeEffect;

        _animator = GetComponent<Animator>();

        InitializeStats();
        InitializeLoot();
    }

    public virtual void InitializeStats()
    {
        _statContainer = new StatContainter();

        var durability = _statContainer.Add(StatTypes.Durability, 100);

        durability.OnMinValueReached += OnDestroyed;
    }

    public virtual void InitializeLoot()
    {
        _pendingLoot = _lootGenerator.GenerateLoot("Oak", 0);
    }

    public virtual async void OnDestroyed()
    {
        Debug.Log($"{gameObject} has been destroyed");

        UnsubscribeFromDurability();
        StartFalling();

        await HandleTrunkFall();

        await PlayDestructionAnimation();

        HarvestAndCleanup();
    }

    public virtual void Harvest()
    {
        var durability = _statContainer.GetStat(StatTypes.Durability);

        if (_pendingLoot == null) return;

        foreach (var item in _pendingLoot)
        {
            _signalBus.Fire(new ItemDropSignal(transform.position, item));
        }
    }

    public virtual void TakeDamage(int amount)
    {
        if (_isFalling) return;

        Debug.Log($"{gameObject} has {amount} damage.");

        _statContainer.GetStat(StatTypes.Durability).Value -= amount;


        _animator.SetTrigger("Cut");

        Debug.Log($"new {gameObject} durability is {_statContainer.GetStat(StatTypes.Durability).Value}");
    }

    private void UnsubscribeFromDurability()
    {
        var durability = _statContainer.GetStat(StatTypes.Durability);
        durability.OnMinValueReached -= OnDestroyed;
    }

    private void StartFalling()
    {
        _isFalling = true;
        _animator.enabled = false;
    }

    private async UniTask HandleTrunkFall()
    {
        GameObject trunk = transform.Find("Trunk").gameObject;
        Rigidbody trunkRigidbody = trunk.AddComponent<Rigidbody>();
        SetupTrunkPhysics(trunkRigidbody);
        trunkRigidbody.AddForce(Vector3.right * 5000f);

        await UniTask.Delay(TimeSpan.FromSeconds(4f));
    }

    private async UniTask PlayDestructionAnimation()
    {
        _animator.enabled = true;
        _animator.SetTrigger("Dis");
        await UniTask.Delay(TimeSpan.FromSeconds(_animator.GetCurrentAnimatorStateInfo(0).length));
    }

    private void HarvestAndCleanup()
    {
        Harvest();
        Instantiate(_smokeEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void SetupTrunkPhysics(Rigidbody rigidbody)
    {
        if (rigidbody == null) return;

        rigidbody.mass = 50f;
        rigidbody.drag = 0.2f;
        rigidbody.angularDrag = 0.1f;
    }
}
