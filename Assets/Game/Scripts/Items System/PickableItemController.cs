using InventorySystem;
using UnityEngine;
using Zenject;

public class PickableItemController : MonoBehaviour
{
    [Inject(Id = "Player")] private GameObject _player;
    private float _pickupDistance;
    [SerializeField] private ItemInstance _item;

    private bool _isPickable;
    private float _unpickableTimer = 2f;

    public ItemInstance Item { get => _item; set => _item = value; }
    public bool IsPickable { get => _isPickable; set => _isPickable = value; }

    [Inject]
    private void Constructor([Inject(Id = "PickupDistance")] int pickupDistance)
    {
        _pickupDistance = pickupDistance;
    }

    private void Update()
    {
        if (Vector3.Distance(_player.transform.position, transform.position) < _pickupDistance)
        {
            OnPlayerCollide();
        }

        if (_unpickableTimer > 0)
        {
            _unpickableTimer -= Time.deltaTime;
        }
        else _isPickable = true;

        if (transform.position.y < -40)
            Destroy(gameObject);
    }

    void OnPlayerCollide()
    {
        if (Time.timeScale == 0) return;
        if (!(GameStateService.instance.CurrentState is WorldState || GameStateService.instance.CurrentState is TradeState)) return;
        if (!_isPickable) return;

        var playerCollection = GameObject.FindWithTag("PlayerInv").GetComponent<InventoryHandler>().Collection;
        if (playerCollection.Add(Item))
        {
            Destroy(gameObject);
        }

    }
}
