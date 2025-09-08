using InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PickableItem : MonoBehaviour
{
    private Transform _player;
    [SerializeField] private float _pickupDistance;
    [SerializeField] private ItemInstance _item;

    public ItemInstance Item { get => _item; set => _item = value; }

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
    }

    [Inject]
    private void Constructor([Inject(Id = "PickupDistance")] int pickupDistance)
    {
        _pickupDistance = pickupDistance;
    }

    private void Update()
    {
        if (Vector3.Distance(_player.position, transform.position) < _pickupDistance)
        {
            OnPlayerCollide();
        }
    }

    void OnPlayerCollide()
    {
        GameObject.FindWithTag("PlayerInv").GetComponent<InventoryHandler>().Collection.Add(Item);
        Destroy(gameObject);
    }
}
