using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;
public class RoomObjectPlacement
{
    //Add prioritizing by criteria
    //e.g. higher priority for distance from door
    public Dictionary<string,RoomObjectPositions> AllObjectsInRoom = new Dictionary<string, RoomObjectPositions>();
    public RoomObjectPlacement(RoomTemplate template,GeneratedRoom room,GeneratedBuilding building)
    {
        for(int x = 0; x < template.Props.Count; x++)
        {
            AllObjectsInRoom.Add(template.Props[x].PropName,new RoomObjectPositions(template.Props[x].PropName));
        }
        foreach(KeyValuePair<string,RoomObjectPositions> kvp in AllObjectsInRoom)
        {
            kvp.Value.InitPositions(room, building);
        }
       
    }

    public void LogPositionsForProps()
    {
        foreach(KeyValuePair<string,RoomObjectPositions> kvp in AllObjectsInRoom)
        {
            Debug.Log("Room Gen: positions for " + kvp.Key + " count " + kvp.Value.ValidPositions.Count);
        }
    }

    public void RefreshObjectValidity(GeneratedBuilding building,GeneratedRoom room)
    {
        foreach (KeyValuePair<string, RoomObjectPositions> kvp in AllObjectsInRoom)
        {
            kvp.Value.RefreshPositions(room, building);
        }
    }

    public void LogPositionsForProp(string propKey)
    {
        if (AllObjectsInRoom.ContainsKey(propKey))
        {
            Debug.Log("Room Gen: Positions for " + propKey + "," + AllObjectsInRoom[propKey].ValidPositions.Count);
        }
        else
        {
            Debug.Log("Room Gen: Positions for " + propKey + ",null");

        }
    }

    public Vector2Int GetCoordinateForProp(string propKey)
    {
        if (AllObjectsInRoom.ContainsKey(propKey))
        {
            if (AllObjectsInRoom[propKey].ValidPositions.Count > 0)
            {
                RoomObjectPosition retVal = AllObjectsInRoom[propKey].GetFurthestFromDoor();
                if (retVal != null)
                {
                    return retVal.Coords;
                }
                else
                {
                    return Vector2Int.one * -1;

                }
                
            } 
        }
        return Vector2Int.one * -1;
    }

    /// <summary>
    /// Gets a prop to place in the room prioritizing size of the prop
    /// </summary>
    /// <param name="propsPlaced"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    public string GetPropToPlaceByLargest(Dictionary<string,int> propsPlaced,RoomTemplate template,List<string> toIgnore)
    {
        float size = 0;
        string retVal = string.Empty;
        foreach (KeyValuePair<string, RoomObjectPositions> kvp in AllObjectsInRoom)
        {
            if (toIgnore.Contains(kvp.Key))
            {
                continue;
            }
            if (propsPlaced[kvp.Key] < template.GetMaxQuantity(kvp.Key))
            {
                if (AllObjectsInRoom[kvp.Key].ObjectToPlace.Size().magnitude > size)
                {
                    size =  AllObjectsInRoom[kvp.Key].ObjectToPlace.Size().magnitude;
                    retVal = kvp.Key;
                }
            }
        }

        return retVal;
    }
}
public class RoomObjectPositions
{
    public EnvironmentObject ObjectToPlace;
    public List<RoomObjectPosition> ValidPositions=new List<RoomObjectPosition>();

    public RoomObjectPositions(string objectToPlace)
    {
        ObjectToPlace=ConstructableObjectManager.Instance.AllObjects[objectToPlace];
    }

    public RoomObjectPosition GetFurthestFromDoor()
    {
        RoomObjectPosition RetVal = null;
        float dist = 0f;
        for(int x = 0; x < ValidPositions.Count; x++)
        {
            
            if (ValidPositions[x].DoorWeight >= dist)
            {
                dist = ValidPositions[x].DoorWeight;
                RetVal = ValidPositions[x];
            }
        }


        return RetVal;
    }

    public void InitPositions(GeneratedRoom room,GeneratedBuilding building)
    {
        Vector2Int position = Vector2Int.zero;
        float wallWeight = 0, doorWeight = 0, propWeight = 0;
        Vector2Int propSize = ObjectToPlace.SizeAsVec2();
        for(int x = 0; x < room.size.x; x++)
        {
            for(int y = 0; y < room.size.y; y++)
            {
                position.x = x;
                position.y = y;
                if (EnvironmentObjectPlacementCriteriaHelpers.IsPositionValidForObject(ObjectToPlace, room, position, building))
                {
                    room.GetWeightsForPropPlacement(position, propSize, out doorWeight, out wallWeight, out propWeight);
                    ValidPositions.Add(new RoomObjectPosition(position, doorWeight,wallWeight, propWeight));
                }
            }
        }
        Debug.Log("Potential Positions: for " + ObjectToPlace.Name + " has " + ValidPositions.Count);
    }

    public void RefreshPositions(GeneratedRoom room, GeneratedBuilding building)
    {
        List<RoomObjectPosition> StillValidPositions = new List<RoomObjectPosition>();
        float wallWeight = 0, doorWeight = 0, propWeight = 0;
        Vector2Int propSize = ObjectToPlace.SizeAsVec2();
        for (int x = 0; x < ValidPositions.Count; x++)
        {
            if(EnvironmentObjectPlacementCriteriaHelpers.IsPositionValidForObject(ObjectToPlace, room, ValidPositions[x].Coords, building))
            {
                room.GetWeightsForPropPlacement(ValidPositions[x].Coords, propSize, out doorWeight, out wallWeight, out propWeight);
                StillValidPositions.Add(ValidPositions[x]);
                StillValidPositions[StillValidPositions.Count-1].UpdateWeights(doorWeight, wallWeight, propWeight);
            }
        }

        ValidPositions = StillValidPositions;
    }

   
}

public class RoomObjectPosition 
{
    public Vector2Int Coords;
    public float DoorWeight, WallWeight, PropWeight;
    public RoomObjectPosition(Vector2Int coords,float door,float wall,float prop)
    {
        Coords = coords;
        DoorWeight = door;
        WallWeight = wall;
        PropWeight= prop;
    }

    public void UpdateWeights(float door, float wall, float prop)
    {
        DoorWeight = door;
        WallWeight = wall;
        PropWeight = prop;
    }
    public bool IsPositionStillValid(Vector2Int newPos,int width,int height)
    {
        if (InRange(newPos.x, Coords.x, Coords.x + width) && InRange(newPos.y, Coords.y, Coords.y + height))
        {
            return false;
        }
        return false;
    }

    static bool InRange(int val,int min,int max)
    {
        return val >= min && val <= max;
    }
}
