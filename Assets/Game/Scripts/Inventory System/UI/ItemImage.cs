//using InventorySystem;
//using UnityEngine;
//using UnityEngine.UI;

//public class ItemImage : MonoBehaviour
//{
//    private Image _image;
//    private ItemSlot _slot;

//    void Start()
//    {
//        _image = GetComponent<Image>();

//        _slot = transform.parent.GetComponentInChildren<ItemSlot>();
//    }

//    void Update()
//    {
//        if (!(_slot.ItemInstance == null))
//        {
//            bool slotFilled = _slot.ItemInstance.ItemDefinition != null;

//            _image.enabled = slotFilled;
//        }
//    }
//}
