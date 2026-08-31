using UnityEngine;

public static class EnvironmentObjectPlacementCriteriaHelpers
{
    public static int doorFail = 0, wallFail = 0, accessFail = 0, adjacencyFail = 0;
    public static bool IsPositionValidForObject(EnvironmentObject objectToCreate,GeneratedRoom room, Vector2Int coordinates,GeneratedBuilding building)
    {
        if (DoesPositionContainExistingProp(coordinates, room, objectToCreate, building))
        {
            return false;
        }

        if (objectToCreate.PlacementCriteria == null)
        {
            return true;
        }
        
        if (!DoWeMeedDoorCriteria(coordinates, room, objectToCreate,building))
        {
            return false;
        }
        if (!DoWeMeedWallCriteria(coordinates, room, objectToCreate, building))
        {
            return false;
        }
        if(!DoWeMeedObjectAccessiblityCriteria(coordinates, room, objectToCreate, building))
        {
            return false;
        }
        if(!DoWeMeedObjectAdjacencyCriteria(coordinates, room, objectToCreate, building))
        {
            return false;
        }
        return true;
    }


    static bool DoesPositionContainExistingProp(Vector2Int position, GeneratedRoom room, EnvironmentObject objectToCreate, GeneratedBuilding building)
    {
        Vector2Int pos = new Vector2Int();
        for(int x = position.x; x < position.x + room.size.x; x++)
        {
            for(int y = position.y; y < position.y + room.size.y; y++)
            {
                pos.x = x;
                pos.y = y;
                pos = building.ConvertRoomCoordsToBuildingCoords(pos, room);
                if (building.Tiles[pos.x, pos.y] != null)
                {
                    if(building.Tiles[pos.x, pos.y].HasProp)
                    {
                        return true;
                    }
                }
                
            }
        }
        return false;
    }


    #region Walls
    static bool DoWeMeedWallCriteria(Vector2Int position,GeneratedRoom room,EnvironmentObject objectToCreate,GeneratedBuilding building)
    {
        for(int x = 0; x < objectToCreate.PlacementCriteria.MyAccessiblityData.Count; x++)
        {
            for(int y=0;y< objectToCreate.PlacementCriteria.MyAccessiblityData[x].WallAccessibilities.Count; y++)
            {
                if (!IsIndividualWallCriteriaValid(position, room, objectToCreate, objectToCreate.PlacementCriteria.MyAccessiblityData[x].WallAccessibilities[y], building)) 
                { 
                    return false;
                }
            }
        }
        return true;
    }

    static bool IsIndividualWallCriteriaValid(Vector2Int position, GeneratedRoom room, EnvironmentObject objectToCreate,WallAccessibiltyData data, GeneratedBuilding building)
    {
        Vector2Int StartPos = GetPositionOnEdgeFromDirection(position, data.Direction, objectToCreate);
        Vector2Int Axis = AxisFromDirection(data.Direction);
        int Size = SizeFromDirection(data.Direction,objectToCreate);
        Vector2Int curPos = StartPos;
        for(int i = 0; i < Size; i++)
        {
            curPos = StartPos + (Axis * i);
            curPos = building.ConvertRoomCoordsToBuildingCoords(curPos, room);
            if (!DoesTileMeetWallCriteria(building.Tiles[curPos.x, curPos.y], data))
            {
                return false;
            }
        }
        return true;
    }

    static bool DoesTileMeetWallCriteria(RoomTile tile,WallAccessibiltyData data)
    {
        if (tile == null)
        {
            return true;
        }
        switch (data.WallAccessibility)
        {
            case WallAccessibility.DontCare:
                return true;
                break;
            case WallAccessibility.MustBeNextToWall:
                return tile.HasWall;
                break;
            case WallAccessibility.CanBeNextToWall:
                return true;
                break;
            case WallAccessibility.NotAgainstWall:
                return !tile.HasWall; 
                break;
            default:
                break;
        }
        return true;
    }

    #endregion
    #region Doors
    static bool DoWeMeedDoorCriteria(Vector2Int position, GeneratedRoom room, EnvironmentObject objectToCreate, GeneratedBuilding building)
    {
        for (int x = 0; x < objectToCreate.PlacementCriteria.MyAccessiblityData.Count; x++)
        {
            
                if (!IsIndividualDoorCriteriaValid(position, room, objectToCreate, objectToCreate.PlacementCriteria.MyAccessiblityData[x].DoorAccessibilty, building))
                {
                    return false;
               }
            
        }
        return true;
    }

    static bool IsIndividualDoorCriteriaValid(Vector2Int position, GeneratedRoom room, EnvironmentObject objectToCreate, DoorAccessibility data, GeneratedBuilding building)
    {
        for(int x = 0; x < 4; x++)
        {
            DirectionFromObject direction=(DirectionFromObject)x;
            Vector2Int StartPos = GetPositionOnEdgeFromDirection(position, direction, objectToCreate);
            Vector2Int Axis = AxisFromDirection(direction);
            int Size = SizeFromDirection(direction, objectToCreate);
            Vector2Int curPos = StartPos;
            for (int i = 0; i < Size; i++)
            {
                curPos = StartPos + (Axis * i);
                curPos = building.ConvertRoomCoordsToBuildingCoords(curPos, room);
                if (!DoesTileMeetDoorCriteria(building.Tiles[curPos.x, curPos.y], data))
                {
                    return false;
                }
            }
        }

        return true;
    }

    static bool DoesTileMeetDoorCriteria(RoomTile tile, DoorAccessibility data)
    {
        if (tile == null)
        {
            return true;
        }
        switch (data)
        {
            case DoorAccessibility.DontCare:
                return true;
                break;
            case DoorAccessibility.DontBlockDoors:
                return !tile.HasDoor;
                break;
            default:
                break;
        }


        return true;
    }
    #endregion
  
    static bool DoWeMeedObjectAdjacencyCriteria(Vector2Int position, GeneratedRoom room, EnvironmentObject objectToCreate, GeneratedBuilding building)
    {
        for (int x = 0; x < objectToCreate.PlacementCriteria.MyAccessiblityData.Count; x++)
        {
            for (int y = 0; y < objectToCreate.PlacementCriteria.MyAccessiblityData[x].OtherObjectAccessiblityData.Count; y++)
            {
                if (!IsIndividualObjectAdjacencyValid(position, room, objectToCreate, objectToCreate.PlacementCriteria.MyAccessiblityData[x].OtherObjectAccessiblityData[y], building))
                {
                    return false;
                }
            }
        }
        return true;
    }

    static bool IsIndividualObjectAdjacencyValid(Vector2Int position, GeneratedRoom room, EnvironmentObject objectToCreate, OtherObjectAccessiblityData data, GeneratedBuilding building)
    {
        Vector2Int StartPos = GetPositionOnEdgeFromDirection(position, data.Direction, objectToCreate);
        Vector2Int Axis = AxisFromDirection(data.Direction);
        int Size = SizeFromDirection(data.Direction, objectToCreate);
        Vector2Int curPos = StartPos;
        int accessibleTiles = 0;
        for (int i = 0; i < Size; i++)
        {
            curPos = StartPos + (Axis * i);
            curPos = building.ConvertRoomCoordsToBuildingCoords(curPos, room);
            if (!DoesTileMeetObjectAdjacencyCriteria(building.Tiles[curPos.x, curPos.y], data))
            {
                return false;
            }

        }
       
        return true;
    }

    static bool DoesTileMeetObjectAdjacencyCriteria(RoomTile tile, OtherObjectAccessiblityData data)
    {
        if (tile == null)
        {
            return true;
        }
        switch (data.ObjectAccessibility)
        {
            case OtherObjectAccessibility.DontCare:
                return true;
                break;
            case OtherObjectAccessibility.SpaceBetweenObjects:
                return !tile.HasProp;
                break;
            default:
                break;
        }


        return true;
    }



    #region ObjectAccessiblity

    static bool DoWeMeedObjectAccessiblityCriteria(Vector2Int position, GeneratedRoom room, EnvironmentObject objectToCreate, GeneratedBuilding building)
    {
        for (int x = 0; x < objectToCreate.PlacementCriteria.MyAccessiblityData.Count; x++)
        {
            for (int y = 0; y < objectToCreate.PlacementCriteria.MyAccessiblityData[x].ObjectAccessibily.Count; y++)
            {
                if (!IsIndividualObjectAccessiblityValid(position, room, objectToCreate, objectToCreate.PlacementCriteria.MyAccessiblityData[x].ObjectAccessibily[y], building))
                {
                    return false;
                }
            }
        }
        return true;
    }

    static bool IsIndividualObjectAccessiblityValid(Vector2Int position, GeneratedRoom room, EnvironmentObject objectToCreate, ObjectAccessibility data, GeneratedBuilding building)
    {
        Vector2Int StartPos = GetPositionOnEdgeFromDirection(position, data.Direction, objectToCreate);
        Vector2Int Axis = AxisFromDirection(data.Direction);
        int Size = SizeFromDirection(data.Direction, objectToCreate);
        Vector2Int curPos = StartPos;
        int accessibleTiles = 0;
        for (int i = 0; i < Size; i++)
        {
            curPos = StartPos + (Axis * i);
            curPos = building.ConvertRoomCoordsToBuildingCoords(curPos, room);
            if (DoesTileMeetObjectAccessiblityCriteria(building.Tiles[curPos.x, curPos.y], data))
            {
                accessibleTiles++;
            }

        }
        switch (data.Accessiblity)
        {
            case AccessableTilesNeeded.All:
                return Size == accessibleTiles;
                break;
            case AccessableTilesNeeded.AtLeastOne:
                return accessibleTiles > 0;
                break;
            case AccessableTilesNeeded.DontCare:
                return true;
                break;
            default:
                break;
        }
        return true;
    }

    static bool DoesTileMeetObjectAccessiblityCriteria(RoomTile tile, ObjectAccessibility data)
    {
        if (tile == null)
        {
            return true;
        }
        switch (data.Accessiblity)
        {
            case AccessableTilesNeeded.All:
            case AccessableTilesNeeded.AtLeastOne:
                return tile.HasDoor == false && tile.HasWall == false && tile.HasProp == false;
                break;
            case AccessableTilesNeeded.DontCare:
                return true;
                break;
            default:
                break;
        }
        return true;
    }

    #endregion

    static Vector2Int GetPositionOnObjectFromDirection(Vector2Int startPos, DirectionFromObject direction, EnvironmentObject objectToCreate)
    {
        switch (direction)
        {
            case DirectionFromObject.Up:
                return startPos + (Vector2Int.up * (objectToCreate.GetHeight-1));
                break;
            case DirectionFromObject.Down:
                return startPos ;
                break;
            case DirectionFromObject.Left:
                return startPos ;
                break;
            case DirectionFromObject.Right:
                return startPos + (Vector2Int.right * (objectToCreate.GetWidth-1));

                break;
            default:
                break;
        }
        return startPos;
    }


    /// <summary>
    /// Get size of axis to traverse to check the given axis for a criteria
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="objectToCreate"></param>
    /// <returns></returns>
    static int SizeFromDirection(DirectionFromObject direction, EnvironmentObject objectToCreate)
    {
        switch (direction)
        {
            case DirectionFromObject.Up:
            case DirectionFromObject.Down:
                return objectToCreate.GetWidth;
                break;
            case DirectionFromObject.Left:
            case DirectionFromObject.Right:
                return objectToCreate.GetHeight;

                break;
            default:
                break;
        }
        return 0;
    }

 
    /// <summary>
    /// Get axis to traverse in order to check the given axis for a criteria
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    static Vector2Int AxisFromDirection(DirectionFromObject direction)
    {
        switch (direction)
        {
            case DirectionFromObject.Up:
            case DirectionFromObject.Down:
                return Vector2Int.right;
                break;
            case DirectionFromObject.Left:
            case DirectionFromObject.Right:
                return Vector2Int.up;

                break;
            default:
                break;
        }
        return Vector2Int.zero;
    }

    static Vector2Int GetPositionOnEdgeFromDirection(Vector2Int startPos, DirectionFromObject direction,EnvironmentObject objectToCreate)
    {
        switch (direction)
        {
            case DirectionFromObject.Up:
                return startPos + (Vector2Int.up*objectToCreate.GetHeight);
                break;
            case DirectionFromObject.Down:
                return startPos + Vector2Int.down;
                break;
            case DirectionFromObject.Left:
                return startPos + Vector2Int.left;
                break;
            case DirectionFromObject.Right:
                return startPos + (Vector2Int.right * objectToCreate.GetWidth);

                break;
            default:
                break;
        }
        return startPos;
    }
}
