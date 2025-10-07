using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemsCollection : MonoBehaviour, ICollection<ItemInstance>
{
    public Action onChange;

    [SerializeField] private List<ItemInstance> _collection;

    public int Count => _collection.Count;
    public bool RemoveWhenQuantityZero { get; set; } = true;
    public bool IsReadOnly => throw new NotImplementedException();

    public void AddEmpty() => _collection.Add(new ItemInstance());

    public void AddRangeEmpty(int count)
    {
        for (int i = 0; i < count; i++)
        {
            AddEmpty();
        }
    }

    /// <summary>
    /// Ignores ItemInstance.Count
    /// </summary>
    public int AddRange(ItemInstance itemInstance, int count)
    {
        int counter = 0;
        itemInstance.SetCount(1);
        for (int i = 0; i < count; i++)
        {
            var result = Add(itemInstance);
            if (!result) break;
            counter++;
        }
        return count - counter;
    }

    public bool AddAt(ItemInstance itemInstance, int slotNum)
    {
        int count = itemInstance.Count;
        return AddWithResult(itemInstance, slotNum, count);
    }

    public bool AddAtWithCount(ItemInstance itemInstance, int slotNum, int count)
    {
        return AddWithResult(itemInstance, slotNum, count);
    }

    public bool Add(ItemInstance itemInstance)
    {
        int count = itemInstance.Count;
        return AddWithResult(itemInstance, -1, count);
    }

    public bool AddWithResult(ItemInstance itemInstance, int slotNum = -1, int count = 1)
    {
        int i = 0;

        int owerflow;
        while (count > 0)
        {
            owerflow = AddWithOverflow(itemInstance, slotNum, count);

            if (owerflow == -1) // If valid slot not found
            {
                return false;
            }

            count = owerflow;

            i++;
            if (i == 100000)
            {
                Debug.Log("Endless loop");
                break;
            }
        }

        return true;
    }

    /// <summary>
    /// If overflow -1 - valid slot not found
    /// </summary>
    /// <returns></returns>
    public int AddWithOverflow(ItemInstance itemInstance, int slotNum = -1, int count = 1)
    {
        int overflow;
        bool toEmptySlot;

        int validSlot = FindValidSlot(itemInstance, out toEmptySlot);

        if (validSlot == -1)
        {
            return -1;
        }

        if (slotNum != -1)
        {
            if (IsSlotEmpty(slotNum))
                toEmptySlot = true;
            else
                toEmptySlot = false;

            validSlot = slotNum;
        }

        if (toEmptySlot || itemInstance.ItemDefinition != _collection[validSlot].ItemDefinition)
        {
            _collection[validSlot] = new ItemInstance(itemInstance.ItemDefinition, 1);
            _collection[validSlot].Add(count - 1, out overflow);
            return overflow;
        }
        else
        {
            _collection[validSlot].Add(count, out overflow);
            return overflow;
        }
    }

    private int FindValidSlot(ItemInstance itemInstance, out bool toEmptySlot)
    {
        int validSlot = -1;

        if (itemInstance.ItemDefinition.MaxCountInStack > 1)
        {
            validSlot = FindFirstSlotWithSameItemDef(itemInstance.ItemDefinition);
            toEmptySlot = false;

            if (validSlot != -1)
                return validSlot;
        }

        validSlot = FindFirstEmptySlot();

        if (validSlot == -1)
        {
            toEmptySlot = true;
            return -1;
        }

        toEmptySlot = true;
        return validSlot;
    }

    private int FindFirstEmptySlot()
    {
        for (int i = 0; i < _collection.Count; i++)
        {
            if (_collection[i].ItemDefinition == null)
            {
                return i;
            }
        }
        return -1;
    }

    private int FindFirstSlotWithSameItemDef(ItemDefinition itemDefinition)
    {
        for (int i = 0; i < _collection.Count; i++)
        {
            if ((_collection[i].ItemDefinition == itemDefinition) && (_collection[i].Count < _collection[i].ItemDefinition.MaxCountInStack))
            {
                return i;
            }
        }
        return -1;
    }

    private bool IsSlotEmpty(int slotNum)
    {
        if (_collection[slotNum].ItemDefinition == null)
            return true;
        else
            return false;
    }

    public void Remove(int itemIndex)
    {
        _collection[itemIndex] = new ItemInstance();
        _collection[itemIndex].SetCount(0);
        onChange?.Invoke();
    }

    public int FindIndex(ItemInstance itemInstance) =>
        _collection.IndexOf(itemInstance);

    public ItemInstance this[int index]
    {
        get => _collection[index];
        set
        {
            if (index < _collection.Count)
            {
                _collection[index] = value;
                onChange?.Invoke();
            }
        }
    }

    internal void RemoveRange(int count)
    {
        _collection.RemoveRange(_collection.Count - count, count);
    }

    public bool Remove(ItemInstance item)
    {
        Remove(_collection.IndexOf(item));
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public void CopyTo(ItemInstance[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<ItemInstance> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public bool Contains(ItemInstance item)
    {
        throw new NotImplementedException();
    }

    void ICollection<ItemInstance>.Add(ItemInstance item)
    {
        throw new NotImplementedException();
    }

    private void OnValidate()
    {
        foreach (var item in _collection)
        {
            if (item.ItemDefinition != null && item.Count <= 0)
            {
                item.SetCount(1);
            }
        }
    }
}
