using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InventorySystem
{
    public class InventoryHandler : MonoBehaviour
    {
        [SerializeField] private ItemsCollection _itemsCollection;

        [Header("Properties")]
        [Min(0)][SerializeField] private int _inventoryCapacity;

        private UIDragController _dragHandler;

        private GameObject _itemSlotPrefab;

        public ItemsCollection Collection { get => _itemsCollection; set => _itemsCollection = value; }

        [Inject]
        public void Constructor([Inject(Id = "ItemSlot")] GameObject itemSlotPrefab)
        {
            _itemSlotPrefab = itemSlotPrefab;
        }

        private void Start()
        {
            _dragHandler = UIDragController.Instance;

            _dragHandler.OnStartDrag += OnDragStarted;
            _dragHandler.OnEndDrag += OnDragEnd;

            SetRightCollectionCount();
            SetRightSlotsCount();

            _dragHandler = UIDragController.Instance;

            for (int i = 0; i < _itemsCollection.Count; i++)
            {
                ItemSlot slot = transform.GetChild(i).GetComponentInChildren<ItemSlot>();
                slot.ItemsCollection = _itemsCollection;
            }

            //_itemsCollection.ResetAllFlags();
        }

        private void SetRightCollectionCount()
        {
            if (_itemsCollection.Count < _inventoryCapacity)
            {
                int diff = _inventoryCapacity - _itemsCollection.Count;
                _itemsCollection.AddRange(diff);
            }

            if (_itemsCollection.Count > _inventoryCapacity)
            {
                int diff = _itemsCollection.Count - _inventoryCapacity;
                _itemsCollection.RemoveRange(diff);
            }
        }

        private void SetRightSlotsCount()
        {
            int slotsCount = transform.Cast<Transform>()
                    .Count(child => child.GetComponent<ItemSlot>() != null);

            if (slotsCount < _inventoryCapacity)
            {
                int diff = _inventoryCapacity - slotsCount;
                for (int i = 0; i < diff; i++)
                {
                    var newItemSlot = Instantiate(_itemSlotPrefab, transform);
                }
            }
        }


        protected virtual void OnDragStarted(DragEventInfo dragEventInfo)
        {
            if (dragEventInfo.ObjectUnderCursor.transform.GetComponentInParent<InventoryHandler>() != this)
                return;

            dragEventInfo.draggableComponent.GetItemIconRect().GetComponent<Image>().enabled = false;

            ItemInstance itemInstance = _itemsCollection[dragEventInfo.SlotUnderCursorNum];

            if (itemInstance.ItemDefinition == null)
            {
                _dragHandler.IsDragging = false;
                return;
            }

            itemInstance.AddFlag(ItemFlags.IsDragging);

            _dragHandler.SetDraggedSprite(itemInstance.ItemDefinition.Sprite);
            _dragHandler.SetDraggedItem(itemInstance);

        }

        protected virtual void OnDragEnd(DragEventInfo dragEventInfo)
        {
            if (dragEventInfo.ObjectUnderCursor?.transform.GetComponentInParent<InventoryHandler>() != this)
                return;

            ItemInstance draggedItemInstance = dragEventInfo.ItemInstance;

            ItemInstance landedItemInstance = _itemsCollection[dragEventInfo.SlotUnderCursorNum];

            _itemsCollection[dragEventInfo.SlotUnderCursorNum] = draggedItemInstance;

            // Drop to original slot
            if (dragEventInfo.OriginalSlotNum == dragEventInfo.SlotUnderCursorNum && dragEventInfo.SourceItemsCollection == _itemsCollection)
            {
                draggedItemInstance.RemoveFlag(ItemFlags.IsDragging);
                return;
            }

            dragEventInfo.SourceItemsCollection.Remove(dragEventInfo.OriginalSlotNum); // назначили пустой ItemInstance на прошлый слот

            draggedItemInstance.RemoveFlag(ItemFlags.IsDragging);

            return;
        }
    }
}