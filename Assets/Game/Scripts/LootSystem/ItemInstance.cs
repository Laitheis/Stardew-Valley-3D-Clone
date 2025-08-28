using System;
using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    [SerializeField] private ItemDefinition _itemDefinition;

    [SerializeField] private string _id;
    [SerializeField] public int _count;

    public ChangeableProperties Properties;

    [System.Serializable]
    public class ChangeableProperties
    {
        [SerializeField] public string CustomName;
        [SerializeField] public int CustomPrice;
    }

    public ItemDefinition ItemDefinition => _itemDefinition;

    public string Name => string.IsNullOrEmpty(Properties.CustomName) ? _itemDefinition.Name : Properties.CustomName;
    public int Price => Properties.CustomPrice != 0 ? Properties.CustomPrice : _itemDefinition.Price;
    public string Id => _id;
    public int Count => _count;

    public ItemInstance(ItemDefinition definition, int count = 0)
    {
        _itemDefinition = definition;
        Properties = new ChangeableProperties();
        Properties.CustomName = definition.Name;
        Properties.CustomPrice = definition.Price;
        _id = Guid.NewGuid().ToString();
        _count = Mathf.Clamp(count, 0, definition.MaxCountInStack);
    }

    public bool Rename(string newName)
    {
        if (_itemDefinition.IsRenameable && !string.IsNullOrEmpty(newName))
        {
            Properties.CustomName = newName;
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

    public bool SetCount(int newCount)
    {
        if (!(newCount > 0 && newCount <= _itemDefinition.MaxCountInStack))
            return false;

        _count = newCount;

        return true;
    }

    public void SetPrice(int newPrice)
    {
        if (newPrice >= 0)
            Properties.CustomPrice = newPrice;
    }

    public bool IsFull() => _count >= _itemDefinition.MaxCountInStack;
    public bool IsEmpty() => _count <= 0;

    public void UseItem()
    {
        _itemDefinition.UseItem();
    }
}