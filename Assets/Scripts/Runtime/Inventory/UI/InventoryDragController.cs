using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class InventoryDragController : MonoBehaviour
{
    [SerializeField] RectTransform dragLayer;
    [SerializeField] Image dragIcon;

    [SerializeField, Range(0f, 1f)]
    float sourceAlpha = 0.25f;

    bool isDragging;

    Image sourceIcon;
    Color sourceOriginalColor;

    public bool IsDragging => isDragging;

    public void Begin(Image source, PointerEventData eventData)
    {
        if (isDragging) End();

        if (source == null || source.sprite == null) return;

        sourceIcon = source;
        sourceOriginalColor = source.color;

        isDragging = true;

        SetSourceAlpha();

        dragIcon.sprite = source.sprite;
        dragIcon.enabled = true;

        MatchSourceSize(source);
        UpdatePosition(eventData);
    }

    public void UpdateDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        UpdatePosition(eventData);
    }

    public void End()
    {
        if (!isDragging) return;

        RestoreSource();

        isDragging = false;

        dragIcon.sprite = null;
        dragIcon.enabled = false;

        sourceIcon = null;
    }

    void SetSourceAlpha()
    {
        Color color = sourceOriginalColor;
        color.a *= sourceAlpha;

        sourceIcon.color = color;
    }

    void RestoreSource()
    {
        if (sourceIcon == null) return;

        sourceIcon.color = sourceOriginalColor;
    }

    void MatchSourceSize(Image source)
    {
        RectTransform sourceRect = source.rectTransform;
        RectTransform targetRect = dragIcon.rectTransform;

        Vector3[] worldCorners = new Vector3[4];
        sourceRect.GetWorldCorners(worldCorners);

        Vector3 bottomLeft = dragLayer.InverseTransformPoint(worldCorners[0]);
        Vector3 topLeft = dragLayer.InverseTransformPoint(worldCorners[1]);
        Vector3 topRight = dragLayer.InverseTransformPoint(worldCorners[2]);
        Vector3 bottomRight = dragLayer.InverseTransformPoint(worldCorners[3]);

        float width = Vector3.Distance(bottomLeft, bottomRight);
        float height = Vector3.Distance(bottomLeft, topLeft);

        targetRect.SetParent(dragLayer, false);

        targetRect.anchorMin = new Vector2(0.5f, 0.5f);
        targetRect.anchorMax = new Vector2(0.5f, 0.5f);
        targetRect.pivot = new Vector2(0.5f, 0.5f);

        targetRect.localScale = Vector3.one;
        targetRect.localRotation = Quaternion.identity;

        targetRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        targetRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    void UpdatePosition(PointerEventData eventData)
    {
        if (dragLayer == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(dragLayer, eventData.position, eventData.pressEventCamera, out Vector2 localPosition);

        Vector2 size = dragIcon.rectTransform.rect.size;
        Rect rect = dragLayer.rect;

        float halfWidth = size.x * 0.5f;
        float halfHeight = size.y * 0.5f;

        localPosition.x = Mathf.Clamp(localPosition.x, rect.xMin + halfWidth, rect.xMax - halfWidth);

        localPosition.y = Mathf.Clamp(localPosition.y, rect.yMin + halfHeight, rect.yMax - halfHeight);

        dragIcon.rectTransform.localPosition = localPosition;
    }
}
