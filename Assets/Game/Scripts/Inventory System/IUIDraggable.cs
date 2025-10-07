using UnityEngine;

public interface IUIDraggable
{
    bool AbleToDrag();
    void OnStartDrag();
    RectTransform GetItemIconRect();
    object GetInfo();
    void OnEndDrag(bool success);
    int GetHierarchyIndex();
}

public interface IDragLandable
{
    bool AbleToLanding(RectTransform draggedRect, object info);
    void OnLanding(RectTransform draggedRect, object info);
    int GetHierarchyIndex();
}

