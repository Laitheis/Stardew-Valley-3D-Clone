using InventorySystem;
using UnityEngine;
using Zenject;

public class PickableItem : MonoBehaviour
{
    private Transform _player;
    [SerializeField] private float _pickupDistance;
    [SerializeField] private ItemInstance _item;

    private bool _isPickable;
    private float _unpickableTimer = 2f;

    public ItemInstance Item { get => _item; set => _item = value; }
    public bool IsPickable { get => _isPickable; set => _isPickable = value; }

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

        if (_unpickableTimer > 0)
        {
            _unpickableTimer -= Time.deltaTime;
        }
        else _isPickable = true;
    }

    void OnPlayerCollide()
    {
        if (!_isPickable) return;

        var playerCollection = GameObject.FindWithTag("PlayerInv").GetComponent<InventoryHandler>().Collection;
        if (playerCollection.Add(Item))
        {
            Destroy(gameObject);
        }

    }
}
