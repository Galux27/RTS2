using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridUtils
{
    static List<Vector2Int> neighbourCache=new List<Vector2Int>();
    public static List<Vector2Int> GetNeighbouringCoords(Vector2Int center)
    {
        neighbourCache.Clear();

        if (center.x > 0)
        {
            neighbourCache.Add(center + Vector2Int.left);
        }

        if (center.x < WorldController.Instance.WorldWidth-1)
        {
            neighbourCache.Add(center + Vector2Int.right);
        }

        if (center.y > 0)
        {
            neighbourCache.Add(center + Vector2Int.down);
        }

        if (center.y < WorldController.Instance.WorldHeight - 1)
        {
            neighbourCache.Add(center + Vector2Int.up);
        }

        return neighbourCache;
    }
}
