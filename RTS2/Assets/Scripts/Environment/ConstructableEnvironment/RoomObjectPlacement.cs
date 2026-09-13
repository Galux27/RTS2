using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;
public class RoomObjectPlacement
{
    //Add prioritizing by criteria
    //e.g. higher priority for distance from door
    // public Dictionary<string,RoomObjectPositions> AllObjectsInRoom = new Dictionary<string, RoomObjectPositions>();

    public RoomObjectGrid GridOfPositions;

    public RoomObjectPlacement(RoomTemplate template,GeneratedRoom room,GeneratedBuilding building)
    {
        GridOfPositions = new RoomObjectGrid(room.size.x, room.size.y);
        Vector2Int position = Vector2Int.zero;
        EnvironmentObject currentObject = null;
        Vector2Int objSize = Vector2Int.zero;

        for (int q = 0; q < template.Props.Count; q++)
        {
            currentObject = ConstructableObjectManager.Instance.AllObjects[template.Props[q].PropName];
            objSize = currentObject.SizeAsVec2();
            objSize.x = Mathf.Max(1, objSize.x);
            objSize.y = Mathf.Max(1, objSize.y);

            for (int x = 0; x < room.size.x-objSize.x; x += objSize.x)
            {
                for (int y = 0; y < room.size.y-objSize.y; y += objSize.y)
                {
                    position.x = x;
                    position.y = y;

                    if (EnvironmentObjectPlacementCriteriaHelpers.IsPositionValidForObject(currentObject, room, position, building))
                    {

                        GridOfPositions.SetObjectCanBePlaced(position, template.Props[q].PropName, objSize);

                    }
                }
            }


        }
       
    }

    public void LogPositionsForProps()
    {
       
    }

    public void OnObjectPlaced(Vector2Int pos,string key,RoomTemplate template,GeneratedRoom room,GeneratedBuilding building)
    {
        EnvironmentObject currentObject = ConstructableObjectManager.Instance.AllObjects[key];
        Vector2Int objSize = currentObject.SizeAsVec2() ;
        GridOfPositions.OnPlaceObject(pos, key, objSize);
        Vector2Int position = Vector2Int.zero;

        for (int q = 0; q < template.Props.Count; q++)
        {
            currentObject = ConstructableObjectManager.Instance.AllObjects[template.Props[q].PropName];
            objSize = currentObject.SizeAsVec2();
            for (int x = pos.x-1; x < pos.x+objSize.x+1; x += 1)
            {
                for (int y = pos.y-1; y < pos.y+objSize.y+1; y += 1)
                {
                    position.x = x;
                    position.y = y;

                    if (EnvironmentObjectPlacementCriteriaHelpers.IsPositionValidForObject(currentObject, room, position, building))
                    {
                        GridOfPositions.SetObjectCanBePlaced(position, template.Props[q].PropName, objSize);
                    }
                }
            }


        }

    }

    public void RefreshObjectValidity(GeneratedBuilding building,GeneratedRoom room)
    {
       
    }

    public void LogPositionsForProp(string propKey)
    {
        
    }

    List<string> PotentialProps = new List<string>();
    public string GetPropThatCouldBePlacedAtCoordinate(Vector2Int coords,RoomTemplate template)
    {
        PotentialProps.Clear();
        for(int x = 0; x < template.Props.Count; x++)
        {
            if (GridOfPositions.GridElements[coords.x, coords.y].ObjectThatCouldBePlaced.Contains(template.Props[x].PropName))
            {
                PotentialProps.Add(template.Props[x].PropName);
            }
        }
        if (PotentialProps.Count == 0)
        {
            return string.Empty;
        }
        return PotentialProps[Random.Range(0, PotentialProps.Count)];
    }
    List<Vector2Int> potentialPropPositions = new List<Vector2Int>();
    public Vector2Int GetCoordinateForProp(string propKey)
    {
        potentialPropPositions.Clear();
        Vector2Int pos = Vector2Int.zero;
        for(int x = 0; x < GridOfPositions.GridElements.GetLength(0); x++)
        {
            for (int y = 0; y < GridOfPositions.GridElements.GetLength(1); y++)
            {
                pos.x = x;
                pos.y = y;
                if (GridOfPositions.GridElements[x, y].ObjectThatCouldBePlaced.Contains(propKey))
                {
                    potentialPropPositions.Add(pos);
                }
            }
        }
        if (potentialPropPositions.Count > 0)
        {
            return potentialPropPositions[Random.Range(0, potentialPropPositions.Count)];
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
        float size2 = 0f;
        for(int x = 0; x < template.Props.Count; x++)
        {
            if (propsPlaced[template.Props[x].PropName] >= template.GetMaxQuantity(template.Props[x].PropName) 
                || toIgnore.Contains(template.Props[x].PropName))
            {
                continue;
            }
            size2 = ConstructableObjectManager.Instance.AllObjects[template.Props[x].PropName].SizeAsVec2().magnitude;
            if (size2> size)
            {
                retVal= template.Props[x].PropName;
                size = size2;
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

    public bool DoesValidPositionsContainPosition(Vector2Int pos)
    {
        for(int x = 0; x < ValidPositions.Count; x++)
        {
            if (ValidPositions[x].Coords == pos)
            {
                return true;
            }
        }
        return false;
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

public class RoomObjectGrid
{
    public RoomObjectGridElement[,] GridElements;
    int width=0, height=0;
    public RoomObjectGrid(int width,int height)
    {
        GridElements=new RoomObjectGridElement[width,height];
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridElements[x, y] = new RoomObjectGridElement();
            }

        }
        this.width = width;
        this.height= height;
    }

    public void SetObjectCanBePlaced(Vector2Int pos,string key,Vector2Int size)
    {

        for (int x = pos.x; x < pos.x + size.x; x++)
        {
            for (int y = pos.y; y < pos.y + size.y; y++)
            {
                if (IsPosValid(x, y))
                {
                    GridElements[x, y].AddObject(key);
                }
            }
        }

    }

    public void OnPlaceObject(Vector2Int pos, string key, Vector2Int size)
    {
        for (int x = pos.x-1; x < pos.x + size.x+1; x++)
        {
            for (int y = pos.y-1; y < pos.y + size.y+1; y++)
            {
                if (IsPosValid(x, y))
                {
                    GridElements[x, y].ObjectThatCouldBePlaced.Clear() ;
                }
            }
        }
    }
    bool IsPosValid(int x,int y)
    {
        return x>=0&&x<width&& y>=0 && y<height;
    }
    public bool CanObjectBePlacedAtCoords(Vector2Int pos,string key)
    {
        return GridElements[pos.x, pos.y].ObjectThatCouldBePlaced.Contains(key);
    }


}

public class RoomObjectGridElement
{
    public HashSet<string> ObjectThatCouldBePlaced = new HashSet<string>();
    public void AddObject(string key)
    {
        ObjectThatCouldBePlaced.Add(key);
    }
}
