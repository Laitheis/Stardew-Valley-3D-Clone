using UnityEngine;
using Zenject;

namespace Inventory.UI
{
    public class ItemTooltipView : MonoBehaviour
    {
        [Inject] private Camera _uiCamera;

        [SerializeField] private UIElementFitter _fitter;
        [SerializeField] private TooltipRefs _tooltip;

        private bool _isClingToMouse;
        private RectTransform _slotRect;
        private Vector2 _offset;

        private void Update()
        {
            if (_isClingToMouse)
            {
                //Vector2 localPoint;
                //RectTransformUtility.ScreenPointToLocalPointInRectangle(
                //    GetComponentInParent<Canvas>().GetComponent<RectTransform>(),
                //    Input.mousePosition,
                //    _uiCamera,
                //    out localPoint);
                _fitter.ShowAt(Input.mousePosition, _offset);
            }
        }

        public void ShowTooltip(ItemDefinition itemDef, RectTransform slotRect, Vector2 offset, bool clingToMouse)
        {
            if (itemDef == null) return;

            _slotRect = slotRect;
            _offset = offset;
            _isClingToMouse = clingToMouse;

            _tooltip.transform.parent.parent.gameObject.SetActive(true);

            _tooltip.Name.text = itemDef.Name;
            _tooltip.Type.text = itemDef.type.ToString();
            _tooltip.Description.text = itemDef.Description;

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

                Vector2 size = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta; // например (1920,1080)
                Vector2 adjustedPos = canvasPos + size / 2f;

                _fitter.ShowAt(adjustedPos, offset);
            }
        }

        public void CloseTooltip()
        {
            if (_tooltip.gameObject.activeSelf)
            {
                _tooltip.transform.parent.parent.gameObject.SetActive(false);
                _isClingToMouse = false;
            }
        }
    }
}
