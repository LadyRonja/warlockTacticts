using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GridLayoutRules
{
    public static readonly float Sqrt3 = Mathf.Sqrt(3f);

    public struct LayoutData
    {
        [JsonConstructor]
        public LayoutData(Vector2 startPosition, float tileWidth, float tileHeight)
        {
            this.startX = startPosition.x;
            this.startY = startPosition.y;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
        }

        public readonly float startX;
        public readonly float startY;
        public readonly float tileWidth;
        public readonly float tileHeight;
    }

    public static Vector2 GetPositionForFlatTopTile(LayoutData layoutData, int q, int r)
    {
        float size = layoutData.tileWidth / 2f;

        // Red Blob Games (Amit Patel) has been a wonderful resource for dealing with the mathematics of hex based grids
        // https://www.redblobgames.com/grids/hexagons/implementation.html
        float f0 = 3f / 2f;
        //float f1 = 0f;
        float f2 = Sqrt3 / 2f;
        float f3 = Sqrt3;

        Vector2 tilePos = new Vector2(
            layoutData.startX + (f0 * q/* + f1 * r*/) * size,
            layoutData.startY + (f2 * q + f3 * r) * size);

        return tilePos;
    }

    public static (int q, int r, int s) GetTileCoordsFromPositionFlatTop(LayoutData layoutData, Vector2 worldPosition)
    {
        float size = layoutData.tileWidth * 0.5f;

        // Red Blob Games (Amit Patel) has been a wonderful resource for dealing with the mathematics of hex based grids
        // https://www.redblobgames.com/grids/hexagons/implementation.html
        float b0 = 2f / 3f;
        //float b1 = 0f;
        float b2 = -1f / 3f;
        float b3 = Sqrt3 / 3f;

        Vector2 offSetRemoval = new Vector2(
            (worldPosition.x - layoutData.startX) / size,
            (worldPosition.y - layoutData.startY) / size
            );

        float qFract = (b0 * offSetRemoval.x/* + b1 * offSetRemoval.y*/);
        float rFract = (b2 * offSetRemoval.x + b3 * offSetRemoval.y);
        float sFract = -(qFract - rFract);

        (int q, int r, int s) = RoundFractionalCoords(qFract, rFract);

        return (q, r, s);
    }

    public static (int q, int r, int s) RoundFractionalCoords(float qFrac, float rFrac)
    {
        int q = Mathf.RoundToInt(qFrac);
        int r = Mathf.RoundToInt(rFrac);
        int s = -(q + r);

        Debug.Assert(q + r + s == 0, $"IMPROPER GRID ROUNDING: q + r + s != 0 {q}, {r}, {s}");
        return (q, r, s);
    }

}
