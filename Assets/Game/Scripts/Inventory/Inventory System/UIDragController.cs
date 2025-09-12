using InventorySystem;
using System;
using UI.Dragging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;


public class UIDragController : MonoBehaviour
{
    public event Action<DragEventInfo> OnDrag;
    public event Action<DragEventInfo> OnStartDrag;
    public event Action<DragEventInfo> OnEndDrag;

    private Image _draggedImagePrefab;

    private float _draggedImageHeight = 100;
    private float _draggedImageWidth = 100;
    private Transform _canvasRoot;

    private bool _destroyImageObjectOnEnd = true;

    // DragEvent Data
    private int _slotUnderCursorNum;
    private int _originalSlotNum;
    private RectTransform _draggedRect;
    private GameObject _objectUnderCursor;
    private ItemsCollection _sourceItemsCollection;
    private ItemInstance _itemInstance;

    private bool _isDragging;
    private bool _isCountinueDragging;

    private int _mouseButton;

    private GameObject _player;
    [Inject] private SignalBus _signalBus;

    internal static UIDragController Instance;

    public bool IsDragging { get => _isDragging; set => _isDragging = value; }
    public int OriginalSlotNum { get => _originalSlotNum; set => _originalSlotNum = value; }
    public bool IsCountinueDragging { get => _isCountinueDragging; set => _isCountinueDragging = value; }

    private void Awake()
    {
        Instance = this;
    }

    [Inject]
    private void Constructor([Inject(Id = "DraggedImagePrefab")] Image draggedImagePrefab, [Inject(Id = "Player")] GameObject player, Canvas mainCanvas)
    {
        _draggedImagePrefab = draggedImagePrefab;
        _player = player;
        _canvasRoot = mainCanvas.transform;
    }

    private void Update()
    {
        HandleInput();

        if (_isDragging && _draggedRect != null)
        {
            DragUpdate();
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            _mouseButton = Input.GetMouseButtonDown(0) ? 0 : 1;

            if (_isDragging)
            {
                TryLandDraggedObject();
            }
            else
            {
                TryStartDragging();
            }
        }
    }

    private void TryStartDragging()
    {
        IUIDraggable draggable = null;
        UiDetectUtil.TryGetUIElementUnderCursor(out GameObject clickedObject);
        clickedObject?.transform.parent.TryGetComponent(out draggable);
        if (draggable == null) return;

        _slotUnderCursorNum = draggable.GetHierarchyIndex();
        _objectUnderCursor = clickedObject;
        _sourceItemsCollection = clickedObject.transform.GetComponentInParent<ItemsCollection>();
        OriginalSlotNum = _slotUnderCursorNum;

        var item = _sourceItemsCollection[OriginalSlotNum];
        if (item.ItemDefinition == null) return;

        _isDragging = true;
        OnStartDrag?.Invoke(new()
        {
            ItemInstance = item,
            DraggedRect = _draggedRect,
            ObjectUnderCursor = _objectUnderCursor,
            SlotUnderCursorNum = _slotUnderCursorNum,
            SourceItemsCollection = _sourceItemsCollection,
            draggableComponent = draggable
        });
    }

    private void TryLandDraggedObject()
    {
        IDragLandable landable = null;
        UiDetectUtil.TryGetUIElementUnderCursor(out GameObject targetObject);
        targetObject?.transform.parent.TryGetComponent(out landable);

        if (landable == null)
        {
            if (_mouseButton == 0 || _itemInstance.Count == 1)
            {
                _signalBus.Fire(new ItemDropEvent(_player.transform.position, _itemInstance));
                _isDragging = false;
                if (_destroyImageObjectOnEnd && _draggedRect != null) 
                    Destroy(_draggedRect.gameObject);
            }
            else if (_mouseButton == 1 || _itemInstance.Count > 1)
            {
                _itemInstance.SetCount(_itemInstance.Count - 1);
                var droppedItemInst = new ItemInstance(_itemInstance.ItemDefinition, 1);
                _signalBus.Fire(new ItemDropEvent(_player.transform.position, droppedItemInst));
            }
            return;
        }

        _slotUnderCursorNum = landable.GetHierarchyIndex();
        _isDragging = false;
        if (_destroyImageObjectOnEnd && _draggedRect != null) Destroy(_draggedRect.gameObject);

        OnEndDrag?.Invoke(new()
        {
            DraggedRect = _draggedRect,
            ItemInstance = _itemInstance,
            ObjectUnderCursor = targetObject,
            SlotUnderCursorNum = _slotUnderCursorNum,
            SourceItemsCollection = _sourceItemsCollection,
            OriginalSlotNum = OriginalSlotNum,
            landableComponent = landable
        });
    }

    public int GetMouseButton() => _mouseButton;
    public bool IsShiftHeld() => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

    private void DragUpdate()
    {
        Vector2 cursorPosition = Input.mousePosition;
        if (_draggedRect != null)
        {
            _draggedRect.position = cursorPosition;
            SetDispDraggedCount();
        }

        OnDrag?.Invoke(null);
    }

    public void SetDraggedItem(ItemInstance itemInstance)
    {
        _isDragging = true;
        _itemInstance = itemInstance;
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

    public void SetDispDraggedCount()
    {
        GetDraggedRect().Find("CountText").GetComponent<TMPro.TextMeshProUGUI>().text = _itemInstance.Count.ToString();
    }
}


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

