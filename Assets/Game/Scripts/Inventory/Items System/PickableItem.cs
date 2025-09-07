using InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    Transform _player;
    [SerializeField] float _collisionDistance;
    [SerializeField] ItemInstance _item;
    private void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
    }
    private void Update()
    {
        if (Vector3.Distance(_player.position, transform.position) < _collisionDistance)
        {
            OnPlayerCollide();
        }
    }
    void OnPlayerCollide()
    {
        //HACK
        //Вызывать метод в InvHandler
        GameObject.FindWithTag("PlayerInv").GetComponent<InventoryHandler>().SetItemInstance(_item);
        Destroy(gameObject);
    }
}
