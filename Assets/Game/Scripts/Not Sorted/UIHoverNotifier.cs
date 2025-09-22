using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIHoverNotifier : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent onMouseEnter;
    public UnityEvent onMouseExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onMouseEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onMouseExit.Invoke();
    }
}

