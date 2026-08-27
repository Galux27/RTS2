using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingFloorplan 
{
    public static BuildingFloorplan GetFloorplanByType(BuildingFloorplanType typeToGet)
    {
        switch (typeToGet)
        {
            case BuildingFloorplanType.Basic:
                return new BuildingFloorplan();
                break;
            case BuildingFloorplanType.Square:
                return new SquareBuildingFloorplan(10, Vector2Int.one * 5);
                break;
            case BuildingFloorplanType.SquareNoSplit:
                return new SquareBuildingNoSplitFloorplan();
                break;
            case BuildingFloorplanType.Corridors:
                return new CorridorBasedFloorplan();
            default:
                return new BuildingFloorplan();
                break;
        }
    }

    static List<Vector2Int> RandomList = new List<Vector2Int>();
    public static Vector2Int GetValidEdgeCoordinatesForRoom(int width,int height,GeneratedBuilding building)
    {
        RandomList.Clear();
        int y1 = 0, y2 = building.Height - height;
        bool Valid = true;
        for (int q = 0; q < building.Width - width; q++)
        {
            for (int x = q; x < building.Width; x++)
            {
                for (int y = y1; y < y1 + height; y++)
                {
                    if (building.Tiles[x, y]!=null&&building.Tiles[x, y].HasBeenUsed())
                    {
                        Valid = false;
                        break;
                    }

                }
                if (!Valid)
                {
                    break;
                }
            }

            if (Valid)
            {
                RandomList.Add(new Vector2Int(q, y1));
            }
            else
            {
                Valid = true;
            }

            for (int x = q; x < building.Width; x++)
            {
                for (int y = y2; y < y2 + height; y++)
                {
                    if (building.Tiles[x, y] != null && building.Tiles[x, y].HasBeenUsed())
                    {
                        Valid = false;
                        break;
                    }

                }
                if (!Valid)
                {
                    break;
                }
            }

            if (Valid)
            {
                RandomList.Add(new Vector2Int(q, y2));
            }
            else
            {
                Valid = true;
            }
        }
        if (RandomList.Count > 0)
        {
            Vector2Int retVal = RandomList[Random.Range(0, RandomList.Count)];
            return retVal;
        }
        return new Vector2Int(-1, -1);
    }
    
    bool AreCoordsOnEdge(int x,int y,GeneratedBuilding building)
    {
        return x==0||y==0||x==building.Width-1||y==building.Height-1;
    }

    public Vector2Int GetStartingCoordsForRoom(int width,int height,GeneratedBuilding building)
    {
        int y1 = 0,y2=building.Height - height;
        bool Valid = true;
        for (int q = 0; q < building.Width - width; q++)
        {
            for (int x = q; x < building.Width; x++)
            {
                for (int y = y1; y < y1 + height; y++)
                {
                    if (building.Tiles[x, y].HasBeenUsed())
                    {
                        Valid = false;
                        break;
                    }
                   
                }
                if (!Valid)
                {
                    break;
                }
            }

            if (Valid)
            {
                return new Vector2Int(q, y1);
            }
            else
            {
                Valid = true;
            }

            for (int x = q; x < building.Width; x++)
            {
                for (int y = y2; y < y2 + height; y++)
                {
                    if (building.Tiles[x, y].HasBeenUsed())
                    {
                        Valid = false;
                        break;
                    }

                }
                if (!Valid)
                {
                    break;
                }
            }

            if (Valid)
            {
                return new Vector2Int(q, y2);
            }
            else
            {
                Valid = true;
            }
        }
        return new Vector2Int(-1, -1);

    }


    public virtual GeneratedBuilding Generate(RoomGenerator RoomGen,int width,int height,Vector2Int pos,BuildingTemplate template,int maxPasses)
    {
        Debug.Log("Building Gen: Generating building at " + pos + " width " + width + "," + height );
        GeneratedBuilding building = new GeneratedBuilding(width, height, pos,template.BuildingName );
        int count = 0;
        GeneratedRoom curRoom = null;
        Vector2Int startPosition = building.GetEdgeOrStart(new Vector2Int(building.Width, building.Height));
        Vector2Int modifier = Vector2Int.zero;
        TShapeCorridorGenerator corridor = new TShapeCorridorGenerator();
        corridor.GenerateCorridor(new Vector2Int(width / 2, height / 2), building, 3, template);
        building.UpdateEdgeTiles();
        RoomTemplate roomTemplate=null;
        while (count < maxPasses && !building.HasFinishedBuildingGen(template))
        {
            roomTemplate = building.GetRoomToGenerate(template);
            if (roomTemplate != null)
            {
                width = Random.Range(roomTemplate.MinWidth, roomTemplate.MaxWidth);
                height = Random.Range(roomTemplate.MinHeight, roomTemplate.MaxHeight);
                if (building.GetValidStartPosition(new Vector2Int(width, height), out startPosition, out modifier))
                {

                    curRoom = RoomGen.GenerateRoom(startPosition + new Vector2Int((width - 1) * modifier.x, (height - 1) * modifier.y), new Vector2Int(width, height), roomTemplate, building.MyRooms.Count,building);
                    Debug.Log("Building Gen: generated building room at " + curRoom.Position+","+curRoom.size);

                    building.AddRoom(curRoom);
                }
                else
                {
                    Debug.Log("Building Gen: failed to generate room of size " + width + "," + height+" from area");

                }
                // startPosition = building.GetEdgeOrStart(new Vector2Int(width, height));

            }
            count++;
        }
        building.UpdateCorridorEdgeTiles(template);
        building.GenerateDoors();
        return building;
    }
}

public class SquareBuildingFloorplan : BuildingFloorplan 
{
    int maxSplits;
    Vector2Int minRoomSize;

    public SquareBuildingFloorplan(int maxSplits,Vector2Int minRoomSize)
    {
        this.maxSplits = maxSplits;
        this.minRoomSize = minRoomSize;
    }
    Vector2Int size;
    public override GeneratedBuilding Generate(RoomGenerator RoomGen, int width, int height, Vector2Int pos, BuildingTemplate template, int maxPasses)
    {
        Debug.Log("Building Gen: Generating building at " + pos + " width " + width + "," + height);

        size = new Vector2Int(width, height);
        GeneratedBuilding building = new GeneratedBuilding(width, height,pos, template.BuildingName);
        int count = 0;
        GeneratedRoom curRoom = null;
        Vector2Int modifier = Vector2Int.zero;
        List<SplitRoom> CurrentSplits = new List<SplitRoom>();
        CurrentSplits.Add(new SplitRoom(new Vector2Int(0,0), new Vector2Int(width, height)));
        List<SplitRoom> potentialNewSplits=new List<SplitRoom>();

        while (count<maxPasses && CurrentSplits.Count<maxSplits)
        {
            int index = Random.Range(0, CurrentSplits.Count);
            potentialNewSplits = SplitRoom(CurrentSplits[index]);
            if(AreSplitsValid(potentialNewSplits))
            {
                CurrentSplits.RemoveAt(index);
                CurrentSplits.AddRange(potentialNewSplits);
            }
            count++;
        }


        RoomTemplate roomTemplate = null;
        for (int x = 0; x < CurrentSplits.Count; x++)
        {
            roomTemplate = building.GetRoomToGenerate(template);
            if (roomTemplate != null)
            { 
                curRoom = RoomGen.GenerateRoom(CurrentSplits[x].coords, CurrentSplits[x].size, roomTemplate, building.MyRooms.Count,building);
                building.AddRoom(curRoom);
            }
        }
        building.UpdateEdgeTiles();

        building.GenerateDoors();
        return building;
    }


    void FixBuildingInteriorWalls(GeneratedBuilding building)
    {
      
        List<Vector2Int> toClear = new List<Vector2Int>();
        for(int x = 1; x < building.Tiles.GetLength(0)-1; x++)
        {
            for(int y=1;y<building.Tiles.GetLength(1)-1; y++)
            {
                if (building.Tiles[x, y] != null)
                {
                    bool HasLeft = false, HasUp = false,HasRight=false,HasDown=false;
                    int count = 0;
                    if (building.Tiles[x + 1, y] != null)
                    {
                        if (building.Tiles[x, y].HasWall 
                            && building.Tiles[x + 1, y].HasWall 
                            && building.Tiles[x + 1, y].RoomID != building.Tiles[x, y].RoomID)
                        {
                            HasLeft = true;
                            count++;
                        }
                    }
                    
                    if (building.Tiles[x , y + 1] != null)
                    {
                        if (building.Tiles[x, y].HasWall 
                            && building.Tiles[x , y + 1].HasWall 
                            && building.Tiles[x , y + 1].RoomID != building.Tiles[x, y].RoomID)
                        {
                            HasUp = true;
                            count++;
                        }
                    }


                    if (building.Tiles[x - 1, y] != null)
                    {
                        if (building.Tiles[x, y].HasWall
                            && building.Tiles[x - 1, y].HasWall
                           /* && building.Tiles[x - 1, y].RoomID != building.Tiles[x, y].RoomID*/)
                        {
                            HasRight = true;
                            count++;
                        }
                    }

                    if (building.Tiles[x, y - 1] != null)
                    {
                        if (building.Tiles[x, y].HasWall
                            && building.Tiles[x, y - 1].HasWall
                           /* && building.Tiles[x, y - 1].RoomID != building.Tiles[x, y].RoomID*/)
                        {
                            HasDown = true;
                            count++;
                        }
                    }
                    if (HasUp&&!HasDown||HasLeft&&!HasDown||HasLeft&&!HasRight||HasLeft&&HasUp)
                    {
                        toClear.Add(new Vector2Int(x, y));
                         //building.Tiles[x, y].ClearWall();
                       
                    }
                }
                }
            }
        for (int x = 0; x < toClear.Count; x++) {
            building.Tiles[toClear[x].x, toClear[x].y].ClearWall();
        }
    }
   

    List<SplitRoom> SplitRoom(SplitRoom room)
    {
        List<SplitRoom> split = new List<SplitRoom>();
        Vector2Int SplitPoint = room.coords;
        int r = Random.Range(0, 100);
        int size1 = 0, size2 = 0;
        if (r < 50)
        {
            float lerp = Random.Range(.25f, .75f);
            size1 = Mathf.FloorToInt(room.size.x /2);
           size2 = (room.size.x -  size1);
            SplitPoint.x = (SplitPoint.x+ size1);
            split.Add(new SplitRoom(room.coords, new Vector2Int(size1, room.size.y))) ;
            split.Add(new SplitRoom(SplitPoint, new Vector2Int(size2, room.size.y)));
        }
        else
        {
            float lerp = Random.Range(.25f, .75f);
            size1 =(room.size.y /2);
            size2 = (room.size.y - size1);
            SplitPoint.y = (SplitPoint.y + size1);
            split.Add(new SplitRoom(room.coords, new Vector2Int( room.size.x,  size1 )));
            split.Add(new SplitRoom(SplitPoint, new Vector2Int( room.size.x, size2)));
        }
        for (int x = 0; x < split.Count; x++)
        {
            Vector2Int end = split[x].coords + split[x].size;
            if (end.x > size.x || end.y > size.y)
            {
                Debug.LogError("Room out of range " + split[x].ToString()+" from " + room.ToString()+" at " + SplitPoint+" rand " + r);
            }
        }

        return split;
    }

    bool AreSplitsValid(List<SplitRoom> splits)
    {
        for(int x = 0; x < splits.Count; x++)
        {
            if (splits[x].size.x < minRoomSize.x || splits[x].size.y < minRoomSize.y)
            {
                return false;
            }
        }

        return true;
    }
}

public struct SplitRoom
{
    public Vector2Int coords, size;
    public SplitRoom(Vector2Int coords, Vector2Int size)
    {
        this.coords = coords;
            this.size = size;
    }

    public override string ToString()
    {
        return "Room: " + coords.ToString() + "=>" + size.ToString();
    }
}

public class SquareBuildingNoSplitFloorplan:BuildingFloorplan
{
    Vector2Int size;
    public override GeneratedBuilding Generate(RoomGenerator RoomGen, int width, int height, Vector2Int pos, BuildingTemplate template, int maxPasses)
    {
        Debug.Log("Building Gen: Generating building at " + pos + " width " + width + "," + height);

        size = new Vector2Int(width, height);
        GeneratedBuilding building = new GeneratedBuilding(width, height, pos, template.BuildingName);
        int count = 0;
        GeneratedRoom curRoom = null;
        Vector2Int modifier = Vector2Int.zero;
        int[,] points = new int[width, height];

        List<SplitRoom> potentialNewSplits = new List<SplitRoom>();

        BuildingRoomData roomTemplate = template.MainRoom;
        curRoom = RoomGen.GenerateRoom(Vector2Int.zero, size, roomTemplate.roomTemplate, building.MyRooms.Count, building);
        building.AddRoom(curRoom);
        RoomTemplate subRoomTemplate = null;
        for(int x = 0; x < template.PotentialRooms.Count; x++)
        {
            subRoomTemplate = building.GetRoomToGenerate(template);
            if (subRoomTemplate != null)
            {
                Vector2Int roomSize = new Vector2Int(Random.Range(subRoomTemplate.MinWidth, subRoomTemplate.MaxWidth), Random.Range(subRoomTemplate.MinHeight, subRoomTemplate.MaxHeight));
                
                Vector2Int start = GetStartingCoordsForRoom(roomSize.x, roomSize.y, building);

                if (start.x >= 0)
                {
                    
                    building.ResetAreaOfBuilding(start, roomSize);

                    curRoom = RoomGen.GenerateRoom(start, roomSize, subRoomTemplate, building.MyRooms.Count, building);
                    building.AddRoom(curRoom,true);
                }
            }
        }


        //for (int x = 0; x < CurrentSplits.Count; x++)
        //{
        //    roomTemplate = building.GetRoomToGenerate(template);
        //    if (roomTemplate != null)
        //    {
        //        curRoom = RoomGen.GenerateRoom(CurrentSplits[x].coords, CurrentSplits[x].size, roomTemplate, building.MyRooms.Count, building);
        //        building.AddRoom(curRoom);
        //    }
        //}
        building.UpdateEdgeTiles();

        building.GenerateDoors();
        return building;
    }
}

public class CorridorBasedFloorplan : BuildingFloorplan
{
    const string CorridorName = "Corridor";
    Vector2Int size;
    public override GeneratedBuilding Generate(RoomGenerator RoomGen, int width, int height, Vector2Int pos, BuildingTemplate template, int maxPasses)
    {
        Debug.Log("Building Gen: Generating building at " + pos + " width " + width + "," + height);

        size = new Vector2Int(width, height);
        GeneratedBuilding building = new GeneratedBuilding(width, height, pos, template.BuildingName);
        int count = 0;
        GeneratedRoom curRoom = null;
        Vector2Int modifier = Vector2Int.zero;
        int[,] points = new int[width, height];


        BuildingRoomData roomTemplate = template.MainRoom;
        Vector2Int MainRoomSize = new Vector2Int(Random.Range(roomTemplate.roomTemplate.MinWidth, roomTemplate.roomTemplate.MaxWidth), Random.Range(roomTemplate.roomTemplate.MinHeight, roomTemplate.roomTemplate.MaxHeight));
        
        Vector2Int MainRoomPosition = BuildingFloorplan.GetValidEdgeCoordinatesForRoom(MainRoomSize.x, MainRoomSize.y, building);
        bool OnTopOfBuilding = MainRoomPosition.y > 0;
        curRoom = RoomGen.GenerateRoom(MainRoomPosition, MainRoomSize, roomTemplate.roomTemplate, building.MyRooms.Count, building);
        building.AddRoom(curRoom);

        RoomTemplate corridor = BuildingDataManager.Instance.RoomTemplates[CorridorName];
        size = new Vector2Int((width-(MainRoomPosition.x + (MainRoomSize.x))), corridor.MaxHeight);
        Vector2Int startPos = new Vector2Int(MainRoomPosition.x+(MainRoomSize.x)-1, MainRoomPosition.y);
        curRoom = RoomGen.GenerateRoom(startPos, size, corridor, building.MyRooms.Count, building);
        building.AddRoom(curRoom, true);

        startPos.x = 1;
        startPos.y = MainRoomPosition.y;
        size.x = MainRoomPosition.x;
        size.y = corridor.MaxHeight;
        curRoom = RoomGen.GenerateRoom(startPos, size, corridor, building.MyRooms.Count, building);
        building.AddRoom(curRoom, true);

        Debug.Log("Building on top " + OnTopOfBuilding);

        if (OnTopOfBuilding)
        {
            startPos.x = MainRoomPosition.x;
            startPos.y = 1;
            size.x = corridor.MaxWidth;
            size.y = (height - MainRoomSize.y);
            curRoom = RoomGen.GenerateRoom(startPos, size, corridor, building.MyRooms.Count, building);
            building.AddRoom(curRoom, true);
        }
        else
        {
            startPos.x = MainRoomPosition.x;
            startPos.y = MainRoomPosition.y+MainRoomSize.y-1;
            size.x = corridor.MaxWidth;
            size.y = (height - MainRoomSize.y);
            curRoom = RoomGen.GenerateRoom(startPos, size, corridor, building.MyRooms.Count, building);
            building.AddRoom(curRoom, true);
        }



        /*  List<SplitRoom> potentialNewSplits = new List<SplitRoom>();

          BuildingRoomData roomTemplate = template.MainRoom;
          curRoom = RoomGen.GenerateRoom(Vector2Int.zero, size, roomTemplate.roomTemplate, building.MyRooms.Count, building);
          building.AddRoom(curRoom);
          RoomTemplate subRoomTemplate = null;
          for (int x = 0; x < template.PotentialRooms.Count; x++)
          {
              subRoomTemplate = building.GetRoomToGenerate(template);
              if (subRoomTemplate != null)
              {
                  Vector2Int roomSize = new Vector2Int(Random.Range(subRoomTemplate.MinWidth, subRoomTemplate.MaxWidth), Random.Range(subRoomTemplate.MinHeight, subRoomTemplate.MaxHeight));

                  Vector2Int start = GetStartingCoordsForRoom(roomSize.x, roomSize.y, building);

                  if (start.x >= 0)
                  {

                      building.ResetAreaOfBuilding(start, roomSize);

                      curRoom = RoomGen.GenerateRoom(start, roomSize, subRoomTemplate, building.MyRooms.Count, building);
                      building.AddRoom(curRoom, true);
                  }
              }
          }


        */
        building.UpdateEdgeTiles();

        building.GenerateDoors();
        return building;
    }
}

public enum BuildingFloorplanType
{
    Basic,
    Square,
    SquareNoSplit,
    Corridors
}