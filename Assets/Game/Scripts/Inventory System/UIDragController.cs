using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DragEventInfo
{
    public int SlotUnderCursorNum;
    public int OriginalSlotNum;

    public ItemsCollection SourceItemsCollection;
    public ItemInstance ItemInstance;
    public GameObject ObjectUnderCursor;
    public RectTransform DraggedRect;
    public IUIDraggable draggableComponent;
    public IDragLandable landableComponent;
}

public class UIDragController : MonoBehaviour, IClickConsumer
{
    public event Action<DragEventInfo> OnDrag;
    public event Action<DragEventInfo> OnStartDrag;
    public event Action<DragEventInfo> OnEndDrag;

    [SerializeField] Camera _camera;

    [Inject] private InputHandler _inputHandler;

    private Image _draggedImagePrefab;

    private float _draggedImageHeight = 100;
    private float _draggedImageWidth = 100;
    private Transform _canvasRoot;

    // DragEvent Data
    private int _slotUnderCursorNum;
    private int _originalSlotNum;
    private RectTransform _draggedRect;
    private GameObject _objectUnderCursor;
    private ItemsCollection _sourceItemsCollection;
    private ItemInstance _itemInstance;

    private bool _isDragging;
    private bool _isCountinueDragging;
    private bool _isMouseOverTraderPanel;

    private int _mouseButton;

    private GameObject _player;
    [Inject] private SignalBus _signalBus;

    public int OriginalSlotNum { get => _originalSlotNum; set => _originalSlotNum = value; }
    public bool IsCountinueDragging { get => _isCountinueDragging; set => _isCountinueDragging = value; }
    public ItemInstance ItemInstance { get => _itemInstance; set => _itemInstance = value; }
    public bool IsMouseOverTraderPanel { get => _isMouseOverTraderPanel; set => _isMouseOverTraderPanel = value; }
    public bool IsDragging { get => _isDragging; set { if (value != _isDragging) { _isDragging = value; } } }

    public int ClickPriority => 50;

    void OnEnable() => _inputHandler.RegisterConsumer(this);
    void OnDisable() => _inputHandler.UnregisterConsumer(this);

    [Inject]
    private void Constructor([Inject(Id = "DraggedImagePrefab")] Image draggedImagePrefab, [Inject(Id = "Player")] GameObject player, Canvas mainCanvas)
    {
        _draggedImagePrefab = draggedImagePrefab;
        _player = player;
        _canvasRoot = mainCanvas.transform;
    }

    private void Update()
    {
        if (IsDragging && _draggedRect != null)
        {
            DragUpdate();
        }

        ClearItemInstance();
    }

    public bool OnClick()
    {
        return HandleInput(true);
    }

    public bool OnRightClick()
    {
        return HandleInput(false);
    }

    public void OnEndClick()
    {
        //
    }

    public bool OnHold()
    {
        return false;
    }

    private void ClearItemInstance()
    {
        if (!IsDragging)
            _itemInstance = null;
    }

    private bool HandleInput(bool isMouseButton0)
    {
        _mouseButton = isMouseButton0 ? 0 : 1;

        if (IsDragging)
        {
            return TryLandDraggedObject();
        }
        else
        {
            return TryStartDragging();
        }
    }

    private bool TryStartDragging()
    {
        IUIDraggable draggable = null;
        UiDetectUtil.TryGetUIElementUnderCursor(out GameObject clickedObject);
        clickedObject?.transform.parent.TryGetComponent(out draggable);
        if (draggable == null) return false;

        _slotUnderCursorNum = draggable.GetHierarchyIndex();
        _objectUnderCursor = clickedObject;
        _sourceItemsCollection = clickedObject.transform.GetComponentInParent<ItemsCollection>();
        OriginalSlotNum = _slotUnderCursorNum;

        var item = _sourceItemsCollection[OriginalSlotNum];
        if (item.ItemDefinition == null) return false;

        IsDragging = true;
        OnStartDrag?.Invoke(new()
        {
            ItemInstance = item,
            DraggedRect = _draggedRect,
            ObjectUnderCursor = _objectUnderCursor,
            SlotUnderCursorNum = _slotUnderCursorNum,
            SourceItemsCollection = _sourceItemsCollection,
            draggableComponent = draggable
        });
        return true;
    }

    private bool TryLandDraggedObject()
    {
        IDragLandable landable = null;
        UiDetectUtil.TryGetUIElementUnderCursor(out GameObject targetObject);
        targetObject?.transform.parent.TryGetComponent(out landable);

        if (landable == null)
        {
            if (_isMouseOverTraderPanel) return false;

            if (_mouseButton == 0 || ItemInstance.Count == 1)
            { // Left mouse button - drop full item
                _signalBus.Fire(new ItemDropEvent(_player.transform.position, ItemInstance, true));
                IsDragging = false;
                if (_draggedRect != null)
                    ClearDraggedRect();
                return true;
            }
            else if (_mouseButton == 1)
            { // Right mouse button - drop one unit of an item
                ItemInstance.SetCount(ItemInstance.Count - 1);
                var droppedItemInst = new ItemInstance(ItemInstance.ItemDefinition, 1);
                _signalBus.Fire(new ItemDropEvent(_player.transform.position, droppedItemInst, true));
                return true;
            }

            return false;
        }

        _slotUnderCursorNum = landable.GetHierarchyIndex();
        IsDragging = false;

        ClearDraggedRect();

        OnEndDrag?.Invoke(new()
        {
            DraggedRect = _draggedRect,
            ItemInstance = ItemInstance,
            ObjectUnderCursor = targetObject,
            SlotUnderCursorNum = _slotUnderCursorNum,
            SourceItemsCollection = _sourceItemsCollection,
            OriginalSlotNum = OriginalSlotNum,
            landableComponent = landable
        });

        return true;
    }

    public int GetMouseButton() => _mouseButton;
    public bool IsShiftHeld() => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

    private void DragUpdate()
    {
        Vector3 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
        if (_draggedRect != null)
        {
            _draggedRect.transform.position = pos;
            _draggedRect.transform.localPosition = new(_draggedRect.transform.localPosition.x, _draggedRect.transform.localPosition.y, 0);
            SetDispDraggedCount();
        }

        OnDrag?.Invoke(null);
    }

    public void SetDraggedItem(ItemInstance itemInstance)
    {
        IsDragging = true;
        ItemInstance = itemInstance;
    }

    public void SetDraggedSprite(Sprite sprite)
    {
        Image image = Instantiate(_draggedImagePrefab, _canvasRoot);

        _draggedRect = image.rectTransform;
        image.sprite = sprite;

        _draggedRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _draggedImageWidth);
        _draggedRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _draggedImageHeight);
    }

    public Transform GetDraggedRect()
    {
        return _draggedRect.transform;
    }

    public void ClearDraggedRect()
    {
        Destroy(_draggedRect.gameObject);
    }

    public void SetDispDraggedCount()
    {
        GetDraggedRect().Find("CountText").GetComponent<TMPro.TextMeshProUGUI>().text = ItemInstance.Count.ToString();
    }

    public void SetMouseEnterTraderPanelFlag()
    {
        _isMouseOverTraderPanel = true;
    }

    public void SetMouseExitTraderPanelFlag()
    {
        _isMouseOverTraderPanel = false;
    }
}