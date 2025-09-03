using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InventorySystem
{
    public class ItemsCollection : MonoBehaviour, ICollection<ItemInstance>
    {
        public Action onChange;

        [SerializeField] private ItemInstance[] _itemInstances;
        public int Count => _itemInstances.Length;
        public bool RemoveWhenQuantityZero { get; set; } = true;
        public bool CanAdd(ItemDefinition item, int quantity, int slot)   
        {
            if (_itemInstances[slot].ItemDefinition == null)   
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public void Add(ItemInstance e)
        {
            TryAdd(e);  
        }
        /// <returns>true - предмет был добавлен в количестве 1 или более штуки, false - предмет не добавлен</returns>
        public bool TryAdd(ItemInstance e)
        {
            return TryAdd(e.ItemDefinition, e.Count);    
        }
        /// <returns>true - предмет был добавлен в количестве 1 или более штуки, false - предмет не добавлен</returns>
        public bool TryAdd(ItemDefinition item, int count)         
        {
            int num = GetFree(item);
            if (num == -1)
            {
                return false;
            }
            _itemInstances[num] = new ItemInstance(item, count);

            onChange?.Invoke();
            return true;
        }
        public bool SetItemAt(ItemInstance entry, int num)
        {
            if (_itemInstances[num].ItemDefinition != null)
            {
                return false;
            }
            _itemInstances[num] = entry;
            return true;
        }
        public void Remove(ItemDefinition item)
        {
            int entryToRemove = Array.IndexOf(_itemInstances, Array.Find(_itemInstances, e => e.ItemDefinition == item));
            _itemInstances[entryToRemove] = null;

            onChange?.Invoke();
        }
        public void Remove(int itemIndex)
        {
            _itemInstances[itemIndex].ItemDefinition = null;

            onChange?.Invoke();
        }
        public bool Remove(ItemInstance ItemInstance)
        {
            Remove(Array.IndexOf(_itemInstances, ItemInstance));
            return true;
        }
        /// <returns>был ли предмет удален</returns>
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
            {
                return null;
            }
            return _itemInstances[num];
        }
        public ItemInstance Find(ItemDefinition i)
        {
            return Array.Find(_itemInstances, e => e.ItemDefinition == i);
        }
        public int FindIndex(ItemDefinition i)
        {
            return Array.IndexOf(_itemInstances, Array.Find(_itemInstances, e => e.ItemDefinition == i));
        }
        public int FindIndex(ItemInstance e)
        {
            return Array.IndexOf(_itemInstances, e);
        }
        public int GetFirstValidSlot(ItemDefinition i, int quantity)
        {
            //HACK
            ItemInstance finded = Array.Find(_itemInstances, i => i.ItemDefinition == null);

            if (finded == null)
                return -1;
            else
                return Array.IndexOf(_itemInstances, finded);
        }
        public IEnumerator<ItemInstance> GetEnumerator()
        {
            return new ItemCollectionEnumerator(_itemInstances);
        }
        public void Clear()
        {
            Array.Clear(_itemInstances, 0, _itemInstances.Length);
            onChange?.Invoke();
        }
        public bool Contains(ItemInstance e)
        {
            return _itemInstances.Contains(e);
        }
        public void CopyTo(ItemInstance[] c, int index)
        {
            c = _itemInstances;
        }
        public bool IsReadOnly => false;
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public ItemInstance this[int index]
        {
            get => _itemInstances[index];
            set
            {
                if (index < _itemInstances.Length)
                {
                    _itemInstances[index] = value;
                    onChange?.Invoke();
                }
            }
        }
        int GetFree(ItemDefinition item)
        {
            for (int i = 0; i < _itemInstances.Length; i++)
            {
                if (_itemInstances[i].ItemDefinition == null)
                {
                    return i;
                }
            }
            return -1;
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

        public void Reset()
        {
            _position = -1;
        }

        public void Dispose()
        {

        }
    }
}
