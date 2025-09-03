//using InventorySystem;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace InventorySystem
//{
//    [CreateAssetMenu]
//    public class ItemsDatabase : ScriptableObject
//    {
//        public static ItemsDatabase Instance => ItemsManager.ItemsDatabase;
//        public List<ItemDefinition> Items;
        
//        public static ItemDefinition Find(string name) => Instance.Items.Find(i => i.Name == name);
//    }
//}