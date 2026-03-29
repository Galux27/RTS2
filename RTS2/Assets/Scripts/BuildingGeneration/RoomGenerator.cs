using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomGenerator 
{
    public virtual GeneratedRoom GenerateRoom(Vector2Int pos,Vector2Int size,RoomTemplate template,int id,GeneratedBuilding building)
    {
        GeneratedRoom room = new GeneratedRoom(size, pos,template.RoomID,id);
        PopulateWallTiles(room, template,building);
        GenerateLocationsForDoors(room);
        PopulateFloorTiles(room, template);
        PopulateRoomEnvObjects(room, template);
        return room;
    }
    const int RoomPropIterations = 1000;

   
    public void GenerateLocationsForDoors(GeneratedRoom room)
    {
        Vector2Int coords = new Vector2Int();
        coords.x=Random.Range(1,room.size.x-1);
        room.RoomTiles[coords.x, 0].IsValidForDoor = true ;
        coords.x = Random.Range(1, room.size.x - 1);
        room.RoomTiles[coords.x, room.size.y-1].IsValidForDoor = true;
        coords.y = Random.Range(1, room.size.y - 1);
        room.RoomTiles[0, coords.y].IsValidForDoor = true;
        coords.y = Random.Range(1, room.size.y - 1);
        room.RoomTiles[room.size.x-1, coords.y].IsValidForDoor = true;
    }


    public void PopulateRoomEnvObjects(GeneratedRoom room, RoomTemplate template)
    {
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
                                    room.AddEnvObject(new GeneratedRoomProp(prop.PropName, new Vector2Int(xStart, yStart)));
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
        int width = room.RoomTiles.GetLength(0);
        int height = room.RoomTiles.GetLength(1);
        for (int x = 0; x < width; x++)
        {
          for(int y = 0; y < height; y++)
            {
                

                    room.RoomTiles[x, y].SetFloor(template.Floor);
              
            }

        }
    }

    void PopulateWallTiles(GeneratedRoom room,RoomTemplate template,GeneratedBuilding building)
    {
        int width = room.RoomTiles.GetLength(0);
        int height = room.RoomTiles.GetLength(1);

        bool isXedge=false,isYedge=false;
        if(room.Position.x+width>=building.Width)
        {
            isXedge=true;
        }
        if (room.Position.y + height >= building.Height)
        {
            isYedge=true;
        }

        for (int x = 0; x < width; x++)
        {
            room.RoomTiles[x, 0].SetWall(template.Wall);
            if (isYedge)
            {
                room.RoomTiles[x, height - 1].SetWall(template.Wall);
            }
           
        }

        for (int x = 0; x < height; x++)
        {
            room.RoomTiles[0, x].SetWall(template.Wall);
            if (isXedge)
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
    public List<GeneratedRoomProp> EnvObjects;
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

    public void AddEnvObject(GeneratedRoomProp prop)
    {
        if (EnvObjects == null)
        {
            EnvObjects = new List<GeneratedRoomProp>();
        }
        EnvObjects.Add(prop);
    }

    public bool IsValid(int x,int y)
    {
        return x >= 0 && y >= 0 && x < size.x && y < size.y;
    }

    public bool TileHasNothing(int x,int y)
    {
        return RoomTiles[x, y].HasWall == false && RoomTiles[x, y].HasDoor == false && RoomTiles[x, y].HasProp == false&&RoomTiles[x,y].IsValidForDoor==false;
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
        Debug.Log("Room Prop: " + id + " at " + pos);
    }
}

public class RoomTile
{
    public string FloorTile, WallTile,DoorTile;
    public bool HasWall = false, HasFloor = false, HasDoor = false, IsEdge = false, HasProp = false,IsCorridor=false,IsValidForDoor=false;
    public int RoomID;


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
        FloorTile = type;
        HasFloor = true;
    }

}