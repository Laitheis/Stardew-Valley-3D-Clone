using System;
using UnityEngine;

public class ItemInstance
{
    [SerializeField] private ItemDefinition _itemDefinition;

    [SerializeField] private string _customName;
    [SerializeField] private int _customPrice;
    [SerializeField] private string _id;
    [SerializeField] private int _count;

    public ItemDefinition ItemDefinition => _itemDefinition;

    public string Name => string.IsNullOrEmpty(_customName) ? _itemDefinition.Name : _customName;
    public int Price => _customPrice != 0 ? _customPrice : _itemDefinition.Price;
    public string Id => _id;
    public int Count => _count;

    public ItemInstance(ItemDefinition definition, int count = 1)
    {
        _itemDefinition = definition;
        _customName = definition.Name;
        _customPrice = definition.Price;
        _id = Guid.NewGuid().ToString();
        _count = Mathf.Clamp(count, 0, definition.MaxCountInStack);
    }

    public bool Rename(string newName)
    {
        if (_itemDefinition.IsRenameable && !string.IsNullOrEmpty(newName))
        {
            _customName = newName;
            return true;
        }
        return false;
    }

    public bool AddCount(int amount, out int overflow)
    {
        overflow = 0;

        if (!_itemDefinition.IsStackable || amount <= 0)
            return false;

        int newCount = _count + amount;
        if (newCount > _itemDefinition.MaxCountInStack)
        {
            overflow = newCount - _itemDefinition.MaxCountInStack;
            _count = _itemDefinition.MaxCountInStack;
        }
        else
        {
            _count = newCount;
        }

        return true;
    }

    public bool RemoveCount(int amount, out int underflow)
    {
        underflow = 0;

        if (amount <= 0 || _count <= 0)
            return false;

        int newCount = _count - amount;
        if (newCount < 0)
        {
            underflow = -newCount;
            _count = 0;
        }
        else
        {
            _count = newCount;
        }

        return true;
    }

    public void SetPrice(int newPrice)
    {
        if (newPrice >= 0)
            _customPrice = newPrice;
    }

    public bool IsFull() => _count >= _itemDefinition.MaxCountInStack;
    public bool IsEmpty() => _count <= 0;

    public void UseItem()
    {
        _itemDefinition.UseItem();
    }
}