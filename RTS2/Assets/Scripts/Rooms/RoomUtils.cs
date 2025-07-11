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


    public static bool IsValid(Room room)
    {

        if (room.roomType == RoomUseType.None)
        {
            return true;
        }
        bool isValid = RoomManager.Instance.ValidityData[room.roomType].IsValid(room);
        bool isEnclosed = IsRoomEnclosed(room);
        return isValid && isEnclosed;
    }


    public static string GetValiditiyIssues(Room r)
    {
        if (r.roomType == RoomUseType.None)
        {
            return "";
        }
        string retVal = "";

        if (IsRoomEnclosed(r) == false)
        {
            retVal += "Not Enclosed, ";
        }

        retVal += RoomManager.Instance.ValidityData[r.roomType].GetIssuesWithRoom(r);
        return retVal ;
    }
    
    public static bool IsRoomEnclosed(Room r)
    {
        List<Vector2Int> tilesOnEdge = new List<Vector2Int>();
        List<Vector2Int> neighbours = null;
        int count = 0;
        for (int x = 0; x < r.tilesInRoom.Count; x++)
        {
            count = 0;
            neighbours = GridUtils.GetNeighbouringCoords(r.tilesInRoom[x]);

            for (int q = 0; q < neighbours.Count; q++)
            {
                if (r.tilesInRoom.Contains(neighbours[q])){
                    count++;
                }
            }

            if (count < 4)
            {
                tilesOnEdge.Add(r.tilesInRoom[x]);
            }
        }
        r.EdgeTiles = tilesOnEdge;
        List<Vector2Int> invalidEdge = new List<Vector2Int>();
        for(int x = 0; x < tilesOnEdge.Count; x++)
        {
            if (WallHelpers.DoesConstructedWallExistAtPosition(tilesOnEdge[x].x, tilesOnEdge[x].y)==false &&
                WallHelpers.DoesConstructedDoorExistAtPosition(tilesOnEdge[x].x, tilesOnEdge[x].y)==false)
            {
                
       
                neighbours = GridUtils.GetNeighbouringCoords(tilesOnEdge[x]);
                count = 0;
                int roomNeighbours = 0, WallNeighbours = 0, doorNeighbours = 0;
                for (int q = 0; q < neighbours.Count; q++)
                {
                    if (r.tilesInRoom.Contains(neighbours[q]))
                    {
                        roomNeighbours++;
                    }
                    if (WallHelpers.DoesConstructedWallExistAtPosition(neighbours[q].x, neighbours[q].y))
                    {
                        WallNeighbours++;
                    }
                    if (WallHelpers.DoesConstructedDoorExistAtPosition(neighbours[q].x, neighbours[q].y))
                    {
                        doorNeighbours++;
                    }
                    count += GetCountForRoomTile(neighbours[q], r);
                }
                if (count < neighbours.Count)
                {
                    invalidEdge.Add(tilesOnEdge[x]);
                    //return false;
                }
            }


           
        }
        r.InvalidEdge = invalidEdge;
        if (invalidEdge.Count > 0)
        {
            Debug.Log("Room: is room enclosed " + r.roomName+" false ");

            return false;
        }
        Debug.Log("Room: is room enclosed " + r.roomName+" true");


        return true;
    }

    public static int GetCountForRoomTile(Vector2Int coords,Room r)
    {
        if (r.tilesInRoom.Contains(coords) ||
                       WallHelpers.DoesConstructedWallExistAtPosition(coords.x, coords.y) ||
                       WallHelpers.DoesConstructedDoorExistAtPosition(coords.x, coords.y))
        {
            return 1;
        }
        return 0;
    }

    public static bool DoesRoomContainObject(Room r,string objectToFind,out int quantity)
    {
        quantity = 0;
        for(int x = 0; x < r.ObjectsInRoom.Count; x++)
        {
            if (r.ObjectsInRoom[x].Name() == objectToFind)
            {
                quantity++;
            }
        }
        return quantity > 0;
        //quantity = 0;
        //Vector2Int coords = Vector2Int.zero;
        //List<EnvironmentObjectInstance> objects = new List<EnvironmentObjectInstance>();
        //EnvironmentObjectInstance instance = null;
        //for(int x = 0; x < r.tilesInRoom.Count; x++)
        //{
        //   coords = WorldChunkManager.Instance.GetChunkCoordsFromTileCoords(r.tilesInRoom[x]);
        //    Debug.Log("Room: checking chunk " + coords + " for " + objectToFind);
        //    WorldChunkManager.Instance.Chunks[coords.x, coords.y].DoesObjectExistAtCoords(r.tilesInRoom[x],objectToFind,out instance);
        //    if (instance != null && objects.Contains(instance) == false)
        //    {
        //        objects.Add(instance);
        //    }
        //}
        //if(objects.Count > 0)
        //{
        //    quantity = objects.Count;
        //    return true;
        //}

        //return false;
    }
}
