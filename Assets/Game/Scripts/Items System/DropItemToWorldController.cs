using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DropItemToWorldController : Zenject.IInitializable
{
    [Inject] private SignalBus _signalBus;
    [Inject(Id = "SmokeEffect")] private GameObject _smokeEffect;
    [Inject(Id = "StarParticles")] private GameObject _starsEffect;

    private DiContainer _container;
    private GameObject _itemPrefab;

    private Vector3 _defaultSpawnOffset = new Vector3(1, 1, 1);

    public DropItemToWorldController(DiContainer container)
    {
        _container = container;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<ItemDropEvent>(OnItemDrop);
    }

    private void OnItemDrop(ItemDropEvent signal)
    {
        ItemDefinition itemDefinition = signal.Item.ItemDefinition;

        _itemPrefab = itemDefinition.Prefab;

        List<ItemInstance> singleItemInstances = new List<ItemInstance>();
        for (int i = 0; i < signal.Item.Count; i++)
        {
            ItemInstance _itemInstance = new ItemInstance(itemDefinition);

            _itemInstance.SetCount(1);

            singleItemInstances.Add(_itemInstance);
        }

        Vector3 _spawnOffset = _defaultSpawnOffset;
        foreach (var item in singleItemInstances)
        {
            var itemGO = _container.InstantiatePrefab(
                _itemPrefab,
                signal.Position + _spawnOffset,
                Quaternion.identity,
                null);
            itemGO.GetComponent<BoxCollider>().enabled = false;
            DOVirtual.DelayedCall(0.3f, () => { itemGO.GetComponent<BoxCollider>().enabled = true; }, false);

            _spawnOffset += new Vector3(0, 1, 0);

            Vector3 direction;
            if (!signal.IsDroppedFromPlayer)
            {
                direction = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f));
            }
            else
            {
                direction = Vector3.up;
            }

            itemGO.GetComponent<Rigidbody>().AddForce(direction * 5f, ForceMode.Impulse);

            var itemInstance = itemGO.GetComponent<PickableItemController>();

            itemInstance.Item = item;

            var stars = _container.InstantiatePrefab(_starsEffect, itemGO.transform);
        }

        if (!signal.IsDroppedFromPlayer)
        {
            var smoke = _container.InstantiatePrefab(_smokeEffect, signal.Position, Quaternion.identity, null);
            GameObject.Destroy(smoke, 1.5f);
        }


        //Debug.Log($"Loot named {signal.Item.Name} drops at position {signal.Position} with quantity {signal.Item.Count}");
    }
}

