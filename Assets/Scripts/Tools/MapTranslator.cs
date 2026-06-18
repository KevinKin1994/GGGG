using UnityEngine;

public static class MapTranslator
{
    // 将屏幕坐标转换为地图坐标，并判断该坐标是否在地图范围内。
    public static bool TryScreenToMapPosition(
        Vector2 screenPosition,
        RectTransform canvasRectTransform,
        Camera uiCamera,
        int mapSideLength,
        Vector2 gridSize,
        Vector4 transparentPadding,
        out Vector2Int mapPosition)
    {
        mapPosition = Vector2Int.zero;

        if (!TryScreenToLocalPosition(screenPosition, canvasRectTransform, uiCamera, out Vector2 localPosition))
        {
            return false;
        }

        mapPosition = LocalToMapPosition(localPosition, gridSize, transparentPadding);
        return IsInsideMap(mapPosition, mapSideLength);
    }

    // 将屏幕坐标转换为指定 Canvas 下的本地 UI 坐标。
    public static bool TryScreenToLocalPosition(
        Vector2 screenPosition,
        RectTransform canvasRectTransform,
        Camera uiCamera,
        out Vector2 localPosition)
    {
        if (canvasRectTransform == null)
        {
            localPosition = Vector2.zero;
            return false;
        }

        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            screenPosition,
            uiCamera,
            out localPosition);
    }

    // 将地图坐标转换为 Canvas 本地 UI 坐标，用于摆放对应格子。
    public static Vector2 MapToLocalPosition(
        Vector2Int mapPosition,
        Vector2 gridSize,
        Vector4 transparentPadding)
    {
        Vector2 effectiveSize = GetEffectiveGridSize(gridSize, transparentPadding);
        float x = effectiveSize.x * (mapPosition.x + mapPosition.y * 0.5f);
        float y = effectiveSize.y * 0.75f * mapPosition.y;
        return new Vector2(x, y) - GetContentCenterOffset(transparentPadding);
    }

    // 将 Canvas 本地 UI 坐标转换为最近的地图坐标。
    public static Vector2Int LocalToMapPosition(
        Vector2 localPosition,
        Vector2 gridSize,
        Vector4 transparentPadding)
    {
        Vector2 effectiveSize = GetEffectiveGridSize(gridSize, transparentPadding);
        Vector2 contentPosition = localPosition + GetContentCenterOffset(transparentPadding);
        float r = contentPosition.y / (effectiveSize.y * 0.75f);
        float q = contentPosition.x / effectiveSize.x - r * 0.5f;
        return AxialRound(q, r);
    }

    // 判断地图坐标是否位于边长为 mapSideLength 的六边形地图范围内。
    public static bool IsInsideMap(Vector2Int mapPosition, int mapSideLength)
    {
        int radius = Mathf.Max(0, mapSideLength - 1);
        int s = -mapPosition.x - mapPosition.y;
        return Mathf.Abs(mapPosition.x) <= radius
               && Mathf.Abs(mapPosition.y) <= radius
               && Mathf.Abs(s) <= radius;
    }

    // 根据格子显示尺寸和透明边距，计算真正用于拼接和坐标换算的有效尺寸。
    public static Vector2 GetEffectiveGridSize(Vector2 gridSize, Vector4 transparentPadding)
    {
        float width = gridSize.x - transparentPadding.x - transparentPadding.z;
        float height = gridSize.y - transparentPadding.y - transparentPadding.w;
        return new Vector2(Mathf.Max(1f, width), Mathf.Max(1f, height));
    }

    // 根据不对称透明边距，计算图片内容中心相对 RectTransform 中心的偏移。
    public static Vector2 GetContentCenterOffset(Vector4 transparentPadding)
    {
        float x = (transparentPadding.x - transparentPadding.z) * 0.5f;
        float y = (transparentPadding.w - transparentPadding.y) * 0.5f;
        return new Vector2(x, y);
    }

    // 将浮点 axial 坐标四舍五入为合法的整数六边形坐标。
    private static Vector2Int AxialRound(float q, float r)
    {
        float x = q;
        float z = r;
        float y = -x - z;

        int roundedX = Mathf.RoundToInt(x);
        int roundedY = Mathf.RoundToInt(y);
        int roundedZ = Mathf.RoundToInt(z);

        float xDiff = Mathf.Abs(roundedX - x);
        float yDiff = Mathf.Abs(roundedY - y);
        float zDiff = Mathf.Abs(roundedZ - z);

        if (xDiff > yDiff && xDiff > zDiff)
        {
            roundedX = -roundedY - roundedZ;
        }
        else if (yDiff > zDiff)
        {
            roundedY = -roundedX - roundedZ;
        }
        else
        {
            roundedZ = -roundedX - roundedY;
        }

        return new Vector2Int(roundedX, roundedZ);
    }
}
