using UI.Dragging;
using UnityEngine;
using Zenject;

namespace InventorySystem
{
    public class ItemsListView : MonoBehaviour
    {
        [SerializeField] protected ItemsCollection _collection;
        [SerializeField] protected Transform _container;

        protected UIDragHandler _dragHandler;
        public ItemsCollection Collection { get; set; }

        protected ItemDefinition _draggedItem;
        protected RectTransform _draggedImage;

        private void Start()
        {
            _dragHandler = UIDragHandler.Instance;
            FillByCollection();
            for (int i = 0; i < _collection.Count; i++)
            {
                ItemSlot slot = _container.GetChild(i).GetComponentInChildren<ItemSlot>();
                slot.ItemsCollection = _collection;
            }

            _collection.onChange += FillByCollection;
        }
        
        void Update()
        {
            FillByCollection();
        }
        
        void FillByCollection()
        {
            for (int i = 0; i < _collection.Count; i++)
            {
                ItemInstance itemInstance = _collection[i];
                if (itemInstance.ItemDefinition == null)
                {
                    _container.GetChild(i).GetComponentInChildren<ItemSlot>().ItemImage.sprite = null;
                }
                else
                {
                    Sprite sprite = itemInstance.ItemDefinition.Sprite;
                    _container.GetChild(i).GetComponentInChildren<ItemSlot>().ItemImage.sprite = sprite;
                }
            }
        }
        int FirstValidSlot(ItemDefinition item, int quantity)
        {
            return _collection.GetFirstValidSlot(item, quantity);
        }
        bool AbleToAddItem(ItemDefinition item, int slotNum)
        {
            return _collection.CanAdd(item, 1, slotNum);
        }
        void OnItemSlotClicked(ItemSlotEvent e)
        {

        }
        public void SetItemCollection(ItemsCollection c)
        {
            Collection = c;
        }
        //public void Display(bool v)
        //{
        //    for (int i = 0; i < _container.childCount; i++)
        //    {
        //        if (i >= Collection.Count)
        //        {
        //            break;
        //        }
        //        _container.GetChild(i).GetComponent<ItemSlot>().Set(Collection.GetItemAt(i).ItemDefinition.Sprite);
        //    }
        //}
    }
}
