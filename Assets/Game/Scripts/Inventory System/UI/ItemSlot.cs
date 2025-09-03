using UI.Dragging;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem
{
    public class ItemSlot : MonoBehaviour, IUIDraggable, IDragLandable
    {
        public event Action<ItemSlotEvent> onUIEvent;

        [SerializeField] GameObject _highlightImage;
        [SerializeField] GameObject _selectImage;

        [HideInInspector] public ItemsCollection ItemsCollection { get; set; }

        public Image ItemImage;

        public object Info;

        public RectTransform RectTransform => GetComponent<RectTransform>();
        public ItemInstance ItemInstance => ItemsCollection[numInContainer];
        public bool Highlighted => _highlightImage.activeSelf;
        public bool Selected => _selectImage.activeSelf;
        public int numInContainer => transform.parent.GetSiblingIndex();

        public bool AbleToPickUp()
        {
            return true;
        }

        public void OnStartDrag() {  }

        public void OnEndDrag(bool success) {   }

        public RectTransform GetDraggedRect() => null;

        public bool AbleToDrag() => true;

        public virtual bool AbleToLanding(RectTransform r, object info) => true;

        public void OnLanding(RectTransform r, object info) { }

        public int GetHierarchyIndex() => transform.parent.GetSiblingIndex();

        public RectTransform GetRect()
        {
            return ItemImage.GetComponent<RectTransform>();
        }

        public object GetInfo()
        {
            return (ItemInstance, numInContainer);
        }

        public void OnPointerDown(PointerEventData e)
        {
            onUIEvent?.Invoke(new() {
                slot = this,
                slotInfo = Info,
                shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift),
                mouseDown = true,
                pointerEventData = e
            });
        }
        
        public void Set(Sprite sprite)
        {
            ItemImage.sprite = sprite;
        }

        public void Highlight(bool v)  
        {
            _highlightImage?.SetActive(v);
        }

        public void DisplaySelectFrame(bool v)
        {
            _selectImage?.SetActive(v);
        }
    }


    public class ItemSlotEvent
    {
        public int slotNum => slot.numInContainer;
        public bool shiftPressed = false;
        public bool mouseDown = false;
        public ItemSlot slot;
        public object slotInfo;
        public PointerEventData pointerEventData;
    }
}
