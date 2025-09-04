using UI.Dragging;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] ItemsCollection _itemsCollection;

        private UIDragHandler _dragHandler;

        public ItemsCollection Collection { get => _itemsCollection; set => _itemsCollection = value; }

        private void Start()
        {
            _dragHandler = UIDragHandler.Instance;

            _dragHandler.OnStartDrag += OnDragStarted;
            _dragHandler.OnEndDrag += OnDragEnd;
        }

        protected virtual void OnDragStarted(DragEventInfo dragEventInfo)
        {
            if (!(dragEventInfo.ObjectUnderCursor.transform.GetComponentInParent<InventoryManager>() == this))
                return;

            ItemInstance itemInstance;

            IUIDraggable draggableComponent = dragEventInfo.ObjectUnderCursor.GetComponent<IUIDraggable>();
            if (draggableComponent != null)
            {
                draggableComponent.GetRect().GetComponent<Image>().enabled = false; //.sprite = null;
            }

            if (dragEventInfo.ObjectUnderCursor.transform.parent.tag == "InventorySlot")
            {
                itemInstance = _itemsCollection[dragEventInfo.SlotUnderCursorNum];
                if (itemInstance.ItemDefinition == null)
                {
                    _dragHandler.IsDragging = false;
                    return;
                }
                _itemsCollection.SetDragging(_itemsCollection.FindIndex(itemInstance), true);

                _dragHandler.SetDraggedSprite(itemInstance.ItemDefinition.Sprite);
                _dragHandler.SetDraggedItem(itemInstance);

                //_playerItemsCollection.Remove(ItemInstance);
            }
        }
        protected virtual void OnDragEnd(DragEventInfo dragEventInfo)
        {
            if (!(dragEventInfo.ObjectUnderCursor.transform.GetComponentInParent<InventoryManager>() == this))
                return;

            if (dragEventInfo.ObjectUnderCursor == null)
            {
                goto noDropOnInventory;
            }

            ItemInstance landedItemInstance;

            ItemInstance draggedItemInstance = dragEventInfo.ItemInstance;

            if (dragEventInfo.ObjectUnderCursor.transform.parent.tag == "InventorySlot")
            {
                landedItemInstance = _itemsCollection[dragEventInfo.SlotUnderCursorNum];

                _itemsCollection[dragEventInfo.SlotUnderCursorNum] = draggedItemInstance;
            }
            else
            {
                goto noDropOnInventory;
            }

            if (dragEventInfo.OriginalSlotNum == dragEventInfo.SlotUnderCursorNum && dragEventInfo.SourceItemsCollection == _itemsCollection)
                return;

            dragEventInfo.SourceItemsCollection.Remove(dragEventInfo.OriginalSlotNum); // сделали пустым прошлый ItemInstance

            //Убрали флаг перетаскивания с прошлого ItemInstance элемента
            dragEventInfo.SourceItemsCollection.SetDragging(dragEventInfo.SourceItemsCollection.FindIndex(draggedItemInstance), false);


            //_dragHandler.SetDraggedSprite(landedItemInstance.ItemDefinition.Sprite);
            //_dragHandler.SetDraggedItem(landedItemInstance);

            return;

        noDropOnInventory:
            Vector3 pos = Vector3.zero;
            //PlayerManager.CharacterStatic.rootTransform;
            //WorldItemController itemController = 
            Instantiate(dragEventInfo.ItemInstance.ItemDefinition.Prefab, pos, Quaternion.identity);
            //.GetComponentInChildren<WorldItemController>();
            //itemController.rootObject.gameObject.AddComponent<BallisticMover>();
            //itemController.ItemInstance = e.ItemInstance;

        }
    }
}