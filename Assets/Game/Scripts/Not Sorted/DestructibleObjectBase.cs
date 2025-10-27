using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class DestructibleObjectBase : MonoBehaviour
{
    [SerializeField] private Texture _damageTex;

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
    [Inject(Id = "DamageParticles")] private GameObject _damageParticles;

    public virtual void Init(Vector3Int gridPos)
    {
        _gridPos = gridPos;

        _animator = GetComponent<Animator>();

        InitializeStats();
        InitializeLoot();

        _inited = true;

        ChangeLayers();
    }

    void Reset()
    {
        ChangeLayers();
    }

    private void ChangeLayers()
    {
        int targetLayer = LayerMask.NameToLayer("StaticObject");
        if (targetLayer == -1)
        {
            Debug.LogError($"Layer does not exist!");
            return;
        }

        SetLayerRecursively(transform, targetLayer);
    }

    private void SetLayerRecursively(Transform parent, int layer)
    {
        parent.gameObject.layer = layer;
        Debug.Log($"Changed layer to {layer} for: {parent.name}");

        for (int i = 0; i < parent.childCount; i++)
        {
            SetLayerRecursively(parent.GetChild(i), layer);
        }
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

    public virtual void OnDestroyed()
    {
        Debug.Log($"{gameObject} has been destroyed");

        UnsubscribeFromDurability();
        StartFalling();

        HarvestAndCleanup();

        MainGameManager.instance.farmTiles.TilesCollection[_gridPos].objectOnTile = null;
    }

    public virtual void TakeDamage(int amount, ItemType tool)
    {
        if (_isFalling) return;
        if (tool != _acceptableTool) return;
        if (_statContainer == null) return;

        Debug.Log($"{gameObject} has {amount} damage.");

        Stat stat = _statContainer.GetStat(StatTypes.Durability);
        if (stat == null) return;

        stat.Value -= amount;
        _animator.SetTrigger("Damage");
        var particles = Instantiate(_damageParticles, gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.Euler(new Vector3(-90, 0, 0)));

        Material mat = particles.gameObject.GetComponent<Renderer>().material;
        if (mat.HasProperty("_BaseMap"))
        {
            mat.SetTexture("_BaseMap", _damageTex);
        }
        Destroy(particles, 2f);

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
        await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
        _animator.Play("Disappearance", 0, 0f);
        await UniTask.Delay(TimeSpan.FromSeconds(_animator.GetCurrentAnimatorStateInfo(0).length));
    }

    protected async void HarvestAndCleanup()
    {
        Harvest();
        await PlayDestructionAnimation();
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
