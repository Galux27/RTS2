using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingFloorplan 
{
    public virtual GeneratedBuilding Generate(RoomGenerator RoomGen,int width,int height,Vector2Int pos,BuildingTemplate template,int maxPasses)
    {
        
        GeneratedBuilding building = new GeneratedBuilding(width, height, pos );
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

                    curRoom = RoomGen.GenerateRoom(startPosition + new Vector2Int((width - 1) * modifier.x, (height - 1) * modifier.y), new Vector2Int(width, height), roomTemplate, building.MyRooms.Count);
                    building.AddRoom(curRoom);
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
        size = new Vector2Int(width, height);
        GeneratedBuilding building = new GeneratedBuilding(width, height,pos);
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
               
               

                    curRoom = RoomGen.GenerateRoom(CurrentSplits[x].coords, CurrentSplits[x].size, roomTemplate, building.MyRooms.Count);
                    building.AddRoom(curRoom);
                
                // startPosition = building.GetEdgeOrStart(new Vector2Int(width, height));

            }
        }
        building.GenerateDoors();
        return building;
    }

    List<SplitRoom> SplitRoom(SplitRoom room)
    {
        List<SplitRoom> split = new List<SplitRoom>();
        Vector2Int SplitPoint = room.coords;
        int r = Random.Range(0, 100);
        if (r < 50)
        {
            float lerp = Random.Range(.25f, .75f);
            SplitPoint.x = Mathf.RoundToInt(SplitPoint.x+ (room.size.x * lerp));
            split.Add(new SplitRoom(room.coords, new Vector2Int(Mathf.RoundToInt( room.size.x *lerp), room.size.y))) ;
            split.Add(new SplitRoom(SplitPoint, new Vector2Int(Mathf.RoundToInt(room.size.x *(1f-lerp)), room.size.y)));

        }
        else
        {
            float lerp = Random.Range(.25f, .75f);
            SplitPoint.y = Mathf.RoundToInt(SplitPoint.y + (room.size.y * lerp));
            split.Add(new SplitRoom(room.coords, new Vector2Int( room.size.x, Mathf.RoundToInt( room.size.y*lerp))));
            split.Add(new SplitRoom(SplitPoint, new Vector2Int( room.size.x, Mathf.RoundToInt(room.size.y *(1f-lerp)))));
        }

        for(int x = 0; x < split.Count; x++)
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

