using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.Analytics;
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
            case BuildingFloorplanType.House:
                return new HouseFloorplan();
            default:
                return new BuildingFloorplan();
                break;
        }
    }


    public static List<Vector2Int> GetValidEdgeTilesForRoom(int width, int height, GeneratedBuilding building, GeneratedRoom roomToBuildOff)
    {
        List<Vector2Int> ValidTilesToStartOn = new List<Vector2Int>();
        int xStart = 0;// Mathf.Clamp( roomToBuildOff.Position.x-width,0,building.Width);
        int yStart = 0;// Mathf.Clamp(roomToBuildOff.Position.y-height,0,building.Height);
        //NEED TOA DD SOMETHING TO CHECK IF THE TILE THATS WIDTH/HEIGHT AWAY IS VALID THEN ADD THAT COORDINATE ASWELL
        for (int x = xStart; x < building.Width - width; x++)
        {
            for (int y = yStart; y < building.Height - height; y++)
            {
                if (IsBuildingTileRoomEdge(building, roomToBuildOff, x, y) ||
                    IsBuildingTileRoomEdge(building, roomToBuildOff, x + width-1, y) || IsBuildingTileRoomEdge(building, roomToBuildOff, x, y + height-1))
                {
                    ValidTilesToStartOn.Add(new Vector2Int(x, y));
                }
            }
        }
        return ValidTilesToStartOn;
    }

    public static Vector2Int GetEdgeOfRoomLargeEnough(int width,int height,GeneratedBuilding building,GeneratedRoom roomToBuildOff)
    {
        List<Vector2Int> ValidTilesToStartOn = GetValidEdgeTilesForRoom(width,height,building,roomToBuildOff);

        Vector2Int startPos = Vector2Int.zero;
        for(int i = 0; i < ValidTilesToStartOn.Count; i++)
        {
            startPos = ValidTilesToStartOn[i];
            bool valid = true;
            for(int x = startPos.x; x < startPos.x + width; x++)
            {
                for(int y = startPos.y; y < startPos.y + height; y++)
                {
                    if (!IsTileValidForAdjacentRoom(x, y, building, roomToBuildOff)){
                        valid = false;
                        break;
                    }
                }
                if (!valid)
                {
                    break;
                }
            }
            if (valid)
            {

                return ValidTilesToStartOn[i];
            }
        }

        

        return Vector2Int.one * -1;
    }

    static bool IsTileValidForAdjacentRoom(int x,int y,GeneratedBuilding generatingIn,GeneratedRoom baseRoom)
    {
        if (x < 0 || y < 0 || x >= generatingIn.Tiles.GetLength(0) || y >= generatingIn.Tiles.GetLength(1))
        {
            return false;
        }

        if (generatingIn.Tiles[x, y] == null)
        {
            return true;
        }
        else
        {
            if (generatingIn.Tiles[x, y].RoomID == baseRoom.RoomID)
            {
                if (!generatingIn.Tiles[x, y].HasWall)
                {
                    return false;
                }
            }else if(generatingIn.Tiles[x, y].RoomID != baseRoom.RoomID)
            {
                if (!generatingIn.Tiles[x, y].HasWall)
                {
                    return false;
                }
            }
        }

        return true;
    }

    static bool IsBuildingTileRoomEdge(GeneratedBuilding toCheck,GeneratedRoom room,int x,int y)
    {
        if(x<0||y<0||x>=toCheck.Tiles.GetLength(1)|| y >= toCheck.Tiles.GetLength(1))
        {
            return false;
        }
        if (toCheck.Tiles[x,y] == null)
        {
            return false;
        }
        if (toCheck.Tiles[x, y].RoomID!=room.RoomID) 
        {
            return false;
        }
        return true;
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
    static bool IsBelow(int x,int y,Vector2Int coords)
    {
        return x < coords.x || y < coords.y;
    }

   static bool ShouldRoomHaveInteriorWalls(GeneratedBuilding building,int roomID)
    {
        if (roomID < 0)
        {
            return false;
        }
        if (BuildingDataManager.Instance.RoomTemplates.ContainsKey(building.MyRooms[roomID].RoomType)) 
        {
            RoomTemplate tempalte = BuildingDataManager.Instance.RoomTemplates[building.MyRooms[roomID].RoomType];
            WallMode mode = tempalte.WallMode;
            Debug.Log("Wall Mode for " + tempalte + " is " + mode);
            if(mode== WallMode.None)
            {
                return false;
            }
            else
            {
                return true;
            }
       }
        return false;
    }
    public static void PopulateRoomInteriorWalls(GeneratedBuilding building)
    {
        BuildingTemplate template = BuildingDataManager.Instance.BuildingTemplates[building.buildingType];
        List<RoomTile> neighbours = new List<RoomTile>();
        List<Vector2Int> neighbourCoords = new List<Vector2Int>();
        if (building.Tiles == null)
        {
            return;
        }
        for (int x = 0; x < building.Width; x++)
        {
            for (int y = 0; y < building.Height; y++)
            {
                if (building.Tiles[x, y]!=null&& building.Tiles[x,y].RoomID>=0 && building.Tiles[x, y].IsCorridor==false && building.Tiles[x, y].HasWall==false)
                {

                    GetNeighbouringTiles(building, x, y, ref neighbours, ref neighbourCoords);
                   
                        for (int q = 0; q < neighbours.Count; q++)
                        {
                            if (neighbours[q] != null)
                            {
                                if (neighbours[q].HasFloor == false 
                                || neighbours[q].HasWall
                                || ShouldRoomHaveInteriorWalls(building, building.Tiles[x, y].RoomID)==false)
                                {
                                    continue;
                                }
                                {
                                    if (!IsBelow(x, y, neighbourCoords[q]))
                                    {
                                        if (IsBuildingCorridor(neighbours[q]) || neighbours[q].RoomID != building.Tiles[x, y].RoomID)
                                        {

                                            building.Tiles[x, y].SetWall(template.CorridorWall);

                                        }
                                    }
                                    else
                                    {
                                        if (IsBuildingCorridor(neighbours[q]) || neighbours[q].RoomID != building.Tiles[x, y].RoomID)
                                        {

                                            building.Tiles[neighbourCoords[q].x, neighbourCoords[q].y].SetWall(template.CorridorWall);

                                        }
                                    }
                                }
                            }
                        }
               
                }
                /* else if (IsBuildingCorridor(building.Tiles[x, y])){
                     GetNeighbouringTiles(building, x, y, ref neighbours, ref neighbourCoords);
                     if (neighbours.Count == 4)
                     {
                         for (int q = 0; q < neighbours.Count; q++)
                         {
                             if (neighbours[q] == null)
                             {
                                 building.Tiles[x, y].SetWall(template.ExternalWall);
                                 break;
                             }else if (neighbours[q].IsCorridor == false)
                             {
                                 building.Tiles[x, y].SetWall(template.CorridorWall);

                             }
                         }
                     }
                     else
                     {
                         building.Tiles[x, y].SetWall(template.ExternalWall);

                     }
                 }*/
            }
        }
    }

    /// <summary>
    /// This code runs like shite, fix it
    /// also
    /// Add external wall exception for windows
    /// Shift floor tiles down and left 1 if the tile is a room tile
    /// make building generation also performed in the update loop rather than the inital road load
    /// </summary>
    /// <param name="building"></param>
    public static void PopulateExteriorWallTiles(GeneratedBuilding building)
    {
        BuildingTemplate template = BuildingDataManager.Instance.BuildingTemplates[building.buildingType];
        List<RoomTile> neighbours = new List<RoomTile>();
        List<Vector2Int> neighbourCoords = new List<Vector2Int>();
        if (building.Tiles == null)
        {
            return;
        }
        for (int x = 0; x < building.Width; x++)
        {
            for (int y = 0; y < building.Height; y++)
            {
                if (building.Tiles[x, y]!=null&&  building.Tiles[x, y].HasFloor)
                {
                   
                    GetNeighbouringTiles(building, x, y, ref neighbours,ref neighbourCoords);
                    if (neighbours.Count >0)
                    {
                        if (neighbours.Count == 4)
                        {
                            for (int q = 0; q < neighbours.Count; q++)
                            {

                                if (neighbours[q] == null || neighbours[q].HasFloor == false)
                                {
                                    if (building.Tiles[neighbourCoords[q].x, neighbourCoords[q].y] == null)
                                    {
                                        building.Tiles[neighbourCoords[q].x, neighbourCoords[q].y] = new RoomTile();
                                    }
                                   // building.Tiles[neighbourCoords[q].x, neighbourCoords[q].y].SetWall(template.ExternalWall);
                                    //building.Tiles[x+1, y+1].SetWall(template.CorridorWall);
                                     if (IsBelow(x, y, neighbourCoords[q]))
                                     {
                                         if (neighbours[q]==null) {
                                             building.Tiles[neighbourCoords[q].x, neighbourCoords[q].y] = new RoomTile();
                                         }
                                         building.Tiles[neighbourCoords[q].x, neighbourCoords[q].y].SetWall(template.ExternalWall);
                                     }
                                     else
                                     {
                                         building.Tiles[x, y].SetWall(template.ExternalWall);
                                     }
                                    // ;

                                }
                            }
                        }
                        else
                        {
                            building.Tiles[x, y].SetWall(template.ExternalWall);
                            for (int q = 0; q < neighbours.Count; q++)
                            {

                                if (neighbours[q] == null || neighbours[q].HasFloor == false)
                                {
                                    if (building.Tiles[neighbourCoords[q].x, neighbourCoords[q].y] == null)
                                    {
                                        building.Tiles[neighbourCoords[q].x, neighbourCoords[q].y] = new RoomTile();
                                    }
                                    // building.Tiles[neighbourCoords[q].x, neighbourCoords[q].y].SetWall(template.ExternalWall);
                                    //building.Tiles[x+1, y+1].SetWall(template.CorridorWall);
                                    if (IsBelow(x, y, neighbourCoords[q]))
                                    {
                                        if (neighbours[q] == null)
                                        {
                                            building.Tiles[neighbourCoords[q].x, neighbourCoords[q].y] = new RoomTile();
                                        }
                                        building.Tiles[neighbourCoords[q].x, neighbourCoords[q].y].SetWall(template.ExternalWall);
                                    }
                                    else
                                    {
                                        building.Tiles[x, y].SetWall(template.ExternalWall);
                                    }
                                    // ;

                                }
                            }
                        }

                    }
                   
                }
               /* else if (IsBuildingCorridor(building.Tiles[x, y])){
                    GetNeighbouringTiles(building, x, y, ref neighbours, ref neighbourCoords);
                    if (neighbours.Count == 4)
                    {
                        for (int q = 0; q < neighbours.Count; q++)
                        {
                            if (neighbours[q] == null)
                            {
                                building.Tiles[x, y].SetWall(template.ExternalWall);
                                break;
                            }else if (neighbours[q].IsCorridor == false)
                            {
                                building.Tiles[x, y].SetWall(template.CorridorWall);

                            }
                        }
                    }
                    else
                    {
                        building.Tiles[x, y].SetWall(template.ExternalWall);

                    }
                }*/
            }
        }
    }

    static bool IsBuildingCorridor(RoomTile tile)
    {
        return tile!=null && tile.IsCorridor;
    }

    static bool HasTileGotWall(RoomTile tile)
    {
            return tile.HasWall;
    }

    static void GetNeighbouringTiles(GeneratedBuilding building, int x, int y, ref List<RoomTile> toPopulate,ref List<Vector2Int> positions)
    {
        toPopulate.Clear();
        positions.Clear();
        if (x > 0)
        {
            toPopulate.Add(building.Tiles[x - 1, y]);
            positions.Add(new Vector2Int(x - 1, y));
        }
        if (x < building.Width - 1)
        {
            toPopulate.Add(building.Tiles[x + 1, y]);
            positions.Add(new Vector2Int(x + 1, y));

        }

        if (y > 0)
        {
            toPopulate.Add(building.Tiles[x, y - 1]);
            positions.Add(new Vector2Int(x , y - 1));

        }

        if (y < building.Height - 1)
        {
            toPopulate.Add(building.Tiles[x, y + 1]);
            positions.Add(new Vector2Int(x, y + 1));

        }
    }

    static bool IsTileExterior(RoomTile tile)
    {
        return tile==null|| tile.RoomID == -1;
    }


    public virtual GeneratedBuilding Generate(RoomGenerator RoomGen,int width,int height,Vector2Int pos,BuildingTemplate template,int maxPasses)
    {
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

                    building.AddRoom(curRoom);
                }
                

            }
            count++;
        }
        building.UpdateCorridorEdgeTiles(template);
        BuildingFloorplan.PopulateExteriorWallTiles(building);
        BuildingFloorplan.PopulateRoomInteriorWalls(building);

        building.GenerateDoors();
        building.ShiftAllFloorTiles(template);
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
        SplitRoom toPopulate = CurrentSplits[0];
        for (int x = 0; x < CurrentSplits.Count; x++)
        {
            roomTemplate = building.GetRoomToGenerate(template);
            toPopulate = GetRoomClosestToSizeOfRoom(ref CurrentSplits, roomTemplate);
            if (roomTemplate != null && !toPopulate.InUse)
            { 
                curRoom = RoomGen.GenerateRoom(CurrentSplits[x].coords, CurrentSplits[x].size, roomTemplate, building.MyRooms.Count,building);
                building.AddRoom(curRoom);
                toPopulate.InUse = true;

                if (roomTemplate.AttachedRooms.Count > 0)
                {
                    
                    GeneratedRoom subRoom = RoomGen.GenerateAttachedRooms(building, curRoom, roomTemplate);
                    if (subRoom != null)
                    {
                        building.AddRoom(subRoom, true);

                    }
                }
                
            }
        }
        BuildingFloorplan.PopulateExteriorWallTiles(building);
        BuildingFloorplan.PopulateRoomInteriorWalls(building);

        building.UpdateEdgeTiles();

        building.GenerateDoors();
        building.PopulatePropsInRooms(RoomGen);
        building.ShiftAllFloorTiles(template);
        return building;
    }


    SplitRoom GetRoomClosestToSizeOfRoom(ref List<SplitRoom> splits,RoomTemplate template)
    {
        Vector2Int avgSize = new Vector2Int(Mathf.RoundToInt(Mathf.Lerp(template.MinWidth, template.MaxWidth, .5f)),
            Mathf.RoundToInt(Mathf.Lerp(template.MinHeight, template.MaxHeight, .5f)));
        float closestInSize = 9999999f;
        SplitRoom closestToSize = splits[0];
        for(int x = 0; x < splits.Count; x++)
        {
            if (!splits[x].InUse)
            {
                if (Vector2.Distance(avgSize,splits[x].size) < closestInSize)
                {
                    closestInSize = Vector2.Distance(avgSize, splits[x].size);
                    closestToSize = splits[x];
                }
            }
        }
        return closestToSize;
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



public class HouseFloorplan : BuildingFloorplan
{
    

    public HouseFloorplan()
    {
       
    }
    Vector2Int size;
    public override GeneratedBuilding Generate(RoomGenerator RoomGen, int width, int height, Vector2Int pos, BuildingTemplate template, int maxPasses)
    {

        //pick a random room
        //generate at 0,0
        //record bottom right and top left
        //Try and genrate an attached room,
        //if you cant generate an attached room due to shape or no options, generate a hallway that covers the attempted axis
        //stop when you've run out of space

        size = new Vector2Int(width, height);
        GeneratedBuilding building = new GeneratedBuilding(width, height, pos, template.BuildingName);
        //to generate hallwasys, go through each row and check each tile, store all visited tiles before hitting a room or the edge,
        //if you hit a room, make all previous tiles a corridor, if its already a room move on and if you hit the edge of the building
        //discard the last set of gathered tiles


        List<Vector2Int> startPositions = new List<Vector2Int>();
        startPositions.Add(Vector2Int.zero);
        bool done = false;
        GeneratedRoom curRoom = null;
        int count = 0;
        while (!done&&count<1111)
        {
            RoomTemplate roomTemplate = building.GetRoomToGenerate(template);// building.GetRandomRoomWeCouldStillGenerate(template);
            if (roomTemplate == null)
            {
                done = true;

            }
            else
            {
                Vector2Int roomSize = new Vector2Int();
                int startIndex = Random.Range(0, startPositions.Count);
                Vector2Int position = startPositions[startIndex];
                Vector2Int remainder = size - position;
                roomSize.x = Random.Range(roomTemplate.MinWidth, roomTemplate.MaxWidth);
                roomSize.y = Random.Range(roomTemplate.MinHeight, roomTemplate.MaxHeight);
                bool skip = false;
                if (roomSize.x > remainder.x)
                {
                    if (remainder.x >= roomTemplate.MinWidth)
                    {
                        roomSize.x = remainder.x;
                    }
                    else
                    {
                        skip = true;
                    }
                }

                if (roomSize.y > remainder.y)
                {
                    if (remainder.y >= roomTemplate.MinHeight)
                    {
                        roomSize.y = remainder.y;
                    }
                    else
                    {
                        skip = true;
                    }
                }
                if (!skip) {
                    for (int x = 0; x < building.MyRooms.Count; x++)
                    {
                        if (LineUtil.SquareIntersection(position, position + roomSize, building.MyRooms[x].Position, building.MyRooms[x].Position + building.MyRooms[x].size))
                        {
                            skip = true;
                            break;
                        }
                    }
                }
                if (!skip)
                {

                    curRoom = RoomGen.GenerateRoom(position, roomSize, roomTemplate, building.MyRooms.Count, building);
                    
                    building.AddRoom(curRoom);
                    startPositions.RemoveAt(startIndex);
                    startPositions.Add(new Vector2Int(position.x + roomSize.x + 1, position.y));
                    startPositions.Add(new Vector2Int(position.x, position.y + roomSize.y + 1));
                    List<Vector2Int> newPositions = new List<Vector2Int>();
                    for(int x = 0; x < startPositions.Count; x++)
                    {
                        if (LineUtil.SquareContainsPoint(position, position + roomSize, startPositions[x])==false)
                        {
                            newPositions.Add(startPositions[x]);
                        }
                    }
                    startPositions = newPositions;
                   


                }
                if (startPositions.Count == 0)
                {
                    done = true;
                }
            }
          
            count++;
        }

        
        building.PopulatePropsInRooms(RoomGen);
        building.ShiftAllFloorTiles(template);
        GenerateFillInCorridors(ref building, size, BuildingDataManager.Instance.RoomTemplates["Hallway"]);
        
        building.UpdateEdgeTiles();
        BuildingFloorplan.PopulateExteriorWallTiles(building);
        BuildingFloorplan.PopulateRoomInteriorWalls(building);
        building.GenerateDoors();
       

        return building;
    }

    bool DoesTileStopCorridorChecks(RoomTile tile,GeneratedRoom room)
    {
        if (tile != null)
        {
            if (tile.RoomID != room.RoomID && tile.HasFloor)
            {
                return true;
            }
        }
        return false;
    }

    bool DoesTileStopCorridorChecks(RoomTile tile)
    {
        if (tile != null)
        {
            if (tile.RoomID != -1 && tile.HasFloor)
            {
                return true;
            }
        }
        return false;
    }

    static List<Vector2Int> CorridorTiles = new List<Vector2Int>();
    List<Vector2Int> GetTilesFromVerticalFromRoom(ref GeneratedBuilding building,GeneratedRoom roomToComeFrom,List<Vector2Int> AlreadyUsed,out CorridorEdge edge)
    {
        CorridorTiles.Clear();
        RoomTemplate template = BuildingDataManager.Instance.RoomTemplates[roomToComeFrom.RoomType];

        Vector2Int startPoint = roomToComeFrom.Position;
        startPoint.y += roomToComeFrom.size.y;
        startPoint.y -= template.FloorYOffset;
        int size = roomToComeFrom.size.x;

        Vector2Int endPoint = startPoint;
        endPoint.x += size;

        
        bool hitEnd = false;
        List<Vector2Int> LastLine = new List<Vector2Int>();
        Vector2Int curPos = new Vector2Int();
        bool hitEdge = false;
        while (!hitEnd)
        {
            if (endPoint.y >= building.Height)
            {
                hitEnd = true;
                hitEdge = true;
            }
            else
            {
                curPos.y = endPoint.y;
                for (int x = startPoint.x; x < endPoint.x; x++)
                {
                    curPos.x = x;
                    if (!AlreadyUsed.Contains(curPos))
                    {
                        LastLine.Add(curPos);
                        if (DoesTileStopCorridorChecks(building.Tiles[x, endPoint.y], roomToComeFrom))
                        {
                            hitEnd = true;
                            break;
                        }
                    }
                }
            }

            if (!hitEnd)
            {
                endPoint.y++;
                CorridorTiles.AddRange(LastLine);
                LastLine.Clear();
            }

            if (hitEdge)
            {
                CorridorTiles.Clear();
            }
        }
        if (CorridorTiles.Count > 0)
        {
            edge = new CorridorEdge(new Vector2Int(endPoint.x, startPoint.y), new Vector2Int(endPoint.x, endPoint.y));
        }
        else
        {
            edge = new CorridorEdge(startPoint, startPoint);

        }
        return CorridorTiles;
    }

    List<Vector2Int> GetTilesFromHorizontalEdge(ref GeneratedBuilding building, CorridorEdge edge, List<Vector2Int> AlreadyUsed)
    {
        CorridorTiles.Clear();
        if (edge.StartPosition == edge.EndPosition)
        {
            return CorridorTiles;
        }
        Vector2Int startPoint = edge.StartPosition;
        int size = edge.Width();


        Vector2Int endPoint = startPoint;
        endPoint.x += size;


        bool hitEnd = false;
        List<Vector2Int> LastLine = new List<Vector2Int>();
        Vector2Int curPos = new Vector2Int();
        bool hitEdge = false;
        while (!hitEnd)
        {
            if (endPoint.y >= building.Height)
            {
                hitEnd = true;
                hitEdge = true;
            }else if (endPoint.y - startPoint.y >= 3)
            {
                hitEnd = true;
            }
            else
            {
                curPos.y = endPoint.y;
                for (int x = startPoint.x; x < endPoint.x; x++)
                {
                    curPos.x = x;
                    if (!AlreadyUsed.Contains(curPos))
                    {
                        LastLine.Add(curPos);
                        if (DoesTileStopCorridorChecks(building.Tiles[x, endPoint.y]))
                        {
                            hitEnd = true;
                            break;
                        }
                    }
                }
            }

            if (!hitEnd)
            {
                endPoint.y++;
                CorridorTiles.AddRange(LastLine);
                LastLine.Clear();
            }

            if (hitEdge)
            {
                int width = endPoint.x - startPoint.x;
                int toRemove = CorridorTiles.Count;
                if (toRemove > 0)
                {
                    while (toRemove > width * 2 && width > 0)
                    {
                        toRemove -= width;
                    }
                    CorridorTiles.RemoveRange(CorridorTiles.Count - toRemove, toRemove);
                }
            }
        }

        return CorridorTiles;
    }


    List<Vector2Int> GetTilesFromVerticalEdge(ref GeneratedBuilding building, CorridorEdge edge, List<Vector2Int> AlreadyUsed)
    {
        CorridorTiles.Clear();
        if (edge.StartPosition == edge.EndPosition)
        {
            return CorridorTiles;
        }
        Vector2Int startPoint = edge.StartPosition;
        int size = edge.Height();


        Vector2Int endPoint = startPoint;
        endPoint.y += size;


        bool hitEnd = false;
        List<Vector2Int> LastLine = new List<Vector2Int>();
        Vector2Int curPos = new Vector2Int();
        bool hitEdge = false;
        while (!hitEnd)
        {
            if (endPoint.x >= building.Width - 1)
            {
                hitEnd = true;
                hitEdge = true;
            }
            else if (endPoint.x - startPoint.x > 3)
            {
                hitEnd = true;
            }
            else
            {
                curPos.x = endPoint.x;
                for (int y = startPoint.y; y < endPoint.y; y++)
                {
                    curPos.y = y;
                    if (!AlreadyUsed.Contains(curPos))
                    {
                        LastLine.Add(curPos);
                        if (DoesTileStopCorridorChecks(building.Tiles[endPoint.x, y]))
                        {
                            hitEnd = true;
                            break;
                        }
                    }
                }
            }

            if (!hitEnd)
            {
                endPoint.x++;
                CorridorTiles.AddRange(LastLine);
                LastLine.Clear();
            }

            if (hitEdge)
            {
                int height = endPoint.y - startPoint.y;
                int toRemove = CorridorTiles.Count;
                if (toRemove > 0)
                {
                    while (toRemove > height * 2 && height > 0)
                    {
                        toRemove -= height;
                    }
                    CorridorTiles.RemoveRange(CorridorTiles.Count - toRemove, toRemove);
                }
            }
        }


        return CorridorTiles;
    }
    List<Vector2Int> GetTilesFromHorizontalFromRoom(ref GeneratedBuilding building, GeneratedRoom roomToComeFrom, List<Vector2Int> AlreadyUsed, out CorridorEdge edge)
    {
        CorridorTiles.Clear();
            RoomTemplate template = BuildingDataManager.Instance.RoomTemplates[roomToComeFrom.RoomType];

        Vector2Int startPoint = roomToComeFrom.Position;
        startPoint.x += roomToComeFrom.size.x;
        startPoint.x -= template.FloorXOffset;
        int size = roomToComeFrom.size.y;


        Vector2Int endPoint = startPoint;
        endPoint.y += size;


        bool hitEnd = false;
        List<Vector2Int> LastLine = new List<Vector2Int>();
        Vector2Int curPos = new Vector2Int();
        bool hitEdge = false;
        while (!hitEnd)
        {
            if (endPoint.x >= building.Width-1)
            {
                hitEnd = true;
                hitEdge = true;
            }
            else
            {
                curPos.x = endPoint.x;
                for (int y = startPoint.y; y < endPoint.y; y++)
                {
                    curPos.y = y;
                    if (!AlreadyUsed.Contains(curPos))
                    {
                        LastLine.Add(curPos);
                        if (DoesTileStopCorridorChecks(building.Tiles[endPoint.x, y], roomToComeFrom))
                        {
                            hitEnd = true;
                            break;
                        }
                    }
                }
            }

            if (!hitEnd)
            {
                endPoint.x++;
                CorridorTiles.AddRange(LastLine);
                LastLine.Clear();
            }

            if (hitEdge)
            {
                CorridorTiles.Clear();

            }
        }
        if (CorridorTiles.Count > 0)
        {
            edge = new CorridorEdge(new Vector2Int(startPoint.x, endPoint.y), new Vector2Int(endPoint.x, endPoint.y));
        }
        else
        {
            edge = new CorridorEdge(startPoint,startPoint);


        }

        return CorridorTiles;
    }

    void GenerateFillInCorridors2(ref GeneratedBuilding building,Vector2Int buildingSize,RoomTemplate template)
    {
        List<Vector2Int> TilesToMakeCorridors = new List<Vector2Int>();
        CorridorEdge edge = new CorridorEdge();

        for (int x=0; x < building.MyRooms.Count; x++)
        {
            TilesToMakeCorridors.AddRange( GetTilesFromVerticalFromRoom(ref building, building.MyRooms[x],TilesToMakeCorridors,out edge));
            TilesToMakeCorridors.AddRange(GetTilesFromVerticalEdge(ref building, edge, TilesToMakeCorridors));
            TilesToMakeCorridors.AddRange(GetTilesFromHorizontalFromRoom(ref building, building.MyRooms[x], TilesToMakeCorridors, out edge));
            TilesToMakeCorridors.AddRange(GetTilesFromHorizontalEdge(ref building, edge, TilesToMakeCorridors));

        }

        building.SetTilesToTemplate(TilesToMakeCorridors, template);

    }

    void GenerateFillInCorridors(ref GeneratedBuilding building,Vector2Int buildingSize,RoomTemplate template)
    {
        GenerateFillInCorridors2(ref building, buildingSize, template);
        return;
        //this isn't working due to 
        //GenerateBuildingFromSplit(Vector2Int chunkBatchToPutIn)
        //only using rooms, not the actual building tiles, could try making this into a room instead?

        List<Vector2Int> positionsForCorridors = new List<Vector2Int>();
        List<Vector2Int> CurrentlyChecking=new List<Vector2Int>();
        Vector2Int val = new Vector2Int();
        for(int y = 0; y < buildingSize.y; y++)
        {
            CurrentlyChecking.Clear();
            bool addedLast = false;
            for(int x = 1; x < buildingSize.x; x++) 
            {
                if (building.Tiles[x, y] == null|| building.Tiles[x, y].HasWall&&!building.Tiles[x, y].HasFloor)
                {
                    CurrentlyChecking.Add(new Vector2Int(x, y));
                    if (!addedLast)
                    {
                        CurrentlyChecking.Add(new Vector2Int(x - 1, y));
                        addedLast = true;
                    }
                }
                else
                {
                    positionsForCorridors.AddRange(CurrentlyChecking);
                    CurrentlyChecking.Clear();
                    addedLast = false;
                }
            }
        }

        for (int x = 0; x < buildingSize.x; x++)
        {
            bool addedLast = false;

            CurrentlyChecking.Clear();
            for (int y = 1; y < buildingSize.y; y++)
            {
                if (building.Tiles[x, y] == null || building.Tiles[x, y].HasWall && !building.Tiles[x, y].HasFloor)
                {
                    val.x = x;
                    val.y = y;
                    if (!positionsForCorridors.Contains(val))
                    {
                        CurrentlyChecking.Add(val);
                        if (!addedLast)
                        {
                            val.y = y - 1;
                            if (!positionsForCorridors.Contains(val))
                            {
                                CurrentlyChecking.Add(val);
                                addedLast = true;
                            }
                        }
                    }
                }
                else
                {
                    positionsForCorridors.AddRange(CurrentlyChecking);
                    CurrentlyChecking.Clear();
                    addedLast = false;
                }
            }
        }
        building.SetTilesToTemplate(positionsForCorridors, template);
    }

    void GetEdgeTiles(Vector2Int roomPos,Vector2Int roomSize,GeneratedBuilding toGetFrom,ref PositionSet positionSet)
    {
        positionSet.Clear();
        for(int x = roomPos.x; x < roomPos.x + roomSize.x; x++)
        {
            if (toGetFrom.CoordsValid(x, roomPos.y) && toGetFrom.Tiles[x, roomPos.y] != null)
            {
                positionSet.Down.Add(new Vector2Int(x, roomPos.y));
            }
            if (toGetFrom.CoordsValid(x, roomPos.y +roomSize.y) && toGetFrom.Tiles[x, roomPos.y + roomSize.y ] != null)
            {
                positionSet.Up.Add(new Vector2Int(x, roomPos.y + roomSize.y ));
            }
        }

        for(int y = roomPos.y; y < roomPos.y + roomSize.y; y++)
        {
            if (toGetFrom.CoordsValid(roomPos.x, y) && toGetFrom.Tiles[roomPos.x, y] != null)
            {
                positionSet.Left.Add(new Vector2Int(roomPos.x , y));
            }
            if (toGetFrom.CoordsValid(roomPos.x + roomSize.x , y) && toGetFrom.Tiles[roomPos.x + roomSize.x, y] != null)
            {
                positionSet.Right.Add(new Vector2Int(roomPos.x +roomSize.x, y));
            }
        }
    }

    SplitRoom GetRoomClosestToSizeOfRoom(ref List<SplitRoom> splits, RoomTemplate template)
    {
        Vector2Int avgSize = new Vector2Int(Mathf.RoundToInt(Mathf.Lerp(template.MinWidth, template.MaxWidth, .5f)),
            Mathf.RoundToInt(Mathf.Lerp(template.MinHeight, template.MaxHeight, .5f)));
        float closestInSize = 9999999f;
        SplitRoom closestToSize = splits[0];
        for (int x = 0; x < splits.Count; x++)
        {
            if (!splits[x].InUse)
            {
                if (Vector2.Distance(avgSize, splits[x].size) < closestInSize)
                {
                    closestInSize = Vector2.Distance(avgSize, splits[x].size);
                    closestToSize = splits[x];
                }
            }
        }
        return closestToSize;
    }

}




public struct SplitRoom
{
    public Vector2Int coords, size;
    public bool InUse;
    public SplitRoom(Vector2Int coords, Vector2Int size)
    {
        this.coords = coords;
            this.size = size;
        InUse = false;
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
                    building.AddRoom(curRoom, true);

                    if (subRoomTemplate.AttachedRooms.Count > 0)
                    {
                       GeneratedRoom subRoom=  RoomGen.GenerateAttachedRooms(building, curRoom, subRoomTemplate);
                        if (subRoom != null)
                        {
                            building.AddRoom(subRoom, true);
                        }
                    }
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
        BuildingFloorplan.PopulateExteriorWallTiles(building);
        BuildingFloorplan.PopulateRoomInteriorWalls(building);
        building.UpdateEdgeTiles();

        building.GenerateDoors();
        building.PopulatePropsInRooms(RoomGen);
        building.ShiftAllFloorTiles(template);
        return building;
    }
}
//Remove the rightmost tiles from consideration when placing objects in buildings
//add something to exterior wall placement to shift the wall into the tile with the floor if the direction between the two positions is positive
public class CorridorBasedFloorplan : BuildingFloorplan
{
    const int MaxAttempts = 2000;
    const string CorridorName = "Corridor";
    Vector2Int size;
    public override GeneratedBuilding Generate(RoomGenerator RoomGen, int width, int height, Vector2Int pos, BuildingTemplate template, int maxPasses)
    {

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
        List<GeneratedRoom> Corridors = new List<GeneratedRoom>();
        RoomTemplate corridor = BuildingDataManager.Instance.RoomTemplates[CorridorName];
        size = new Vector2Int((width-(MainRoomPosition.x + (MainRoomSize.x))), corridor.MaxHeight);
        Vector2Int startPos = new Vector2Int(MainRoomPosition.x+(MainRoomSize.x)-1, MainRoomPosition.y);
        curRoom = RoomGen.GenerateRoom(startPos, size, corridor, building.MyRooms.Count, building);
        if (curRoom != null)
        {
            building.AddRoom(curRoom, true);
            curRoom.SetAsCorridor();
            Corridors.Add(curRoom);
        }
        startPos.x = 1;
        startPos.y = MainRoomPosition.y;
        size.x = MainRoomPosition.x;
        size.y = corridor.MaxHeight;
        curRoom = RoomGen.GenerateRoom(startPos, size, corridor, building.MyRooms.Count, building);
        if (curRoom != null)
        {
            curRoom.SetAsCorridor();

            building.AddRoom(curRoom, true);
            Corridors.Add(curRoom);
        }

        if (OnTopOfBuilding)
        {
            startPos.x = MainRoomPosition.x;
            startPos.y = 1;
            size.x = corridor.MaxWidth;
            size.y = (height - MainRoomSize.y);
            curRoom = RoomGen.GenerateRoom(startPos, size, corridor, building.MyRooms.Count, building);
            if (curRoom != null)
            {
                building.AddRoom(curRoom, true);
                curRoom.SetAsCorridor();

                Corridors.Add(curRoom);
            }
        }
        else
        {
            startPos.x = MainRoomPosition.x;
            startPos.y = MainRoomPosition.y+MainRoomSize.y-1;
            size.x = corridor.MaxWidth;
            size.y = (height - MainRoomSize.y);
            curRoom = RoomGen.GenerateRoom(startPos, size, corridor, building.MyRooms.Count, building);
            if (curRoom != null)
            {
                building.AddRoom(curRoom, true);
                curRoom.SetAsCorridor();

                Corridors.Add(curRoom);
            }
        }
        RoomTemplate subRoomTemplate = null;
        int RoomsToAddOffCorridors = 0;
        int attempts = 0,successes=0;
        int corridorIndex = 0;
        Vector2Int SubRoomSize = Vector2Int.zero;
        Vector2Int startCoords = Vector2Int.zero;
        for (int i = 0; i < template.PotentialRooms.Count; i++)
        {
            attempts = 0;
            successes = 0;
            RoomsToAddOffCorridors = Random.Range(template.PotentialRooms[i].Min, template.PotentialRooms[i].Max);
            subRoomTemplate = template.PotentialRooms[i].roomTemplate;
            SubRoomSize.x = Random.Range(subRoomTemplate.MinWidth, subRoomTemplate.MaxWidth);
            SubRoomSize.y = Random.Range(subRoomTemplate.MinHeight, subRoomTemplate.MaxHeight);
            corridorIndex = 0;
            while (corridorIndex < Corridors.Count && successes < RoomsToAddOffCorridors&&attempts< MaxAttempts)
            {

                startCoords = BuildingFloorplan.GetEdgeOfRoomLargeEnough(SubRoomSize.x,SubRoomSize.y, building, Corridors[corridorIndex]);
                if (startCoords.x >= 0)
                {
                    curRoom = RoomGen.GenerateRoom(startCoords, SubRoomSize, subRoomTemplate, building.MyRooms.Count, building);
                    building.AddRoom(curRoom, true);

                    if (subRoomTemplate.AttachedRooms.Count > 0)
                    {
                        GeneratedRoom subRoom= RoomGen.GenerateAttachedRooms(building, curRoom, subRoomTemplate);
                        if (subRoom != null)
                        {
                            building.AddRoom(subRoom, true);
                        }
                    }
                    successes++;
                }
                else
                {
                    corridorIndex++;
                }
              
                attempts++;
            }
        }
        BuildingFloorplan.PopulateExteriorWallTiles(building);
        BuildingFloorplan.PopulateRoomInteriorWalls(building);

        building.UpdateEdgeTiles();

        building.GenerateDoors();
        building.PopulatePropsInRooms(RoomGen);
        building.ShiftAllFloorTiles(template);

        return building;
    }

 
}

public class PositionSet
{
    public List<Vector2Int> Left, Right, Up, Down;
    public PositionSet()
    {
        Left = new List<Vector2Int>();
        Right = new List<Vector2Int>();
        Up = new List<Vector2Int>();
        Down = new List<Vector2Int>();
    }

    public bool DoWeHaveAnything()
    {
        return Left.Count + Right.Count + Up.Count + Down.Count > 0;
    }
    public void Clear()
    {
        Left.Clear();
        Right.Clear();
        Up.Clear();
        Down.Clear();
    }


    public Vector2Int GetPosition(Vector2Int sizeOfArea)
    {
        int r = Random.Range(0, 100);
        if (r < 50)
        {
            if (Left.Count > 0)
            {
                Vector2Int retVal = Left[Random.Range(0,Left.Count)];
                retVal.x -= sizeOfArea.x;
                return retVal;
            }

            if (Down.Count > 0)
            {
                Vector2Int retVal = Down[Random.Range(0, Down.Count)];
                retVal.y -= sizeOfArea.y;
                return retVal;
            }
        }

        if (r < 75)
        {
            if (Up.Count > 0)
            {
                return Up[Random.Range(0, Up.Count)];
                
            }
        }
        if (Right.Count > 0)
        {
            return Right[Random.Range(0, Right.Count)];

        }
        return Vector2Int.zero;
    }
}
public enum BuildingFloorplanType
{
    Basic,
    Square,
    SquareNoSplit,
    Corridors,
    House
}

public struct CorridorEdge
{
    public Vector2Int StartPosition;
    public Vector2Int EndPosition;
    public CorridorEdge(Vector2Int s,Vector2Int e)
    {
        StartPosition = s;
        EndPosition = e;
    }

    public int Width()
    {
        return EndPosition.x- StartPosition.x;
    }
    public int Height()
    {
        return EndPosition.y - StartPosition.y;

    }
}