using System.Linq;
using UnityEngine;
using Zenject;

namespace InventorySystem
{
    public class InventoryHandler : MonoBehaviour
    {
        [SerializeField] private ItemsCollection _itemsCollection;

        [Header("Properties")]
        [Min(0)][SerializeField] private int _inventoryCapacity;

        private UIDragController _dragController;

        private GameObject _itemSlotPrefab;

        private DiContainer _container;

        public ItemsCollection Collection { get => _itemsCollection; set => _itemsCollection = value; }

        [Inject]
        public void Constructor([Inject(Id = "ItemSlot")] GameObject itemSlotPrefab, DiContainer container)
        {
            _itemSlotPrefab = itemSlotPrefab;
            _container = container;
        }

        private void Start()
        {
            _dragController = UIDragController.Instance;

            _dragController.OnStartDrag += OnDragStarted;
            _dragController.OnEndDrag += OnDragEnd;

            SetRightCollectionCount();
            SetRightSlotsCount();

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
                    var newItemSlot = _container.InstantiatePrefab(_itemSlotPrefab, transform);
                }
            }
        }

        protected virtual void OnDragStarted(DragEventInfo dragEventInfo)
        {
            // Validate InvHandler
            if (dragEventInfo.ObjectUnderCursor.transform.GetComponentInParent<InventoryHandler>() != this)
                return;

            ItemInstance origItemInst = dragEventInfo.ItemInstance;
            ItemInstance draggedItemInst = null;

            // Shift — fast dragging
            if (_dragController.IsShiftHeld())
            {
                var allInventories = FindObjectsOfType<InventoryHandler>()
                    .Where(inv => inv != this)
                    .ToList();

                foreach (var inv in allInventories)
                {
                    int overflow = inv.Collection.AddWithOverflow(dragEventInfo.ItemInstance, -1, dragEventInfo.ItemInstance.Count);

                    if (overflow == -1) continue;

                    if (overflow == 0) dragEventInfo.SourceItemsCollection.Remove(dragEventInfo.ItemInstance);

                    if (overflow > 0)
                    {
                        dragEventInfo.ItemInstance.SetCount(overflow);
                        _dragController.IsDragging = false;
                        return;
                    }
                }
                _dragController.IsDragging = false;
                return;
            }

            // Right-click - take half
            if (_dragController.GetMouseButton() == 1)
            {
                int takeCount = origItemInst.ItemDefinition.MaxCountInStack == 1 ? 1 : Mathf.FloorToInt(origItemInst.Count / 2f);
                if (takeCount < 1) takeCount = 1;

                ItemInstance splitInstance = new ItemInstance(origItemInst.ItemDefinition, takeCount);
                origItemInst.RemoveCount(takeCount, out _);
                if (origItemInst.Count == 0) dragEventInfo.SourceItemsCollection.Remove(origItemInst);
                draggedItemInst = splitInstance;
            }
            else
            {
                draggedItemInst = origItemInst;
                dragEventInfo.SourceItemsCollection.Remove(origItemInst);
            }

            _dragController.IsDragging = true;

            _dragController.SetDraggedSprite(draggedItemInst.ItemDefinition.Sprite);
            _dragController.SetDraggedItem(draggedItemInst);
        }

        protected virtual void OnDragEnd(DragEventInfo dragEventInfo)
        {
            if (dragEventInfo.ObjectUnderCursor?.transform.GetComponentInParent<InventoryHandler>() != this)
                return;

            ItemInstance draggedItemInstance = dragEventInfo.ItemInstance;

            SetDraggedItem(draggedItemInstance, dragEventInfo.SourceItemsCollection, dragEventInfo.SlotUnderCursorNum);
        }

        public void SetDraggedItem(ItemInstance sourceItem, ItemsCollection sourceDraggedCollection, int slotNum = -1)
        {
            var targetItem = _itemsCollection[slotNum];

            // Swap items
            if (targetItem.ItemDefinition != null && targetItem.ItemDefinition != sourceItem.ItemDefinition)
            {
                _dragController.OriginalSlotNum = slotNum;

                _itemsCollection.Remove(targetItem);
                _itemsCollection.AddAt(sourceItem, slotNum);

                ContinueDragging(targetItem);
                return;
            }

            // RMB — lay out one by one
            if (_dragController.GetMouseButton() == 1)
            {
                if (sourceItem.Count > 1)
                {
                    sourceItem.SetCount(sourceItem.Count - 1);
                    _itemsCollection.AddAtWithCount(sourceItem, slotNum, 1);
                    ContinueDragging(sourceItem);
                }
                else // Count == 1
                {
                    _itemsCollection.AddAt(sourceItem, slotNum);
                }
                return;
            }

            // Attempt to merge stacks
            int overflow = _itemsCollection.AddWithOverflow(sourceItem, slotNum, sourceItem.Count);

            if (overflow <= 0) return;

            // There is some left - continue to drag
            sourceItem.SetCount(overflow);
            _dragController.OriginalSlotNum = slotNum;
            ContinueDragging(sourceItem);
        }


        void ContinueDragging(ItemInstance itemInstance)
        {
            _dragController.IsCountinueDragging = true;
            _dragController.IsDragging = true;

            _dragController.SetDraggedSprite(itemInstance.ItemDefinition.Sprite);
            _dragController.SetDraggedItem(itemInstance);
        }
    }
}