//using InventorySystem;
//using UnityEngine;
//using Zenject;

//public class WorldItemController : MonoBehaviour
//{
//    [SerializeField] private ItemInstance _itemInstance;
//    private ItemDatabase _itemDatabase;
//    public ItemInstance ItemInstance { get => _itemInstance; set => _itemInstance = value; }
//    public float unpickableTime;
//    public bool pickable;
//    public GameObject rootObject;

//    [Inject]
//    public void Constructor(ItemDatabase itemDatabase)
//    {
//        _itemDatabase = itemDatabase;
//    }

//    private void Start()
//    {
//        if (_itemInstance.ItemDefinition == null)
//        {
//            _itemInstance = new(_itemDatabase.GetItemAt(0), 1);
//        }
//        else
//        {
//            _itemInstance.SetCount(1);
//        }
//        pickable = false;
//        Invoke("SetPickable", unpickableTime);
//    }


//    public void ResetUnpickableTime()
//    {
//        pickable = false;
//        Invoke("SetPickable", unpickableTime);
//    }
//    void SetPickable()
//    {
//        pickable = true;
//    }
//    private void OnTriggerStay(Collider other)
//    {
//        if (pickable)
//        {
//            if (other.transform.tag == "Player")
//            {
//                //bool added = PlayerManager.CharacterStatic.itemsCollection.TryAdd(ItemInstance);
//                //if (added)
//                //{
//                Destroy(rootObject);
//                //}
//            }
//        }
//    }

//}
