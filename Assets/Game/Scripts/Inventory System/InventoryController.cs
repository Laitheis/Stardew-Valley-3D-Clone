//using UnityEngine;
//using UnityEngine.EventSystems;

//namespace InventorySystem
//{
//    [RequireComponent(typeof(ItemsCollection))]
//    public class InventoryController : MonoBehaviour
//    {
//        //[SerializeField] KeyCode _openKey;

//        [SerializeField] ItemsListView _view;
//        [SerializeField] GameObject itemPopUpWindow;
//        [SerializeField] GameObject ButtonPopUpWindow;
//        [SerializeField] GameObject PopUpRoot;


//        ItemsCollection _collection;

//        public ItemsCollection Collection => _collection;

//        ItemInstance _draggedItem;
//        private void Start()
//        {
//            _collection = GetComponent<ItemsCollection>();
            
//        }
        
//        void Update()
//        {
            
//            //if (Input.GetMouseButtonDown(0))
//            //{
//            //    if (!IsPointerOverUIObject(itemPopUpWindow, ButtonPopUpWindow))
//            //    {
//            //        OnGlobalClick();
//            //    }
//            //}
//            //if (Input.GetKeyDown(KeyCode.Escape))
//            //{
//            //    if (!IsPointerOverUIObject(itemPopUpWindow, ButtonPopUpWindow))
//            //    {
//            //        OnGlobalClick();
//            //    }
//            //}
//        }
//        public int CountItems(ItemDefinition item) => _collection.Count;
//        public void ThrowItemToWorld(int itemIndex)
//        {
//            ThrowItemToWorld(itemIndex, 1);
//        }
//        public void ThrowItemToWorld(int itemIndex, int quantity)
//        {
//            ItemInstance item = _collection[itemIndex];
//            bool completly = _collection.Reduce(itemIndex, quantity);

//            if (completly)
//            {
//                GameObject go = Instantiate(item.ItemDefinition.Prefab, transform.position, Quaternion.identity);
//                var c = go.GetComponent<WorldItemController>();
//                c.ItemInstance = item;
//            }
//            else
//            {
//                item.SetCount(quantity);
//                GameObject go = Instantiate(item.ItemDefinition.Prefab, transform.position, Quaternion.identity);
//                var c = go.GetComponent<WorldItemController>();
//                c.ItemInstance = item;
//            }

//        }
//        public void AddFromWorld(WorldItem itemObject)
//        {
//            _collection.TryAdd(itemObject.ItemInstance.ItemDefinition, itemObject.ItemInstance.Count);
//            Destroy(itemObject.gameObject);
//        }
//        private bool IsPointerOverUIObject(GameObject target1, GameObject target2)
//        {
//            // Проверяем, находится ли указатель мыши над защищенным объектом
//            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
//            {
//                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
//            };

//            // Список результатов Raycast'а
//            var results = new System.Collections.Generic.List<RaycastResult>();
//            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

//            foreach (var result in results)
//            {
//                if (result.gameObject == target1 || result.gameObject == target2)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        private void OnGlobalClick()
//        {
//            //GameManager.Instance.PopUpRoot.SetActive(false);
//        }
//        public void AddItem(ItemDefinition i)
//        {
//            _collection.TryAdd(i, 1);
//        }
//        public void AddItem(ItemDefinition i, int quantity)
//        {
//            _collection.TryAdd(i, quantity);
//        }
//        public void Remove(ItemDefinition i)
//        {
//            _collection.Remove(i);
//        }
//        public ItemInstance Get(ItemDefinition item)
//        {
//            return _collection.GetItemAt(0);
//        }
//    }
    
//}