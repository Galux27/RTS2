using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenerator 
{

    public void GetTilesFromBuilding(GeneratedRoom room,GeneratedBuilding building)
    {
        //work out why this breaks the tiles 
   
        Vector2Int Origin = room.Position;
        Origin.x = Mathf.Clamp(Origin.x, 0, building.Tiles.GetLength(0) - 1);
        Origin.y = Mathf.Clamp(Origin.y, 0, building.Tiles.GetLength(1) - 1);
        for (int x = 0; x < room.size.x; x++)
        {
            for (int y = 0; y < room.size.y; y++)
            {
                try
                {
                    if (building.Tiles[x + Origin.x, y + Origin.y] != null)
                    {
                        room.RoomTiles[x, y].CopyData( building.Tiles[x + Origin.x, y + Origin.y]);
                    }
                }
                catch
                {
                    Debug.LogError("Out of range " + x + "," + y + "," + Origin + "," + building.Tiles.GetLength(0) + "," + building.Tiles.GetLength(1)+","+room.size);
                }
               
            }
        }
    }

    public virtual GeneratedRoom GenerateRoom(Vector2Int pos,Vector2Int size,RoomTemplate template,int id,GeneratedBuilding building)
    {
        Debug.Log("Generating room of size " + size + " at " + pos + " part of " + template.name);
        if (size.x < 3 || size.y < 3)
        {
            return null;
        }
        GeneratedRoom room = new GeneratedRoom(size, pos,template.RoomID,id);
        GetTilesFromBuilding(room, building);

        PopulateWallTiles(room, template,building);
        GenerateLocationsForDoors(room);
        PopulateFloorTiles(room, template);
        //PopulateRoomEnvObjects(room, template);
        if (template.CanHaveWindows)
        {
            GenerateWindows(room, template, building);
        }
        return room;
    }
    const int RoomPropIterations = 1000;

   
    public void GenerateWindows(GeneratedRoom room, RoomTemplate template, GeneratedBuilding building)
    {
        int width = room.RoomTiles.GetLength(0);
        int height = room.RoomTiles.GetLength(1);

        bool xTopEdge = false, yTopEdge = false, xBottomEdge = false, yBottomEdge = false ;
        if (room.Position.x + width >= building.Width)
        {
            xTopEdge = true;
        }
       
        if (room.Position.y + height >= building.Height)
        {
            yTopEdge = true;
        }
        if (room.Position.x ==0)
        {
            xBottomEdge = true;
        }   
        if(room.Position.y == 0)
        {
            yBottomEdge = true;
        }
        if (xTopEdge)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (y%3==0)
                {
                    room.RoomTiles[width-1, y].SetWall("Window");
                }
            }
        }
        
        if(yTopEdge)
        {
            for (int x = 1; x < width - 1; x++)
            {
                if(x%3==0)
                {
                    room.RoomTiles[x, height-1].SetWall("Window");
                }
            }
        }

        if (xBottomEdge)
        {
            for (int y = 1; y < height-1; y++)
            {
                if (y % 3 == 0)
                {
                    room.RoomTiles[0, y].SetWall("Window");
                }
            }
        }

        if (yBottomEdge)
        {
            for (int x = 1; x < width-1; x++)
            {
                if (x % 3 == 0)
                {
                    room.RoomTiles[x, 0].SetWall("Window");
                }
            }
        }
    }

    public void GenerateLocationsForDoors(GeneratedRoom room)
    {
        Debug.Log("Generating location for doors " + room.RoomTiles.GetLength(0) + "," + room.RoomTiles.GetLength(1));
        Vector2Int coords = new Vector2Int();
        coords.x=Random.Range(2,room.size.x-2);
        room.RoomTiles[coords.x, 0].IsValidForDoor = true ;
        coords.x = Random.Range(2, room.size.x - 2);
        room.RoomTiles[coords.x, room.size.y-1].IsValidForDoor = true;
        coords.y = Random.Range(2, room.size.y - 2);
        room.RoomTiles[0, coords.y].IsValidForDoor = true;
        coords.y = Random.Range(2, room.size.y - 2);
        room.RoomTiles[room.size.x-1, coords.y].IsValidForDoor = true;
       

    }

    void PopulateRoomEnvObjectsInGrid(GeneratedRoom room,RoomTemplate template)
    {
        RoomTemplateProp prop = null;
        Dictionary<string, int> propCounts = new Dictionary<string, int>();
        ConstructableObject obj = null;
        int width = 0, height = 0;
        for (int x = 0; x < template.Props.Count; x++)
        {
            propCounts.Add(template.Props[x].PropName, 0);
        }

        for (int yStart = room.size.y - 1; yStart > 0; yStart--)
        {
            for (int xStart = room.size.x - 1; xStart > 0; xStart--)
            {
                for (int p = 0; p < template.Props.Count; p++)
                {
                    prop = template.Props[p];
                    if (propCounts[prop.PropName] < prop.MaxQuantity)
                    {
                        obj = ConstructableObjectManager.Instance.AllObjects[prop.PropName];
                        width = obj.Width;
                        height = obj.Height;
                        if (prop.NeedsEdge)
                        {
                            width += 2;
                            height += 2;
                        }


                        bool valid = true;
                        if (xStart + width > room.size.x || yStart + height > room.size.y)
                        {
                            valid = false;
                        }
                        // if (valid)
                        bool foundWall = false;
                        bool foundDoor = false;
                        if (valid)
                        {
                            for (int x = xStart; x < xStart + width; x++)
                            {
                                for (int y = yStart - 1; y < yStart + height + 1; y++)
                                {
                                    if (room.IsValid(x, y))
                                    {
                                        if (x >= xStart && x < xStart + width && y >= yStart && y < yStart + width)
                                        {
                                            if (room.TileHasNothing(x, y) == false)
                                            {
                                                valid = false;
                                            }
                                        }
                                        else
                                        {
                                            if (room.RoomTiles[x, y].HasWall)
                                            {
                                                foundWall = true;
                                            }

                                        }

                                        if (room.RoomTiles[x, y].IsValidForDoor)
                                        {
                                            foundDoor = true;
                                        }

                                    }
                                    else
                                    {
                                        foundWall = true;
                                    }

                                }
                            }


                            if (prop.MustBeOnRoomEdge && !foundWall || foundDoor)
                            {
                                valid = false;
                            }

                            if (valid)
                            {
                                {
                                    for (int x = xStart; x < xStart + width; x++)
                                    {
                                        for (int y = yStart; y < yStart + height; y++)
                                        {

                                            room.RoomTiles[x, y].HasProp = true;
                                        }
                                    }
                                    room.AddEnvObject(room.RoomTiles[xStart,yStart],new GeneratedRoomProp(prop.PropName, new Vector2Int(xStart, yStart)));
                                    if (!propCounts.ContainsKey(prop.PropName))
                                    {
                                        propCounts.Add(prop.PropName, 0);
                                    }
                                    propCounts[prop.PropName]++;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void PopulateRoomEnvObjectsInGrid(GeneratedRoom room,GeneratedBuilding building,RoomTemplate template)
    {
        
        //could be a better way of working out how to populate the room, maybe store all the potential positions 
        //as a grid so they're easy to alter rather than going through a list of items every time
        //update the other props used in a warehouse too
        room.RefreshTileWeights();
        RoomObjectPlacement roomObjectPlacement = new RoomObjectPlacement(template, room, building);
        roomObjectPlacement.LogPositionsForProps();
        Dictionary<string, int> propCounts = new Dictionary<string, int>();

        for (int x = 0; x < template.Props.Count; x++)
        {
            propCounts.Add(template.Props[x].PropName, 0);
        }
        EnvironmentObject toPlace = null;
        string PropToPlace = string.Empty;
        Vector2Int pos = Vector2Int.zero;
        string potentialProp = string.Empty;
        Vector2Int size = Vector2Int.zero;
        for (int x = 0; x < room.size.x; x+=2) { 
            for(int y = 0; y < room.size.y; y+=2)
            {
                pos.x = x;
                pos.y = y;
                
                potentialProp=roomObjectPlacement.GetPropThatCouldBePlacedAtCoordinate(pos,template);
                if (potentialProp != string.Empty)
                {
                    
                    toPlace = ConstructableObjectManager.Instance.AllObjects[potentialProp];
                    size = toPlace.SizeAsVec2();
                    AddEnvObjectToRoomWithoutRefresh(room, pos, toPlace, building);
                   
                   // room.RefreshWeightsForProps(pos-size,size*2);
                    propCounts[potentialProp]++;

                    roomObjectPlacement.OnObjectPlaced(pos, potentialProp, template, room, building);
                    potentialProp = string.Empty;
                }
            }
        }

      
    }

    public void PopulateRoomEnvObjects(GeneratedRoom room,GeneratedBuilding building)
    {



        RoomTemplate template = BuildingDataManager.Instance.RoomTemplates[room.RoomType];

        if (template.Props.Count == 0)
        {
            return;
        }
        if (template.CanBeGridBased)
        {
            PopulateRoomEnvObjectsInGrid(room,building,template);
            return;
        }
        room.RefreshTileWeights();

        RoomObjectPlacement roomObjectPlacement = new RoomObjectPlacement(template, room, building);
        roomObjectPlacement.LogPositionsForProps();
        Dictionary<string, int> propCounts = new Dictionary<string, int>();

        for (int x = 0; x < template.Props.Count; x++)
        {
            propCounts.Add(template.Props[x].PropName, 0);
        }
        bool Done = false;
        RoomTemplateProp currentProp = null;
        EnvironmentObject toPlace = null;
        int attempts = 0;
        bool runOutOfPropsToPlace = false;
        string PropToPlace = string.Empty;
        int maxAttemtps = 100;
        List<string> PropsFailedToPlace = new List<string>();

        while (!Done&&attempts<maxAttemtps)
        {

            currentProp = template.GetPropByName(roomObjectPlacement.GetPropToPlaceByLargest(propCounts,template,PropsFailedToPlace));

            if (currentProp!=null)
            {
                Vector2Int posToPlace = roomObjectPlacement.GetCoordinateForProp(currentProp.PropName);

                if (posToPlace.x >= 0)
                {
                    toPlace = ConstructableObjectManager.Instance.AllObjects[currentProp.PropName];

                    AddEnvObjectToRoom(room, posToPlace, toPlace, building);

                    propCounts[currentProp.PropName]++;

                    roomObjectPlacement.OnObjectPlaced(posToPlace, currentProp.PropName, template, room, building);

                    PropsFailedToPlace.Clear();
                }
                else
                {
                    if (!PropsFailedToPlace.Contains(currentProp.PropName))
                    {
                        PropsFailedToPlace.Add(currentProp.PropName);
                        roomObjectPlacement.LogPositionsForProp(currentProp.PropName);
                    }

                }
            }
            else
            {
                Done = true;
            }
            attempts++;
        }
    }

    void AddEnvObjectToRoomWithoutRefresh(GeneratedRoom room, Vector2Int pos, EnvironmentObject objectToAdd, GeneratedBuilding building)
    {
        room.AddEnvObject(room.RoomTiles[pos.x, pos.y], new GeneratedRoomProp(objectToAdd.Name, pos));
        for (int x = pos.x; x < pos.x + objectToAdd.Width; x++)
        {
            for (int y = pos.y; y < pos.y + objectToAdd.Height; y++)
            {
                room.RoomTiles[x, y].HasProp = true;
            }
        }

    }

    void AddEnvObjectToRoom(GeneratedRoom room,Vector2Int pos,EnvironmentObject objectToAdd,GeneratedBuilding building)
    {
        room.AddEnvObject(room.RoomTiles[pos.x, pos.y], new GeneratedRoomProp(objectToAdd.Name,pos));
        for(int x = pos.x; x < pos.x + objectToAdd.Width; x++)
        {
            for(int y = pos.y; y < pos.y + objectToAdd.Height; y++)
            {
                if (room.IsValid(x, y))
                {
                    room.RoomTiles[x, y].HasProp = true;
                }
            }
        }

        room.RefreshWeightsForProps();
    }




    public void PopulateRoomEnvObjects(GeneratedRoom room, RoomTemplate template)
    {
        if (template.CanBeGridBased)
        {
            PopulateRoomEnvObjectsInGrid(room, template);
            return;
        }

        RoomTemplateProp prop = null;
        Dictionary<string, int> propCounts = new Dictionary<string, int>();
        ConstructableObject obj = null;
        int width=0, height=0;
        for (int x = 0; x < template.Props.Count; x++) {
            propCounts.Add(template.Props[x].PropName, 0);
        }

        for (int yStart = room.size.y - 1; yStart > 0; yStart--)
        {
            for (int xStart = room.size.x - 1; xStart > 0; xStart--)
            {
                for (int p = 0; p < template.Props.Count; p++)
                {
                    prop = template.Props[p];
                    if(propCounts[prop.PropName] < prop.MaxQuantity)
                    {
                        obj=ConstructableObjectManager.Instance.AllObjects[prop.PropName];
                        width = obj.Width;
                        height = obj.Height;
                        if (prop.NeedsEdge)
                        {
                            width += 2;
                            height += 2;
                        }

                        
                        bool valid = true;
                        if (xStart + width > room.size.x || yStart + height > room.size.y)
                        {
                            valid = false;
                        }
                        // if (valid)
                        bool foundWall = false;
                        bool foundDoor = false;
                        if(valid)
                        {
                          for(int x = xStart-1; x < xStart + width+1; x++)
                          {
                                for(int y=yStart-1;y< yStart + height+1; y++)
                                {
                                    if (room.IsValid(x, y))
                                    {
                                        if (x >= xStart && x < xStart + width&& y >= yStart && y < yStart + width)
                                        {
                                            if (room.TileHasNothing(x, y) == false)
                                            {
                                                valid = false;
                                            }
                                        }
                                        else
                                        {
                                            if (room.RoomTiles[x, y].HasWall)
                                            {
                                                foundWall = true;
                                            }

                                        }

                                        if (room.RoomTiles[x, y].IsValidForDoor)
                                        {
                                            foundDoor = true;
                                        }

                                    }
                                    else
                                    {
                                        foundWall = true;
                                    }
                                   
                                }
                            }


                            if (prop.MustBeOnRoomEdge && !foundWall ||foundDoor)
                            {
                                valid = false;
                            }
                       
                            if (valid)
                            {
                                {
                                    for (int x = xStart; x < xStart + width; x++)
                                    {
                                        for (int y = yStart; y < yStart + height; y++)
                                        {
                                            
                                            room.RoomTiles[x, y].HasProp = true;
                                        }
                                    }
                                    room.AddEnvObject(room.RoomTiles[xStart,yStart],new GeneratedRoomProp(prop.PropName, new Vector2Int(xStart, yStart)));
                                    if (!propCounts.ContainsKey(prop.PropName))
                                    {
                                        propCounts.Add(prop.PropName, 0);
                                    }
                                    propCounts[prop.PropName]++;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void PopulateFloorTiles(GeneratedRoom room, RoomTemplate template)
    {
        int width = room.RoomTiles.GetLength(0)-1;
        int height = room.RoomTiles.GetLength(1)-1;
        for (int x = 0; x < width; x++)
        {
          for(int y = 0; y < height; y++)
            {
                

                    room.RoomTiles[x, y].SetFloor(template.Floor);
              
            }

        }
    }


    public void PopulateWallTilesThatAreOnExteriorOfBuilding(GeneratedRoom room, RoomTemplate template, GeneratedBuilding building)
    {
        int width = room.RoomTiles.GetLength(0);
        int height = room.RoomTiles.GetLength(1);

        bool isXedge = false, isYedge = false;
        if (room.Position.x + width >= building.Width)
        {
            isXedge = true;
        }
        if (room.Position.y + height >= building.Height)
        {
            isYedge = true;
        }

        for (int x = 0; x < width; x++)
        {
            if (room.Position.y == building.Position.y)
            {
                room.RoomTiles[x, 0].SetWall(template.Wall);
            }
            if (isYedge)
            {
                room.RoomTiles[x, height - 1].SetWall(template.Wall);
            }

        }

        for (int x = 0; x < height; x++)
        {
            if (room.Position.x == building.Position.x)
            {
                room.RoomTiles[0, x].SetWall(template.Wall);
            }
            if (isXedge || template.CanHaveInternalWalls)
            {
                room.RoomTiles[width - 1, x].SetWall(template.Wall);
            }
        }
    }

 


    void PopulateWallTiles(GeneratedRoom room,RoomTemplate template,GeneratedBuilding building)
    {
        if (!template.CanGenerateAnyWalls)
        {
            PopulateWallTilesThatAreOnExteriorOfBuilding(room, template, building);
            return;
        }
        int width = room.RoomTiles.GetLength(0);
        int height = room.RoomTiles.GetLength(1);

        bool isXedge=false,isYedge=false,isLowXEdge=false,isLowYEdge=false;
        if(room.Position.x+width>=building.Width)
        {
            isXedge=true;
        }
        if (room.Position.y + height >= building.Height)
        {
            isYedge=true;
        }

        if (room.Position.x == 0)
        {
            isLowXEdge = true;
        }
        Debug.Log("generating room " + room.Position + "," + building.Position);
        if (room.Position.y == 0)
        {
            isLowYEdge = true;
        }

        for (int x = 0; x < width; x++)
        {

            if (isLowYEdge || template.CanHaveInternalWalls)
            {
                room.RoomTiles[x, 0].SetWall(template.Wall);
            }
            if (isYedge)
            {
                room.RoomTiles[x, height - 1].SetWall(template.Wall);
            }
            else if (template.CanHaveInternalWalls)
            {
                room.RoomTiles[x, height - 1].SetWall(template.Wall);

            }

        }

        for (int x = 0; x < height; x++)
        {
            if (isLowXEdge || template.CanHaveInternalWalls)
            {
                try
                {
                    room.RoomTiles[0, x].SetWall(template.Wall);
                }catch(System.Exception e)
                {
                    Debug.LogError(e.ToSafeString());
                    Debug.LogError("error setting room tiles x was " + x + " dimensions " + width + "," + height + "," + room.RoomTiles.GetLength(0) + "," + room.RoomTiles.GetLength(1));
                }
            }
            if (isXedge)
            {
                room.RoomTiles[width - 1, x].SetWall(template.Wall);
            }else if (template.CanHaveInternalWalls)
            {
                room.RoomTiles[width - 1, x].SetWall(template.Wall);

            }
        }
    }
}
[System.Serializable]
public class GeneratedRoom
{
    public string RoomType;
    public RoomTile[,] RoomTiles;
    public Vector2Int Position;
    public Vector2Int size;
    public Dictionary<RoomTile,GeneratedRoomProp> EnvObjects;
    public int RoomID = -1;
    public GeneratedRoom(Vector2Int size,Vector2Int pos,string type,int ID)
    {
        RoomID = ID;
        RoomType = type;
        RoomTiles = new RoomTile[size.x, size.y];
        for(int x=0; x<size.x; x++)
        {
            for(int y=0; y<size.y; y++)
            {
                RoomTiles[x,y]= new RoomTile();
                RoomTiles[x, y].SetID(RoomID);
            }
        }

        Position = pos;
        this.size = size;
    }

    public bool IsPartOfRoomInArea(Vector2Int min,Vector2Int max,GeneratedBuilding building)
    {
        return PointInRange(min, max,building.Position+ Position) || PointInRange(min, max, building.Position + Position + size);
    }


    /// <summary>
    /// pass in coordinates and gets a room that contains all the area of this room within its bounds
    /// </summary>
    /// <param name="min">low bound in world coords</param>
    /// <param name="max">high bound in world coords</param>
    /// <param name="building">building the room is part of</param>
    /// <returns></returns>
    public GeneratedRoom TakeSliceOfRoom(Vector2Int min,Vector2Int max,GeneratedBuilding building)
    {
        //need to add props & add something to refresh adjacent walls as some are jank


        Vector2Int RoomPosition = Position;
        Vector2Int roomStart = building.Position + RoomPosition;
        Vector2Int roomMax = building.Position+ Position + size;
        //gets intersection of squares
         int x5 =Mathf.Max(min.x, roomStart.x);
         int y5 = Mathf.Max(min.y, roomStart.y);
         int x6 = Mathf.Min(max.x, roomMax.x);
         int y6 = Mathf.Min(max.y, roomMax.y);
        if (x5>x6||y5>y6)
        {
            return null;
        }
        int width = x6 - x5;
        int height = y6 - y5;

        int localXStart = Mathf.RoundToInt( Mathf.InverseLerp(roomStart.x, roomMax.x, x5)*size.x);
        int localYStart = Mathf.RoundToInt(Mathf.InverseLerp(roomStart.y, roomMax.y, y5) * size.y);
        GeneratedRoom room = new GeneratedRoom(new Vector2Int(width, height), new Vector2Int(x5, y5), RoomType, RoomID);

        Debug.Log("Room Slice: room area " + roomStart + " to " + roomMax + " area checking " + min + " to " + max+" intersection "+ x5+","+y5+" to "+ x6+","+y6+" local "+localXStart+","+localYStart+" size " + width+","+height+" room size "+ size);

        Debug.Log("Taking slice of room " + x5 + "," + y5 + " max " + x6 + "," + y6 + ",local" + localXStart + 
            "," + localYStart+" building pos " +building.Position+",size b "+building.Width+","+building.Height+", room size"+this.size
            +" area start " + min+" area end " +max+",slice size "+ width+","+height+" room pos "+ room.Position);

     //code isn't working because the the conversion is done from the original building and the application is done in the sliced building
     //need to write something to convert between the building passed into here and the building its applied to

        Vector2Int position = Vector2Int.zero;
        int newX = localXStart, newY = localYStart;

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                room.RoomTiles[x, y].CopyData(RoomTiles[newX, newY]);

                if (EnvObjects!=null && EnvObjects.ContainsKey(RoomTiles[newX, newY]))
                {
                    //subtracting room position to get around stupid corner I've painted myself into with how they're placed in the world
                    room.AddEnvObject(room.RoomTiles[x, y],new GeneratedRoomProp( EnvObjects[RoomTiles[newX, newY]].ID, new Vector2Int(x,y),true));
                }
                newY++;
            }
            newX++;
            newY = localYStart;
        }

        
      


        return room;
    }

   

    bool PointInRange(Vector2Int min,Vector2Int max,Vector2Int pos)
    {
        return pos.x>=min.x&&pos.x<=max.x && pos.y>=min.y&&pos.y<=max.y;
    }

    public void SetAsCorridor()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                RoomTiles[x, y].IsCorridor = true;
            }
        }

    }


    public void ResetRoom(Vector2Int position,Vector2Int size)
    {
        float startX = Mathf.InverseLerp(this.Position.x,this.Position.x+this.size.x, position.x);
        float startY = Mathf.InverseLerp(this.Position.y, this.Position.y + this.size.y, position.y);
        float endX = Mathf.InverseLerp(this.Position.x, this.Position.x + this.size.x, position.x+size.x);
        float endY = Mathf.InverseLerp(this.Position.y, this.Position.y + this.size.y, position.y + size.y);

        int xStart = Mathf.FloorToInt(Mathf.Lerp(0,this.size.x,startX));
        int xEnd = Mathf.FloorToInt(Mathf.Lerp(0, this.size.x, endX));
        int yStart = Mathf.FloorToInt(Mathf.Lerp(0, this.size.y, startY));
        int yEnd = Mathf.FloorToInt(Mathf.Lerp(0, this.size.y,endY));

        for (; xStart < xEnd; xStart++)
        {
            for(int y = yStart; y < yEnd; y++)
            {
                RoomTiles[xStart, y].ResetTile();
                CheckToRemoveEnvObject(RoomTiles[xStart, y]);
            }
        }

    }

    void CheckToRemoveEnvObject(RoomTile tile)
    {
        if (EnvObjects == null)
        {
            return;
        }
        if (EnvObjects.ContainsKey(tile))
        {
            EnvObjects.Remove(tile);
        }
    }

    public Vector2Int GetEdgeCoord()
    {
        int x = 0, y = 0;
        if (Random.Range(0f, 100f) < 50f)
        {
            x = Random.Range(0, RoomTiles.GetLength(0));
            y = RoomTiles.GetLength(1) - 1;
        }
        else
        {
            y = Random.Range(0, RoomTiles.GetLength(1));
            x = RoomTiles.GetLength(0) - 1;
        }
        return new Vector2Int(Position.x + x, Position.y + y);
    }

    public void AddEnvObject(RoomTile tile,GeneratedRoomProp prop)
    {
        if (EnvObjects == null)
        {
            EnvObjects = new Dictionary<RoomTile, GeneratedRoomProp>();
        }
        if (EnvObjects.ContainsKey(tile))
        {
            return;
        }
        EnvObjects.Add(tile,prop);
    }

    public bool IsValid(int x,int y)
    {
        return x >= 0 && y >= 0 && x < size.x && y < size.y;
    }

    public bool TileHasNothing(int x,int y)
    {
        return RoomTiles[x, y].HasWall == false 
            && RoomTiles[x, y].HasDoor == false 
            && RoomTiles[x, y].HasProp == false&&RoomTiles[x,y].IsValidForDoor==false;
    }


    public void RefreshTileWeights()
    {
        List<Vector2Int> DoorCoordinates = new List<Vector2Int>();
        List<Vector2Int> WallCoordinates = new List<Vector2Int>();

        for(int x = 0; x < RoomTiles.GetLength(0); x++)
        {
            for (int y = 0; y < RoomTiles.GetLength(1); y++)
            {
                if (RoomTiles[x, y].HasDoor)
                {
                    DoorCoordinates.Add(new Vector2Int(x, y));
                }

                if (RoomTiles[x, y].HasWall)
                {
                    WallCoordinates.Add(new Vector2Int(x, y));
                }
            }
        }
        Vector2Int curPos = Vector2Int.zero;
        for (int x = 0; x < RoomTiles.GetLength(0); x++)
        {
            for (int y = 0; y < RoomTiles.GetLength(1); y++)
            {
                curPos.x = x;
                curPos.y = y;

                if (RoomTiles[x, y].HasDoor)
                {
                    RoomTiles[x, y].AvgDistToDoor = 0;
                }
                else
                {
                    RoomTiles[x, y].AvgDistToDoor = GetAvgOfPositionList(DoorCoordinates, curPos);

                }
                if (RoomTiles[x, y].HasWall)
                {
                    RoomTiles[x, y].AvgDistToWall = 0;

                }
                else
                {
                    RoomTiles[x, y].AvgDistToWall = GetAvgOfPositionList(WallCoordinates, curPos);

                }

            }
        }
    }

    public void RefreshTileWeightsInRange(Vector2Int position,Vector2Int size)
    {
        List<Vector2Int> DoorCoordinates = new List<Vector2Int>();
        List<Vector2Int> WallCoordinates = new List<Vector2Int>();

        for (int x = position.x; x < position.x+size.x; x++)
        {
            for (int y = position.y; y < position.y + size.y; y++)
            {
                if (!IsValid(x, y))
                {
                    continue;
                }
                if (RoomTiles[x, y].HasDoor)
                {
                    DoorCoordinates.Add(new Vector2Int(x, y));
                }

                if (RoomTiles[x, y].HasWall)
                {
                    WallCoordinates.Add(new Vector2Int(x, y));
                }
            }
        }
        Vector2Int curPos = Vector2Int.zero;
        for (int x = position.x; x < position.x + size.x; x++)
        {
            for (int y = position.y; y < position.y + size.y; y++)
            {
                if (!IsValid(x, y))
                {
                    continue;
                }
                curPos.x = x;
                curPos.y = y;

                if (RoomTiles[x, y].HasDoor)
                {
                    RoomTiles[x, y].AvgDistToDoor = 0;
                }
                else
                {
                    RoomTiles[x, y].AvgDistToDoor = GetAvgOfPositionList(DoorCoordinates, curPos);

                }
                if (RoomTiles[x, y].HasWall)
                {
                    RoomTiles[x, y].AvgDistToWall = 0;

                }
                else
                {
                    RoomTiles[x, y].AvgDistToWall = GetAvgOfPositionList(WallCoordinates, curPos);

                }

            }
        }
    }


    float GetAvgOfPositionList(List<Vector2Int> positions,Vector2Int coords)
    {
        if (positions.Count == 0)
        {
            return 0;
        }
        float retVal = 0f;
        for(int x = 0; x < positions.Count; x++)
        {
            retVal += Vector2Int.Distance(positions[x], coords);
        }

        return retVal/positions.Count;
    }

    public void GetWeightsForPropPlacement(Vector2Int position,Vector2Int size,out float doorWeight,out float wallWeight,out float propWeight)
    {
        doorWeight = 99999;
        wallWeight = 99999;
        propWeight = 99999;
        int totalTiles = size.x * size.y;
        for(int x = position.x; x < position.x + size.x; x++)
        {
            for(int y = position.y; y < position.y + size.y; y++)
            {
               
                doorWeight = Mathf.Min(doorWeight, RoomTiles[x, y].AvgDistToDoor);
                wallWeight = Mathf.Min(wallWeight,RoomTiles[x, y].AvgDistToWall);
                propWeight = Mathf.Min(propWeight,RoomTiles[x, y].AvgDistToProp);

            }
        }
        
    }

    public void RefreshWeightsForProps(Vector2Int position,Vector2Int size)
    {
        List<Vector2Int> PropCoordinates = new List<Vector2Int>();

        for (int x = position.x; x < position.x + size.x; x++)
        {
            for (int y = position.y; y < position.y + size.y; y++)
            {
                if (!IsValid(x, y))
                {
                    continue;
                }
                if (RoomTiles[x, y].HasProp)
                {
                    PropCoordinates.Add(new Vector2Int(x, y));
                }

            }
        }
        Vector2Int curPos = Vector2Int.zero;

        for (int x = position.x; x < position.x + size.x; x++)
        {
            for (int y = position.y; y < position.y + size.y; y++)
            {
                if (!IsValid(x, y))
                {
                    continue;
                }
                curPos.x = x;
                curPos.y = y;
                if (RoomTiles[x, y].HasProp)
                {
                    RoomTiles[x, y].AvgDistToProp = 0;
                }
                else
                {
                    RoomTiles[x, y].AvgDistToProp = GetAvgOfPositionList(PropCoordinates, curPos);
                }

            }
        }
    }

    public void RefreshWeightsForProps()
    {
        List<Vector2Int> PropCoordinates = new List<Vector2Int>();

        for (int x = 0; x < RoomTiles.GetLength(0); x++)
        {
            for (int y = 0; y < RoomTiles.GetLength(1); y++)
            {
                if (RoomTiles[x, y].HasProp)
                {
                    PropCoordinates.Add(new Vector2Int(x, y));
                }

            }
        }
        Vector2Int curPos = Vector2Int.zero;

        for (int x = 0; x < RoomTiles.GetLength(0); x++)
        {
            for (int y = 0; y < RoomTiles.GetLength(1); y++)
            {
                curPos.x = x;
                curPos.y = y;
                if (RoomTiles[x, y].HasProp)
                {
                    RoomTiles[x, y].AvgDistToProp = 0;
                }
                else
                {
                    RoomTiles[x, y].AvgDistToProp=GetAvgOfPositionList(PropCoordinates,curPos);
                }

            }
        }
    }

}
public class GeneratedRoomProp
{
    public string ID;
    public Vector2Int pos;
    public bool IsFromSlice = false;
    public GeneratedRoomProp(string id,Vector2Int pos,bool fromSlice=false)
    {
        this.pos = pos;
        this.ID = id;
        IsFromSlice=fromSlice;
    }
}

[System.Serializable]
public class RoomTile
{
    public string FloorTile, WallTile,DoorTile;
    public bool HasWall = false, HasFloor = false, HasDoor = false, IsEdge = false, HasProp = false,IsCorridor=false,IsValidForDoor=false;
    public int RoomID=-1;

    public float AvgDistToWall=0f,AvgDistToDoor,AvgDistToProp=0f;

    public void CopyData(RoomTile toCopy)
    {
        if (toCopy.HasDoor)
        {
            SetDoor(toCopy.DoorTile);
        }
        if (toCopy.HasFloor)
        {
            SetFloor(toCopy.FloorTile);
        }

        if (toCopy.HasWall)
        {
            SetWall(toCopy.WallTile);
        }

        if (toCopy.IsEdge)
        {
            IsEdge = true;
        }

        AvgDistToDoor = toCopy.AvgDistToDoor;
        AvgDistToWall = toCopy.AvgDistToWall;
        AvgDistToProp = toCopy.AvgDistToProp;
        
    }


    public bool HasBeenUsed()
    {
        return RoomID >0;
    }

    public void ResetTile(bool resetFloor=false)
    {
        if (resetFloor)
        {
            FloorTile = string.Empty;
            HasFloor = false;
        }
        SetID(-1);
        ClearWall();
        HasDoor = false;
        HasProp = false;
        IsCorridor = false;
        IsValidForDoor = false;
        
    }

   

    public void SetID(int id)
    {
        RoomID = id;
    }

    public void ClearWall()
    {
        WallTile = "";
        HasWall = false;
    }
    public void SetWall(string type)
    {
        WallTile = type;
        HasWall = true;
    }

    public void SetDoor(string type)
    {
        DoorTile = type;
        HasDoor = true;
    }

    public void SetFloor(string type)
    {
        if (HasFloor)
        {
            Debug.Log("Replacing " + FloorTile + " with " + type);
        }
        FloorTile = type;
        HasFloor = true;
    }

}