using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUtils
{
    static List<Vector2Int> ToCheck = new List<Vector2Int>(), CoordsChecked = new List<Vector2Int>(), neighbours=new List<Vector2Int>();
    static HashSet<Vector2Int> Checked;

    //go out from given position and get all tiles, stopping if we hit a wall
   public static void PerformFloodCheck(Vector2Int startCoords)
    {
        ToCheck = new List<Vector2Int>();
        Checked = new HashSet<Vector2Int>();
        CoordsChecked = new List<Vector2Int>();
        ToCheck.Add(startCoords);
        while (ToCheck.Count > 0)
        {
            List<Vector2Int> nextToCheck=new List<Vector2Int>();
           for(int x = 0; x < ToCheck.Count; x++)
            {

                if (WorldController.Instance.WallManager.DoesSomethingExistAtCoords(ToCheck[x]) == false)
                {

                    GetNeighbours(ToCheck[x]);
                    for (int i = 0; i < neighbours.Count; i++)
                    {
                        if (Checked.Contains(neighbours[i]))
                        {
                            continue;
                        }
                        else
                        {
                            if (nextToCheck.Contains(neighbours[i]) == false)
                            {
                                nextToCheck.Add(neighbours[i]);
                            }
                        }
                    }

                    Checked.Add(ToCheck[x]);
                    CoordsChecked.Add(ToCheck[x] );

                }


            }
            ToCheck = nextToCheck;
        }

        Debug.Log("Room: total room size at " + startCoords + " is " + CoordsChecked.Count);
    }

    public static List<Vector2Int> RoomFound()
    {
        return CoordsChecked;
    }

    static void GetNeighbours(Vector2Int coords)
    {
        neighbours.Clear();
        if (coords.x > 0)
        {
            neighbours.Add(coords + Vector2Int.left);
        }
        if (coords.x < WorldController.Instance.WorldWidth - 1)
        {
            neighbours.Add(coords + Vector2Int.right);
        }

        if(coords.y > 0)
        {
            neighbours.Add(coords + Vector2Int.down);
        }
        if(coords.y<WorldController.Instance.WorldHeight-1)
        {
            neighbours.Add(coords+Vector2Int.up);
        }
    }

    
    public static Bounds CreateBoundsFromPoints(List<Vector2Int> points)
    {
        Bounds b = new Bounds();
        for(int x=0;x<points.Count; x++)
        {
            b.Encapsulate(new Vector3(points[x].x, points[x].y));
        }
        return b;
    }


}
