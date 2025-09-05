using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class ItemsCollection : MonoBehaviour, ICollection<ItemInstance>
    {
        public Action onChange;

        [SerializeField] private List<ItemInstance> _itemInstances;

        public int Count => _itemInstances.Count;
        public bool RemoveWhenQuantityZero { get; set; } = true;
        public bool IsReadOnly => throw new NotImplementedException();

        // ==== Collection logic ====
        //public bool CanAdd(ItemDefinition item, int quantity, int slot)
        //{
        //    return _itemInstances[slot].ItemDefinition == null;
        //}

        public void AddEmpty() => _itemInstances.Add(new ItemInstance());

        public void AddRange(int count)
        {
            for (int i = 0; i < count; i++)
            {
                AddEmpty();
            }
        }


        public void Add(ItemInstance itemInstance) => Debug.Log(""); //TryAdd(itemInstance);

        //private void TryAdd(ItemInstance itemInstance)
        //{
        //    if(itemInstance.ItemDefinition == null)
        //    {

        //    }

        //    TryAdd(itemInstance.ItemDefinition, itemInstance.Count);
        //}

        //public bool TryAdd(ItemDefinition item, int count)
        //{
        //    int num = GetFree(item);
        //    if (num == -1)
        //        return false;

        //    _itemInstances[num] = new ItemInstance(item, count);

        //    onChange?.Invoke();
        //    return true;
        //}

        //public bool SetItemAt(ItemInstance entry, int index)
        //{
        //    if (_itemInstances[index].ItemDefinition != null)
        //        return false;

        //    _itemInstances[index] = entry;
        //    return true;
        //}

        //public void Remove(ItemDefinition item)
        //{
        //    int entryToRemove = Array.IndexOf(_itemInstances, Array.Find(_itemInstances, e => e.ItemDefinition == item));
        //    _itemInstances[entryToRemove] = null;

        //    ResetFlags(entryToRemove);
        //    onChange?.Invoke();
        //}

        public void Remove(int itemIndex)
        {
            _itemInstances[itemIndex] = new ItemInstance();

            onChange?.Invoke();
        }

        //public bool Remove(ItemInstance ItemInstance)
        //{
        //    Remove(Array.IndexOf(_itemInstances, ItemInstance));
        //    return true;
        //}

        //public bool Reduce(int itemIndex, int reduceAmount)
        //{
        //    ItemInstance e = _itemInstances[itemIndex];
        //    int newQuantity = reduceAmount >= e.Count ? 0 : e.Count - reduceAmount;

        //    _itemInstances[itemIndex].SetCount(newQuantity);
        //    onChange?.Invoke();
        //    if (RemoveWhenQuantityZero && newQuantity == 0)
        //    {
        //        Remove(e);
        //        return true;
        //    }
        //    return false;
        //}

        //public ItemInstance GetItemAt(int index)
        //{
        //    if (index >= _itemInstances.Length)
        //        return null;
        //    return _itemInstances[index];
        //}

        //public ItemInstance Find(ItemDefinition i) =>
        //    Array.Find(_itemInstances, e => e.ItemDefinition == i);

        //public int FindIndex(ItemDefinition i) =>
        //    Array.IndexOf(_itemInstances, Array.Find(_itemInstances, e => e.ItemDefinition == i));

        public int FindIndex(ItemInstance itemInstance) =>
            _itemInstances.IndexOf(itemInstance);

        //public int GetFirstValidSlot(ItemDefinition i, int quantity)
        //{
        //    ItemInstance finded = Array.Find(_itemInstances, i => i.ItemDefinition == null);

        //    if (finded == null)
        //        return -1;
        //    else
        //        return Array.IndexOf(_itemInstances, finded);
        //}

        //public IEnumerator<ItemInstance> GetEnumerator() =>
        //    new ItemCollectionEnumerator(_itemInstances);

        //public void Clear()
        //{
        //    Array.Clear(_itemInstances, 0, _itemInstances.Length);
        //    Array.Clear(_isDragging, 0, _isDragging.Length);
        //    Array.Clear(_isReloading, 0, _isReloading.Length);

        //    onChange?.Invoke();
        //}

        //public bool Contains(ItemInstance e) =>
        //    _itemInstances.Contains(e);

        //public void CopyTo(ItemInstance[] c, int index) =>
        //    c = _itemInstances;

        //public bool IsReadOnly => false;

        //IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ItemInstance this[int index]
        {
            get => _itemInstances[index];
            set
            {
                if (index < _itemInstances.Count)
                {
                    _itemInstances[index] = value;
                    onChange?.Invoke();
                }
            }
        }

        //int GetFree(ItemDefinition item)
        //{
        //    for (int i = 0; i < _itemInstances.Count; i++)
        //    {
        //        if (_itemInstances[i].ItemDefinition == null)
        //            return i;
        //    }
        //    return -1;
        //}

        //public void ResetAllFlags()
        //{
        //    for (int i = 0; i < _itemInstances.Count; i++)
        //    {
        //        _isDragging[i] = false;
        //        _isReloading[i] = false;
        //    }
        //}

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(ItemInstance[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(ItemInstance item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<ItemInstance> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(ItemInstance item)
        {
            throw new NotImplementedException();
        }

        internal void RemoveRange(int count)
        {
            _itemInstances.RemoveRange(_itemInstances.Count - count, count);
        }
    }

    public class ItemCollectionEnumerator : IEnumerator<ItemInstance>
    {
        private ItemInstance[] _items;
        private int _position = -1;

        public ItemCollectionEnumerator(ItemInstance[] items)
        {
            _items = items;
        }

        public ItemInstance Current
        {
            get
            {
                if (_position < 0 || _position >= _items.Length)
                    throw new InvalidOperationException();
                return _items[_position];
            }
        }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            _position++;
            return _position < _items.Length;
        }

        public void Reset() => _position = -1;

        public void Dispose() { }
    }
}