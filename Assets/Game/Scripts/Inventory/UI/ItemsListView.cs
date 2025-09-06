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

                if(itemInstance == null || itemInstance.ItemDefinition == null || itemInstance.HasFlag(ItemFlags.IsDragging))
                {
                    if (i >= transform.childCount) return;

                    var child = transform.GetChild(i);
                    var itemIconGO = child.Find("Item");
                    var image = itemIconGO.GetComponent<Image>();
                    image.enabled = false;
                }
                else
                {
                    Sprite sprite = itemInstance.ItemDefinition.Sprite;

                    var icon = transform.GetChild(i).GetComponentInChildren<ItemSlot>().ItemImage;
                    icon.sprite = sprite;
                    icon.enabled = true;
                }

                //Debug.Log($"ֿנוהלוע ג סכמעו {i} טללוע פכאד {_collection.GetDraggingFlag(i)}");
            }
        }
        //int FirstValidSlot(ItemDefinition item, int quantity)
        //{
        //    return _collection.GetFirstValidSlot(item, quantity);
        //}
        //bool AbleToAddItem(ItemDefinition item, int slotNum)
        //{
        //    return _collection.CanAdd(item, 1, slotNum);
        //}
        //void OnItemSlotClicked(ItemSlotEvent e)
        //{

        //}
        //public void SetItemCollection(ItemsCollection c)
        //{
        //    Collection = c;
        //}
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
