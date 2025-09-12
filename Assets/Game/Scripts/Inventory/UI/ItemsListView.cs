using UI.Dragging;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InventorySystem
{
    public class ItemsListView : MonoBehaviour
    {
        [SerializeField] protected ItemsCollection _collection;

        protected UIDragController _dragHandler;

        void Update()
        {
            FillByCollection();
        }
        
        void FillByCollection()
        {
            for (int i = 0; i < _collection.Count; i++)
            {
                ItemInstance itemInstance = _collection[i];

                if(itemInstance == null || itemInstance.ItemDefinition == null)
                {
                    if (i >= transform.childCount) return;

                    var itemSlot = transform.GetChild(i).GetComponentInChildren<ItemSlot>();
                    itemSlot.ItemImage.enabled = false;
                    itemSlot.CountBG.enabled = false;
                    itemSlot.CountText.enabled = false;
                }
                else
                {
                    Sprite sprite = itemInstance.ItemDefinition.Sprite;

                    var itemSlot = transform.GetChild(i).GetComponentInChildren<ItemSlot>();
                    itemSlot.ItemImage.sprite = sprite;

                    itemSlot.ItemImage.enabled = true;
                    itemSlot.CountBG.enabled = true;
                    itemSlot.CountText.enabled = true;

                    itemSlot.CountText.text = itemInstance.Count.ToString();
                }
            }
        }
    }
}
