//using System;
//using UnityEngine;
//namespace InventorySystem
//{
//    [Serializable]
//    public class ItemInstance
//    {
//        [SerializeField] private Item _item;

//        public Item Item { get => _item; set => _item = (Item)value; }

//        public int Count;

//        public ItemInstance Clone()
//        {
//            return new ItemInstance() { Item = Item, Count = Count };
//        }
//    }
//}