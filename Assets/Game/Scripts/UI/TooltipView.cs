using UnityEngine;
using Zenject;

public class TooltipView : MonoBehaviour
{
    [Inject] private Camera _uiCamera;

    [SerializeField] private UIElementFitController _fitter;
    [SerializeField] private TooltipRefs _tooltipRefs;

    private bool _isClingToMouse;
    private RectTransform _slotRect;
    private Vector2 _offset;

    public bool IsClingToMouse { get => _isClingToMouse; set => _isClingToMouse = value; }

    private void Update()
    {
        if (_isClingToMouse)
        {
            _fitter.ShowAt(Input.mousePosition, _offset);
        }
    }

    public void ShowTooltip(ItemDefinition itemDef, RectTransform slotRect, Vector2 offset, bool clingToMouse)
    {
        if (itemDef == null) return;

        _slotRect = slotRect;
        _offset = offset;
        _isClingToMouse = clingToMouse;

        _tooltipRefs.transform.parent.parent.gameObject.SetActive(true);

        _tooltipRefs.Name.text = itemDef.Name;
        _tooltipRefs.Type.text = itemDef.type.ToString();
        _tooltipRefs.Description.text = itemDef.Description;

        if (!_isClingToMouse)
        {
            Vector3 worldPos = slotRect.position;

            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                GetComponentInParent<Canvas>().GetComponent<RectTransform>(),
                _uiCamera.WorldToScreenPoint(slotRect.position),
                _uiCamera,
                out canvasPos
            );

            Vector2 size = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
            Vector2 adjustedPos = canvasPos + size / 2f;

            _fitter.ShowAt(adjustedPos, offset);
        }
    }

    public void CloseTooltip()
    {
        if (_tooltipRefs.gameObject.activeSelf)
        {
            _tooltipRefs.transform.parent.parent.gameObject.SetActive(false);
            _isClingToMouse = false;
        }
    }
}