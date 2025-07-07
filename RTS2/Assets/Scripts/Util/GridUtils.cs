using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridUtils
{
    static List<Vector2Int> neighbourCache=new List<Vector2Int>();
    public static List<Vector2Int> GetNeighbouringCoords(Vector2Int center)
    {
        neighbourCache.Clear();

        
            neighbourCache.Add(center + Vector2Int.left);
        

        
            neighbourCache.Add(center + Vector2Int.right);
        

       
            neighbourCache.Add(center + Vector2Int.down);
        

       
            neighbourCache.Add(center + Vector2Int.up);
        

        return neighbourCache;
    }
}
