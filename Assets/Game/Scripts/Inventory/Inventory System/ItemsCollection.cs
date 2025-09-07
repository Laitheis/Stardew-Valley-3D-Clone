using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

        public void AddRangeEmpty(int count)
        {
            for (int i = 0; i < count; i++)
            {
                AddEmpty();
            }
        }

        public bool AddAt(ItemInstance itemInstance, int slotNum, int count = 1)
        {
            return AddWithResult(itemInstance, slotNum, count);
        }

        public bool Add(ItemInstance itemInstance, int count = 1)
        {
            return AddWithResult(itemInstance, -1, count);
        }

        public bool AddWithResult(ItemInstance itemInstance, int slotNum = -1, int count = 1)
        {
            int i = 0;

            int owerflow;
            while (count > 0)
            {
                owerflow = AddWithOwerflow(itemInstance, slotNum, count);

                if (owerflow == -1)
                {
                    return false;
                }

                count = owerflow;

                i++;
                if (i == 10000)
                {
                    Debug.Log("Endless loop");
                    break;
                }
            }

            return true;
        }

        private int AddWithOwerflow(ItemInstance itemInstance, int slotNum = -1, int count = 1)
        {
            int overflow;
            bool toEmptySlot;

            int validSlot = FindValidSlot(itemInstance, out toEmptySlot);

            if(validSlot == -1)
            {
                return -1;
            }

            if(slotNum != -1)
            {
                if (IsSlotEmpty(slotNum))
                    toEmptySlot = true;
                else
                    toEmptySlot = false;

                validSlot = slotNum;
            }

            if (toEmptySlot)
            {
                _itemInstances[validSlot] = new ItemInstance(itemInstance.ItemDefinition, 1);
                _itemInstances[validSlot].Add(count - 1, out overflow);
                return overflow;
            }
            else
            {
                _itemInstances[validSlot].Add(count, out overflow);
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
            }

            if (validSlot == -1)
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
            for (int i = 0; i < _itemInstances.Count; i++)
            {
                if (_itemInstances[i].ItemDefinition == null)
                {
                    return i;
                }
            }
            return -1;
        }

        private int FindFirstSlotWithSameItemDef(ItemDefinition itemDefinition)
        {
            for (int i = 0; i < _itemInstances.Count; i++)
            {
                if ((_itemInstances[i].ItemDefinition == itemDefinition) && (_itemInstances[i].Count < _itemInstances[i].ItemDefinition.MaxCountInStack))
                {
                    return i;
                }
            }
            return -1;
        }

        private bool IsSlotEmpty(int slotNum)
        {
            if (_itemInstances[slotNum].ItemDefinition == null)
                return true;
            else
                return false;
        }
        
        public void Remove(int itemIndex)
        {
            _itemInstances[itemIndex] = new ItemInstance();

            onChange?.Invoke();
        }
       
        public int FindIndex(ItemInstance itemInstance) =>
            _itemInstances.IndexOf(itemInstance);

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

        void ICollection<ItemInstance>.Add(ItemInstance item)
        {
            throw new NotImplementedException();
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