using UnityEngine;


public class SelectedSlotController : MonoBehaviour
{
    [SerializeField] private InventoryHandler _inventory;
    [SerializeField] private RectTransform _selectionFrame;

    private int _selectedSlotNum = 0;

    public int SelectedSlotNum
    {
        get => _selectedSlotNum;
        set => _selectedSlotNum = Mathf.Clamp(value, 0, _inventory.ItemsCollection.Count - 1);
    }
    public RectTransform SelectionFrame { get => _selectionFrame; set => _selectionFrame = value; }

    private void Update()
    {
        Vector2 scroll = Input.mouseScrollDelta;

        if (scroll.y > 0)
        {
            SelectedSlotNum--;
        }
        if (scroll.y < 0)
        {
            SelectedSlotNum++;
        }

        SelectionFrame.position = _inventory.transform.GetChild(_selectedSlotNum).transform.position;
    }
}