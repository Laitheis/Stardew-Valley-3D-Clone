using System.Linq;
using Unity.VisualScripting;
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
        }

        private void SetRightCollectionCount()
        {
            if (_itemsCollection.Count < _inventoryCapacity)
            {
                int diff = _inventoryCapacity - _itemsCollection.Count;
                _itemsCollection.AddRangeEmpty(diff);
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
            // Validate InvHandler
            if (dragEventInfo.ObjectUnderCursor.transform.GetComponentInParent<InventoryHandler>() != this)
                return;

            ItemInstance itemInstance = _itemsCollection[dragEventInfo.SlotUnderCursorNum];

            itemInstance.AddFlag(ItemFlags.IsDragging);

            _dragHandler.IsDragging = true;

            _dragHandler.SetDraggedSprite(itemInstance.ItemDefinition.Sprite);
            _dragHandler.SetDraggedItem(itemInstance);

            _dragHandler.GetDraggedRect().Find("CountText").GetComponent<TMPro.TextMeshProUGUI>().text = itemInstance.Count.ToString();
        }

        protected virtual void OnDragEnd(DragEventInfo dragEventInfo)
        {
            if (dragEventInfo.ObjectUnderCursor?.transform.GetComponentInParent<InventoryHandler>() != this)
                return;

            ItemInstance draggedItemInstance = dragEventInfo.ItemInstance;

            // If drop to original slot
            if (dragEventInfo.OriginalSlotNum == dragEventInfo.SlotUnderCursorNum && dragEventInfo.SourceItemsCollection == _itemsCollection)
            {
                draggedItemInstance.RemoveFlag(ItemFlags.IsDragging);
                return;
            }

            SetDraggedItem(dragEventInfo.SourceItemsCollection, dragEventInfo.OriginalSlotNum, dragEventInfo.SlotUnderCursorNum);

            return;
        }

        public void SetDraggedItem(ItemsCollection sourceDraggedCollection, int sourceNum, int slotNum = -1)
        {
            if (slotNum == -1)
            {
                // TODO: Implement insertion into the first available free slot
                return;
            }

            ItemInstance dragged = sourceDraggedCollection[sourceNum];
            ItemInstance itemAtSlot = _itemsCollection[slotNum];

            // If slot is empty — move the item directly
            if (itemAtSlot.ItemDefinition == null)
            {
                sourceDraggedCollection.Remove(sourceNum);
                _itemsCollection.AddAt(dragged, slotNum);
                dragged.RemoveFlag(ItemFlags.IsDragging);
                return;
            }

            // If item types differ — swap items
            if (itemAtSlot.ItemDefinition != dragged.ItemDefinition)
            {
                sourceDraggedCollection.Remove(sourceNum);
                ContinueDragging(itemAtSlot);
                _itemsCollection.AddAt(dragged, slotNum);
                dragged.RemoveFlag(ItemFlags.IsDragging);
                return;
            }

            // Try to merge stacks
            int freeSpace = itemAtSlot.ItemDefinition.MaxCountInStack - itemAtSlot.Count;
            int remainingCount = dragged.Count - freeSpace;

            if (remainingCount == 0)
            {
                // Full merge possible
                itemAtSlot.SetCount(itemAtSlot.Count + dragged.Count);
                sourceDraggedCollection.Remove(sourceNum);
            }
            else
            {
                // Partial merge, continue dragging leftovers
                itemAtSlot.SetCount(itemAtSlot.ItemDefinition.MaxCountInStack);
                dragged.SetCount(remainingCount);
                ContinueDragging(dragged);
            }
        }

        void ContinueDragging(ItemInstance itemInstance)
        {
            _dragHandler.IsDragging = true;

            _dragHandler.SetDraggedSprite(itemInstance.ItemDefinition.Sprite);
            _dragHandler.SetDraggedItem(itemInstance);
            _dragHandler.GetDraggedRect().Find("CountText").GetComponent<TMPro.TextMeshProUGUI>().text = itemInstance.Count.ToString();
        }
    }
}