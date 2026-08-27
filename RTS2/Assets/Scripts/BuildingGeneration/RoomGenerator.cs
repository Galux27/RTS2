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
        GeneratedRoom room = new GeneratedRoom(size, pos,template.RoomID,id);
        GetTilesFromBuilding(room, building);

        PopulateWallTiles(room, template,building);
        GenerateLocationsForDoors(room);
        PopulateFloorTiles(room, template);
        PopulateRoomEnvObjects(room, template);
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
                room.RoomTiles[0, x].SetWall(template.Wall);
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

}
public class GeneratedRoomProp
{
    public string ID;
    public Vector2Int pos;
    public GeneratedRoomProp(string id,Vector2Int pos)
    {
        this.pos = pos;
        this.ID = id;
    }
}

public class RoomTile
{
    public string FloorTile, WallTile,DoorTile;
    public bool HasWall = false, HasFloor = false, HasDoor = false, IsEdge = false, HasProp = false,IsCorridor=false,IsValidForDoor=false;
    public int RoomID=-1;


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
    }


    public bool HasBeenUsed()
    {
        return RoomID >0;
    }

    public void ResetTile()
    {
        FloorTile = string.Empty;
        SetID(-1);
        ClearWall();
        HasFloor = false;
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