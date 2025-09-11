using UI.Dragging;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem
{
    public class ItemSlot : MonoBehaviour, IUIDraggable, IDragLandable
    {
        [HideInInspector] public ItemsCollection ItemsCollection;

        public Image ItemImage;
        public Image ItemBG;
        public TMPro.TextMeshProUGUI CountText;
        public Image CountBG;

        public ItemInstance ItemInstance => ItemsCollection[NumInContainer];
        public int NumInContainer => transform.GetSiblingIndex();

        public void OnStartDrag() {  }

        public void OnEndDrag(bool success) {   }

        public bool AbleToDrag() => true;

        public virtual bool AbleToLanding(RectTransform r, object info) => true;

        public void OnLanding(RectTransform r, object info) { }

        public int GetHierarchyIndex() => transform.GetSiblingIndex();

        public RectTransform GetItemIconRect()
        {
            return ItemImage.GetComponent<RectTransform>();
        }

        public object GetInfo()
        {
            return (ItemInstance, NumInContainer);
        }
    }
}
