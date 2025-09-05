using InventorySystem;
using System;
using UI.Dragging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using static UnityEditor.PlayerSettings;


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

    internal static UIDragController Instance;

    public bool IsDragging { get => _isDragging; set => _isDragging = value; }

    private void Awake()
    {
        Instance = this;
    }

    [Inject]
    private void Constructor([Inject(Id = "DraggedImagePrefab")] Image draggedImagePrefab, Canvas mainCanvas)
    {
        _draggedImagePrefab = draggedImagePrefab;
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
        if (Input.GetMouseButtonDown(0))
        {
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

        if (draggable == null) 
            return;

        _isDragging = true;

        _slotUnderCursorNum = draggable.GetHierarchyIndex();
        _objectUnderCursor = clickedObject;

        _sourceItemsCollection = clickedObject.transform.GetComponentInParent<ItemsCollection>();

        _originalSlotNum = _slotUnderCursorNum;

        OnStartDrag?.Invoke(new() { DraggedRect = _draggedRect, ObjectUnderCursor = _objectUnderCursor, SlotUnderCursorNum = _slotUnderCursorNum, SourceItemsCollection = _sourceItemsCollection, draggableComponent = draggable });

    }

    private void TryLandDraggedObject()
    {
        IDragLandable landable = null;

        UiDetectUtil.TryGetUIElementUnderCursor(out GameObject targetObject);
        targetObject?.transform.parent.TryGetComponent(out landable);

        //HACK
        if(landable == null)
        {
            Instantiate(_itemInstance.ItemDefinition.Prefab, new Vector3(0,0,0), Quaternion.identity);

            _sourceItemsCollection.Remove(_originalSlotNum);

            _isDragging = false;

            if (_destroyImageObjectOnEnd)
            {
                Destroy(_draggedRect.gameObject);
            }

            return;
        }

        _slotUnderCursorNum = landable.GetHierarchyIndex();
        _isDragging = false;

        if (_destroyImageObjectOnEnd)
        {
            Destroy(_draggedRect.gameObject);
        }

        OnEndDrag?.Invoke(new() { DraggedRect = _draggedRect, ItemInstance = _itemInstance, ObjectUnderCursor = targetObject, SlotUnderCursorNum = _slotUnderCursorNum, SourceItemsCollection = _sourceItemsCollection, OriginalSlotNum = _originalSlotNum, landableComponent = (IDragLandable)landable });

    }

    private void DragUpdate()
    {
        Vector2 cursorPosition = Input.mousePosition;
        if (_draggedRect != null)
        {
            _draggedRect.position = cursorPosition;
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
        _isDragging = true;

        Image image = Instantiate(_draggedImagePrefab, _canvasRoot);

        _draggedRect = image.rectTransform;
        image.sprite = sprite;

        _draggedRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _draggedImageWidth);
        _draggedRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _draggedImageHeight);
    }

    //private void DisposeDragging(bool success)
    //{

    //    _isDragging = false;
    //}

    //private void TriggerDragEvent()
    //{
    //    Vector2 cursorPosition = Input.mousePosition;
    //    //EventBus.Publish(nameof(DragEventInfo), new DragEventInfo());
    //}
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

