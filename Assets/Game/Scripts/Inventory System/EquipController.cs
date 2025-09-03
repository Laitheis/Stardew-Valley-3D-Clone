using UnityEngine;
using UnityEngine.EventSystems;

namespace InventorySystem
{

    public class EquipController : MonoBehaviour
    {
        public ItemDefinition WeaponItem { get; set; }

        [SerializeField] KeyCode _openKey;

        [SerializeField] GameObject itemPopUpWindow;
        [SerializeField] GameObject ButtonPopUpWindow;
        [SerializeField] GameObject PopUpRoot;

        ItemsCollection _collection;
        private void Start()
        {
            _collection = GetComponent<ItemsCollection>();
        }
        void Update()
        {
            if (Input.GetKeyDown(_openKey))
            {
                
                //var inventory = GameObject.Find("Inventory Root");
                //if (inventory != null)
                //{
                //    if (inventory.transform.GetChild(0).gameObject.activeSelf)
                //    {
                //        inventory.transform.GetChild(0).gameObject.SetActive(false);
                //        Time.timeScale = 1f;
                //        Cursor.lockState = CursorLockMode.Locked;
                //        Cursor.visible = false;
                //        Camera.main.GetComponent<ThirdPersonCamera>().enabled = true;
                //    }
                //    else
                //    {
                //        inventory.transform.GetChild(0).gameObject.SetActive(true);
                //        Time.timeScale = 0f;
                //        Cursor.lockState = CursorLockMode.None;
                //        Cursor.visible = true;
                //        Camera.main.GetComponent<ThirdPersonCamera>().enabled = false;
                //    }
                //}
                //else
                //{
                //    Debug.Log("Ошибка. Не удалось найти инвентарь");
                //}
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsPointerOverUIObject(itemPopUpWindow, ButtonPopUpWindow))
                {
                    OnGlobalClick();
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!IsPointerOverUIObject(itemPopUpWindow, ButtonPopUpWindow))
                {
                    OnGlobalClick();
                }
            }
        }
        private bool IsPointerOverUIObject(GameObject target1, GameObject target2)
        {
            // Проверяем, находится ли указатель мыши над защищенным объектом
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };

            // Список результатов Raycast'а
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            foreach (var result in results)
            {
                if (result.gameObject == target1 || result.gameObject == target2)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnGlobalClick()
        {
            //GameManager.Instance.PopUpRoot.SetActive(false);
        }
        public void AddItem(ItemDefinition i)
        {
            _collection.TryAdd(i, 1);
        }
        public void Remove(ItemDefinition i)
        {
            _collection.Remove(i);
        }
        public void EquipWeapon(ItemDefinition weapon)
        {

        }
    }
}