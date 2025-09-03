//using InventorySystem;
//using System.Collections;
//using TMPro;
//using UnityEngine;
//using Zenject;

//namespace InventorySystem
//{
//    public class ItemsManager : MonoBehaviour
//    {
//        [Inject] private ItemDatabase _itemsDatabase;

//        public static ItemsManager Instance;

//        public static ItemDatabase ItemsDatabase => Instance._itemsDatabase;

//        private void Awake()
//        {
//            Instance = this;

//            ItemsManager founded = GameObject.FindObjectOfType<ItemsManager>();
//            if (founded != null && founded != this)
//            {
//                Destroy(gameObject);
//                return;
//            }
//            if (_itemsDatabase == null)
//            {
//                //IEnumerator e = Unity.VisualScripting.AssetUtility.GetAllAssetsOfType<ItemsDatabase>().GetEnumerator();
//                //e.MoveNext();
//                //itemsDatabase = (ItemsDatabase)e.Current;
//            }
//            DontDestroyOnLoad(gameObject);
//        }
//    }
//}