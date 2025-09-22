using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory.UI
{
    public class ItemTooltipView : MonoBehaviour
    {
        [SerializeField] UIElementFitter _fitter;
        [SerializeField] private TooltipRefs _tooltip;

        public void ShowTooltip(ItemInstance itemInstance, RectTransform slotRect)
        {
            if (itemInstance.ItemDefinition == null) return;

            _tooltip.gameObject.SetActive(true);

            _tooltip.Name.text = itemInstance.ItemDefinition.Name;
            _tooltip.Type.text = itemInstance.ItemDefinition.type.ToString();
            _tooltip.Description.text = itemInstance.ItemDefinition.Description;

            _fitter.ShowAt(slotRect);
        }
        public void CloseTooltip()
        {
            if(_tooltip.gameObject.activeSelf)
            {
                _tooltip.gameObject.SetActive(false);
            }
        }
    }
}
