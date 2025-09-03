using UI.Dragging;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InventorySystem
{
    public class PlayerInventoryManager : MonoBehaviour
    {
        public static PlayerInventoryManager Instance;

        [SerializeField] ItemsCollection _playerItemsCollection;
        [SerializeField] Transform _playerInventroyContainer;
        //[SerializeField] RectTransform _mainContainer;

        private ItemInstance _draggedItem;
        private RectTransform _draggedImage;

        private UIDragHandler _dragHandler;

        //public bool Enabled => _mainContainer.gameObject.activeSelf;

        public ItemsCollection Collection { get => _playerItemsCollection; set => _playerItemsCollection = value; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _dragHandler = UIDragHandler.Instance;

            _dragHandler.OnStartDrag += OnDragStarted;
            _dragHandler.OnEndDrag += OnDragEnd;
        }

        //public void Display(bool value)
        //{
        //    _mainContainer.gameObject.SetActive(value);
        //}

        protected virtual void OnDragStarted(DragEventInfo dragEventInfo)
        {
            ItemInstance ItemInstance;

            IUIDraggable draggableComponent = dragEventInfo.objectUnderCursor.GetComponent<IUIDraggable>();
            if (draggableComponent != null)
            {
                draggableComponent.GetRect().GetComponent<Image>().sprite = null;
            }

            if (dragEventInfo.objectUnderCursor.transform.parent.tag == "InventorySlot")
            {
                ItemInstance = _playerItemsCollection[dragEventInfo.slotUnderCursorNum];
                if (ItemInstance.ItemDefinition == null)
                {
                    _dragHandler.IsDragging = false;
                    return;
                }
                _dragHandler.SetDraggedSprite(ItemInstance.ItemDefinition.Sprite);
                _dragHandler.SetDraggedItem(ItemInstance);
                _playerItemsCollection.Remove(ItemInstance);

            }
        }
        protected virtual void OnDragEnd(DragEventInfo dragEventInfo)
        {
            if (dragEventInfo.objectUnderCursor == null)
            {
                goto noDropOnInventory;
            }

            ItemInstance draggedItemInstance = dragEventInfo.ItemInstance;
            ItemInstance landedItemInstance = null;

            if (draggedItemInstance != null)
            {
                if (dragEventInfo.objectUnderCursor.transform.parent.tag == "InventorySlot")
                {
                    landedItemInstance = _playerItemsCollection[dragEventInfo.slotUnderCursorNum];

                    _playerItemsCollection[dragEventInfo.slotUnderCursorNum] = draggedItemInstance;
                }
                else
                {
                    goto noDropOnInventory;
                }

                if (landedItemInstance != null && landedItemInstance.ItemDefinition != null)
                {
                    _dragHandler.SetDraggedSprite(landedItemInstance.ItemDefinition.Sprite);
                    _dragHandler.SetDraggedItem(landedItemInstance);
                }

            }
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