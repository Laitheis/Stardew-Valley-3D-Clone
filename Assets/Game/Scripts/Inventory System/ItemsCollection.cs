using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace InventorySystem
{
    public class ItemsCollection : MonoBehaviour, ICollection<ItemInstance>
    {
        public Action onChange;

        [SerializeField] private ItemInstance[] _itemInstances;

        // параллельные массивы для флагов
        private bool[] _isDragging;
        private bool[] _isReloading;

        private void Awake()
        {
            _isDragging = new bool[_itemInstances.Length];
            _isReloading = new bool[_itemInstances.Length];
        }

        public int Count => _itemInstances.Length;
        public bool RemoveWhenQuantityZero { get; set; } = true;

        // ==== Flags methods ====
        public void SetDragging(int index, bool value)
        {
            if (index >= 0 && index < _isDragging.Length)
                _isDragging[index] = value;
        }
        public bool GetDraggingFlag(int index) =>
            (index >= 0 && index < _isDragging.Length) && _isDragging[index];

        public void SetReloading(int index, bool value)
        {
            if (index >= 0 && index < _isReloading.Length)
                _isReloading[index] = value;
        }
        public bool GetReloading(int index) =>
            (index >= 0 && index < _isReloading.Length) && _isReloading[index];

        // ==== Collection logic ====
        public bool CanAdd(ItemDefinition item, int quantity, int slot)
        {
            return _itemInstances[slot].ItemDefinition == null;
        }

        public void Add(ItemInstance e) => TryAdd(e);

        public bool TryAdd(ItemInstance e) =>
            TryAdd(e.ItemDefinition, e.Count);

        public bool TryAdd(ItemDefinition item, int count)
        {
            int num = GetFree(item);
            if (num == -1)
                return false;

            _itemInstances[num] = new ItemInstance(item, count);

            onChange?.Invoke();
            return true;
        }

        public bool SetItemAt(ItemInstance entry, int num)
        {
            if (_itemInstances[num].ItemDefinition != null)
                return false;

            _itemInstances[num] = entry;
            return true;
        }

        public void Remove(ItemDefinition item)
        {
            int entryToRemove = Array.IndexOf(_itemInstances, Array.Find(_itemInstances, e => e.ItemDefinition == item));
            _itemInstances[entryToRemove] = null;

            ResetFlags(entryToRemove);
            onChange?.Invoke();
        }

        public void Remove(int itemIndex)
        {
            _itemInstances[itemIndex] = new ItemInstance();
            ResetFlags(itemIndex);

            onChange?.Invoke();
        }

        public bool Remove(ItemInstance ItemInstance)
        {
            Remove(Array.IndexOf(_itemInstances, ItemInstance));
            return true;
        }

        public bool Reduce(int itemIndex, int reduceAmount)
        {
            ItemInstance e = _itemInstances[itemIndex];
            int newQuantity = reduceAmount >= e.Count ? 0 : e.Count - reduceAmount;

            _itemInstances[itemIndex].SetCount(newQuantity);
            onChange?.Invoke();
            if (RemoveWhenQuantityZero && newQuantity == 0)
            {
                Remove(e);
                return true;
            }
            return false;
        }

        public ItemInstance GetItemAt(int num)
        {
            if (num >= _itemInstances.Length)
                return null;
            return _itemInstances[num];
        }

        public ItemInstance Find(ItemDefinition i) =>
            Array.Find(_itemInstances, e => e.ItemDefinition == i);

        public int FindIndex(ItemDefinition i) =>
            Array.IndexOf(_itemInstances, Array.Find(_itemInstances, e => e.ItemDefinition == i));

        public int FindIndex(ItemInstance e) =>
            Array.IndexOf(_itemInstances, e);

        public int GetFirstValidSlot(ItemDefinition i, int quantity)
        {
            ItemInstance finded = Array.Find(_itemInstances, i => i.ItemDefinition == null);

            if (finded == null)
                return -1;
            else
                return Array.IndexOf(_itemInstances, finded);
        }

        public IEnumerator<ItemInstance> GetEnumerator() =>
            new ItemCollectionEnumerator(_itemInstances);

        public void Clear()
        {
            Array.Clear(_itemInstances, 0, _itemInstances.Length);
            Array.Clear(_isDragging, 0, _isDragging.Length);
            Array.Clear(_isReloading, 0, _isReloading.Length);

            onChange?.Invoke();
        }

        public bool Contains(ItemInstance e) =>
            _itemInstances.Contains(e);

        public void CopyTo(ItemInstance[] c, int index) =>
            c = _itemInstances;

        public bool IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ItemInstance this[int index]
        {
            get => _itemInstances[index];
            set
            {
                if (index < _itemInstances.Length)
                {
                    _itemInstances[index] = value;
                    ResetFlags(index); // сбросить флаги при замене
                    onChange?.Invoke();
                }
            }
        }

        int GetFree(ItemDefinition item)
        {
            for (int i = 0; i < _itemInstances.Length; i++)
            {
                if (_itemInstances[i].ItemDefinition == null)
                    return i;
            }
            return -1;
        }

        public void ResetAllFlags()
        {
            for (int i = 0; i < _itemInstances.Length; i++)
            {
                _isDragging[i] = false;
                _isReloading[i] = false;
            }
        }

        private void ResetFlags(int index)
        {
            if (index >= 0 && index < _itemInstances.Length)
            {
                _isDragging[index] = false;
                _isReloading[index] = false;
            }
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
