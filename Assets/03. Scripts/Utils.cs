using UnityEngine;

public static class Utils
{
    public static Vector2 GetWorldPosition(RectTransform canvasRect, Transform target, Camera camera)
    {
        if (!canvasRect)
        {
            return Vector2.zero;
        }
        
        if (!target || !camera)
        {
            return Vector2.zero;
        }
                
        Vector2 canvasSizeDelta = canvasRect.sizeDelta;
        
        Vector2 viewportPosition = camera.WorldToViewportPoint(target.position);
        Vector2 screenPosition = new Vector2(((viewportPosition.x * canvasSizeDelta.x) - (canvasSizeDelta.x * 0.5f)), ((viewportPosition.y * canvasSizeDelta.y) - (canvasSizeDelta.y * 0.5f)));
        return screenPosition;
    }
}
