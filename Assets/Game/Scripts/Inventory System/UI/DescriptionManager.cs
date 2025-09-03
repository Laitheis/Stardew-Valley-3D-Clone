using InventorySystem;
using UnityEngine;


public interface IDescriptionOwner
{
    string GetDescription();
}


public class DescriptionManager : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private DescriptionUI _inventoryDesc;
    [SerializeField] private DescriptionUI _crosshairObjectDesc;
    [SerializeField] private LayerMask _interactableLayerMask;
    [SerializeField] float _maxRayDistance = 5;

    private void Update()
    {
        HandleRaycast();
    }

    private void HandleRaycast()
    {
        if (_mainCamera == null)
        {
            return;
        }

        if (UiDetectUtil.TryGetUIElementUnderCursor(out GameObject result))
        {
            ItemSlot slot = result.GetComponentInParent<ItemSlot>();
            if (slot != null)
            {
                string description = GetDescription(slot);
                _inventoryDesc.Set(description);
                _inventoryDesc.Display(true);
                return;
            }
        }

        _crosshairObjectDesc.Display(false);
        if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out RaycastHit hit, _maxRayDistance, _interactableLayerMask))
        {
            WorldItemController worldItem = hit.transform.GetComponent<WorldItemController>();
            if (worldItem != null)
            {
                string description = GetDescription(worldItem);
                _crosshairObjectDesc.Set(description);
                _crosshairObjectDesc.Display(true);
                return;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _crosshairObjectDesc.Display(false);
        }
    }
    string GetDescription(ItemSlot slot)
    {
        ItemsCollection collection = slot.ItemsCollection;
        ItemDefinition item = collection[slot.numInContainer].ItemDefinition;
        if (item == null)
        {
            return string.Empty;
        }
        else
        {
            return item.Name + "\n" + item.Description;
        }
    }
    string GetDescription(WorldItemController worlditem)
    {
        ItemDefinition item = worlditem.ItemInstance.ItemDefinition;
        if (item == null)
        {
            return string.Empty;
        }
        else
        {
            return item.Name + "\n" + item.Description;
        }
    }
}

