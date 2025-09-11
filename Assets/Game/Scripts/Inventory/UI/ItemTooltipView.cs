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
        [SerializeField] private TooltipHolder _tooltip;

        public void ShowTooltip(ItemInstance itemInstance)
        {
            _tooltip.gameObject.SetActive(true);

            _tooltip.Name.text = itemInstance.ItemDefinition.Name;
            _tooltip.Type.text = itemInstance.ItemDefinition.Type.ToString();
            _tooltip.Description.text = itemInstance.ItemDefinition.Description;
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
