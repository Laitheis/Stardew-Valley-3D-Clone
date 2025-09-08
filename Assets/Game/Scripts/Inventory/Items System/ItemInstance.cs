using System;
using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    [SerializeField] private ItemDefinition _itemDefinition;

    [SerializeField] private string _guid;
    [SerializeField] [Min(0)] private int _count = 0;
    [SerializeField] private ItemFlags _itemFlags;

    public ChangeableProperties Properties;

    [System.Serializable]
    public class ChangeableProperties
    {
        [SerializeField] public string CustomName;
        [SerializeField] public int CustomPrice;
    }

    public ItemDefinition ItemDefinition { get => _itemDefinition; set => _itemDefinition = value; }

    public string Name => string.IsNullOrEmpty(Properties.CustomName) ? _itemDefinition.Name : Properties.CustomName;
    public int Price => Properties.CustomPrice != 0 ? Properties.CustomPrice : _itemDefinition.Price;
    public string Guid => _guid;
    public int Count => _count;
    public ItemFlags ItemFlags => _itemFlags;

    public ItemInstance(ItemDefinition definition, int count = 1)
    {
        _itemDefinition = definition;
        Properties = new ChangeableProperties();
        Properties.CustomName = definition.Name;
        Properties.CustomPrice = definition.Price;
        _guid = System.Guid.NewGuid().ToString();
        _count = Mathf.Clamp(count, 1, definition.MaxCountInStack);
    }

    public ItemInstance()
    {
        _guid = System.Guid.NewGuid().ToString();
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

    public void Add(int amount, out int overflow)
    {
        if (amount <= 0)
        {
            overflow = 0;
            return;
        }

        overflow = 0;

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

    // Flags methods
    public void AddFlag(ItemFlags flag)
    {
        _itemFlags |= flag;
    }

    public void RemoveFlag(ItemFlags flag)
    {
        _itemFlags &= ~flag;
    }

    public void ToggleFlag(ItemFlags flag)
    {
        _itemFlags ^= flag;
    }

    public bool HasFlag(ItemFlags flag)
    {
        return (_itemFlags & flag) == flag;
    }

    public void ClearFlags()
    {
        _itemFlags = ItemFlags.None;
    }

    // Use Item
    public void UseItem()
    {
        _itemDefinition.UseItem();
    }
}


[System.Flags]
public enum ItemFlags
{
    None = 0,
    IsDragging = 1 << 0
}
