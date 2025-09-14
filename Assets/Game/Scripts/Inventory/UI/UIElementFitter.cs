using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIElementFitter : MonoBehaviour
{
    [SerializeField] private RectTransform _movableElement;
    private RectTransform _targetElement;
    [SerializeField] private Canvas _canvas;

    public Vector2 offset;

    public void ShowAt(RectTransform target)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_movableElement);

        _targetElement = target;

        Vector3[] worldCorners = new Vector3[4];
        target.GetWorldCorners(worldCorners);

        Vector3 targetCenter = (worldCorners[0] + worldCorners[2]) / 2f;

        _movableElement.transform.position = target.position + (Vector3)offset;

        ClampToCanvas();
    }

    private void ClampToCanvas()
    {
        RectTransform canvasRect = _canvas.transform as RectTransform;
        Vector3[] elementCorners = new Vector3[4];
        _movableElement.GetWorldCorners(elementCorners);

        Vector3 shift = Vector3.zero;

        if (elementCorners[0].y < canvasRect.position.y - canvasRect.rect.height / 2f)
            shift.y += (canvasRect.position.y - canvasRect.rect.height / 2f) - elementCorners[0].y;

        if (elementCorners[0].x < canvasRect.position.x - canvasRect.rect.width / 2f)
            shift.x += (canvasRect.position.x - canvasRect.rect.width / 2f) - elementCorners[0].x;

        if (elementCorners[1].y > canvasRect.position.y + canvasRect.rect.height / 2f)
            shift.y -= elementCorners[1].y - (canvasRect.position.y + canvasRect.rect.height / 2f);

        if (elementCorners[2].x > canvasRect.position.x + canvasRect.rect.width / 2f)
            shift.x -= elementCorners[2].x - (canvasRect.position.x + canvasRect.rect.width / 2f);

        _movableElement.position += shift;
    }
}
