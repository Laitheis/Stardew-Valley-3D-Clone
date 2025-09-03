using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public static class UiDetectUtil
{
    public static bool TryGetUIElementUnderCursor(out GameObject uiElement)
    {
        uiElement = null;

        if (EventSystem.current == null)
            return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();

        foreach (var raycaster in Object.FindObjectsOfType<GraphicRaycaster>())
        {
            raycaster.Raycast(pointerData, results);
        }

        if (results.Count > 0)
        {
            uiElement = results[0].gameObject;
            return true;
        }

        return false;
    }

    public static bool TryGetAllUIElementsUnderCursor(out List<GameObject> uiElements)
    {
        uiElements = new List<GameObject>();

        if (EventSystem.current == null)
            return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();

        foreach (var raycaster in Object.FindObjectsOfType<GraphicRaycaster>())
        {
            raycaster.Raycast(pointerData, results);
        }

        if (results.Count > 0)
        {
            foreach (var result in results)
            {
                uiElements.Add(result.gameObject);
            }
            return true;
        }

        return false;
    }
}
