using InventorySystem;
using System;
using UI.Dragging;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class UIDragHandler : MonoBehaviour
{
    public event Action<DragEventInfo> OnDrag;
    public event Action<DragEventInfo> OnStartDrag;
    public event Action<DragEventInfo> OnEndDrag;

    private Image _draggedImagePrefab;
    private DiContainer _container;

    private float _draggedImageHeight = 100;
    private float _draggedImageWidth = 100;
    private Transform _canvasRoot;

    private bool _destroyImageObjectOnEnd = true;
    private bool _makeCopyOfDraggedRect = true;

    private int _slotUnderCursorNum;
    private RectTransform _draggedRect;
    private GameObject _objectUnderCursor;
    private ItemsCollection _sourceItemsCollection;
    private ItemInstance _itemInstance;

    private bool _isDragging;

    private Camera _uiCamera;

    internal static UIDragHandler Instance;

    public bool IsDragging { get => _isDragging; set => _isDragging = value; }

    private void Awake()
    {
        Instance = this;
    }

    [Inject]
    private void Constructor([Inject(Id = "DraggedImagePrefab")] Image draggedImagePrefab, Canvas mainCanvas, DiContainer container)
    {
        _draggedImagePrefab = draggedImagePrefab;
        _uiCamera = Camera.main;
        _container = container;
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
        if (UiDetectUtil.TryGetUIElementUnderCursor(out GameObject clickedObject) &&
            clickedObject.TryGetComponent(out IUIDraggable draggable))
        {
            _isDragging = true;

            _slotUnderCursorNum = draggable.GetHierarchyIndex();
            _objectUnderCursor = clickedObject;

            OnStartDrag?.Invoke(new() { draggedRect = _draggedRect, objectUnderCursor = _objectUnderCursor, slotUnderCursorNum = _slotUnderCursorNum });
        }
    }

    private void TryLandDraggedObject()
    {
        IDragLandable landable = null;
        if (UiDetectUtil.TryGetUIElementUnderCursor(out GameObject targetObject) &&
            targetObject.TryGetComponent(out landable))
        {
            if (landable.AbleToLanding(_draggedRect, _itemInstance))
            {
                landable.OnLanding(_draggedRect, _itemInstance);
            }
        }

        _slotUnderCursorNum = landable == null ? 0 : landable.GetHierarchyIndex();
        _isDragging = false;

        if (_destroyImageObjectOnEnd)
        {
            UnityEngine.Object.Destroy(_draggedRect.gameObject);
        }
        OnEndDrag?.Invoke(new() { draggedRect = _draggedRect, ItemInstance = _itemInstance, objectUnderCursor = targetObject, slotUnderCursorNum = _slotUnderCursorNum, sourceItemsCollection = _sourceItemsCollection });
    }

    private void DragUpdate()
    {
        Vector2 cursorPosition = Input.mousePosition;
        if (_draggedRect != null)
        {
            _draggedRect.position = cursorPosition;

        }

        //TriggerDragEvent();

        OnDrag?.Invoke(null);
    }

    private void DisposeDragging(bool success)
    {

        _isDragging = false;
    }

    private void TriggerDragEvent()
    {
        Vector2 cursorPosition = Input.mousePosition;
        //EventBus.Publish(nameof(DragEventInfo), new DragEventInfo());
    }
}


public class DragEventInfo
{
    public int slotUnderCursorNum;
    public ItemsCollection sourceItemsCollection;
    public ItemInstance ItemInstance;
    public GameObject objectUnderCursor;
    public RectTransform draggedRect;
}

