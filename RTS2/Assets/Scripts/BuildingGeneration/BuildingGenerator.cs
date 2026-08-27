using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
  static BuildingGenerator instance;
    public static BuildingGenerator Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<BuildingGenerator>();
                instance.Init();
            }
            return instance;
        }
    }

    public void Init()
    {
        RoomGen = new RoomGenerator();

    }

    public RoomGenerator RoomGen;
    public BuildingTemplate BuildingTemplate;
    const int MaxGenerationPasses = 50;
    public GeneratedRoom MyRoom;
    public bool IsGenerating = false;

    public BuildingTemplate testTemplate;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateTestBuilding();
        }
    }

    public void GenerateTestBuilding()
    {
        IsGenerating = true;
        int width = Random.Range(testTemplate.MinWidth, testTemplate.MaxWidth);
        int height = Random.Range(testTemplate.MinHeight, testTemplate.MaxHeight);
        Vector2Int camPos = new Vector2Int((int)CameraController.Instance.transform.position.x, 
            (int)CameraController.Instance.transform.position.y);
        BuildingFloorplan floorplan = BuildingFloorplan.GetFloorplanByType(testTemplate.FloorplanType);
        ApplyBuidlingToWorld(floorplan.Generate(RoomGen, width, height, camPos - new Vector2Int(width / 2, height / 2), testTemplate, MaxGenerationPasses));
        IsGenerating = false;
    }

    public void GenerateBuilding(BuildingZoneBuilding building)
    {
        try
        {
            BuildingFloorplan floorplan = new SquareBuildingFloorplan(10, new Vector2Int(5, 5));
            ApplyBuidlingToWorld(floorplan.Generate(RoomGen, building.Size.x, building.Size.y, building.Position, BuildingDataManager.Instance.BuildingTemplates[building.Template], MaxGenerationPasses));
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.ToSafeString());
        }
    }

    public void ApplyBuidlingToWorld(GeneratedBuilding b)
    {
        Vector2Int pos = b.Position;
        RoomTile cur = null;
        Vector2Int batchCoords, chunkCoords, localCoords;
        uint ID = 0;
        EnvironmentObjectInstance obj = null;
        float elevation = 0f;
        int count = 0;
        for (int x = 0; x < b.Width; x++)
        {
            for (int y = 0; y < b.Height; y++)
            {
                pos = b.Position;
                pos.x += x;
                pos.y += y;
                cur = b.Tiles[x, y];
                if (cur == null)
                {
                    continue;
                }
                WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(pos.x, pos.y, out batchCoords, out chunkCoords, out localCoords);
                try
                {
                    obj = WorldChunkManager.Instance.GetChunkBatch(batchCoords).Chunks[chunkCoords.x, chunkCoords.y].GetEnvObjectNearPoint(pos, 2f);
                }
                catch
                {
                    Debug.LogError("Error trying to get object in batch " + (WorldChunkManager.Instance.GetChunkBatch(batchCoords) == null)+" "+batchCoords+","+chunkCoords+","+localCoords+","+b.Position);
                    continue;
                }
              
                if (obj != null)
                {

                    obj.DestroyInstance();
                    obj = null;
                }
                ID = WorldRenderer.Instance.WorldTilesManager.GetTileID(cur.FloorTile);

                //if (cur.HasDoor)
                //{

                //    //WallHelpers.CreateWallBuildableStructure(pos.x, pos.y, WorldController.Instance.BuildingTilemap,
                //    //     WallTypeManager.Instance.GetWallTile("Concrete"), new Vector3(pos.x, pos.y, 0), new Vector3(.5f, .5f, 0f));
                //    WallHelpers.CreateDoorBuildableStructure(pos.x, pos.y, WorldController.Instance.BuildingTilemap, WallTypeManager.Instance.SelectedWallTile
                //        , new Vector3(pos.x, pos.y, 0), new Vector3(.5f, .5f, 0f));
                //}
                //else

                if (cur.HasWall)
                {

                    WallHelpers.CreateWallBuildableStructure(pos.x, pos.y, WorldController.Instance.BuildingTilemap,
                        WallTypeManager.Instance.GetWallTile(cur.WallTile), new Vector3(pos.x, pos.y, 0), new Vector3(.5f, .5f, 0f));


                }

                if (cur.HasFloor)
                {
                    if (x < b.Width - 1 && y < b.Height - 1)
                    {
                        WorldChunkManager.Instance.ChunkBatches[batchCoords].Chunks[chunkCoords.x, chunkCoords.y].UpdateTile(localCoords.x, localCoords.y, cur.FloorTile, ID);
                    }
                }
                elevation += WorldChunkManager.Instance.ChunkBatches[batchCoords].Chunks[chunkCoords.x, chunkCoords.y].ChunkTiles[localCoords.x, localCoords.y].Elevation.GetElevation();
                count++;

            }
        }

        for (int x = 0; x < b.Width; x++)
        {
            for (int y = 0; y < b.Height; y++)
            {
                pos = b.Position;
                pos.x += x;
                pos.y += y;
                cur = b.Tiles[x, y];
                if (cur == null)
                {
                    continue;
                }
                WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(pos.x, pos.y, out batchCoords, out chunkCoords, out localCoords);
                

                if (cur.HasDoor)
                {
                    WallHelpers.CreateDoorBuildableStructure(pos.x, pos.y, WorldController.Instance.BuildingTilemap, WallTypeManager.Instance.SelectedWallTile
                        , new Vector3(pos.x, pos.y, 0), new Vector3(.5f, .5f, 0f));
                }


            }
        }

        elevation /= count;
        List<Vector2Int> batches = new List<Vector2Int>();
        for (int x = 0; x < b.Width; x++)
        {
            for (int y = 0; y < b.Height; y++)
            {
                pos = b.Position;
                pos.x += x;
                pos.y += y;
                cur = b.Tiles[x, y];
                if (cur == null)
                {
                    continue;
                }
               // if (cur.HasFloor||cur.IsCorridor)
                {
                    WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(pos.x, pos.y, out batchCoords, out chunkCoords, out localCoords);
                    WorldChunkManager.Instance.ChunkBatches[batchCoords].Chunks[chunkCoords.x, chunkCoords.y]
                        .ChunkTiles[localCoords.x, localCoords.y].SetElevation(elevation) ;
                    if (!batches.Contains(batchCoords))
                    {
                        batches.Add(batchCoords);
                    }
                    WorldChunkManager.Instance.ChunkBatches[batchCoords].Chunks[chunkCoords.x, chunkCoords.y].NeedsUpdate = true;
                }
                }
            }
        //for(int x = 0; x < batches.Count; x++)
        //{
        //    WorldChunkManager.Instance.ChunkBatches[batches[x]].UpdateElevations();
        //    WorldChunkManager.Instance.ChunkBatches[batches[x]].RefreshElevationTiles();
        //}

        Vector2Int envObjPos = new Vector2Int();
        for (int x = 0; x < b.MyRooms.Count; x++)
        {
            if (b.MyRooms[x].EnvObjects != null)
            {
                foreach(KeyValuePair<RoomTile,GeneratedRoomProp> kvp in b.MyRooms[x].EnvObjects)
                {
                    envObjPos = b.Position + b.MyRooms[x].Position + kvp.Value.pos;
                    ConstructableObjectManager.Instance.CreateObject_Generator(envObjPos, new Vector3(envObjPos.x, envObjPos.y), kvp.Value.ID);

                }
              
            }
        }
    }
 
    
}
[System.Serializable]
public class GeneratedBuilding
{
    public List<GeneratedRoom> MyRooms;
    public int Width, Height;
    public Vector2Int Position;

    public RoomTile[,] Tiles;
    List<Vector2Int> Edges;
    public Dictionary<int, RoomLink> Links;
    bool hasAnything = false;
    string buildingType;
    public GeneratedBuilding(int width, int height, Vector2Int pos,string type)
    {
        Width = width;
        Height = height;
        Position =pos;
        MyRooms = new List<GeneratedRoom>();
        Tiles=new RoomTile[width,height];   
        buildingType=type;
    }

    public void ResetAreaOfBuilding(Vector2Int coords,Vector2Int size)
    {

        for(int x = 0; x < MyRooms.Count; x++)
        {
            MyRooms[x].ResetRoom(coords, size);
        }

        for(int x = coords.x; x < coords.x + size.x; x++)
        {
            for(int y = coords.y; y < coords.y + size.y; y++)
            {
                Tiles[x, y].ResetTile(x<(coords.x + size.x-1) &&y<(coords.y + size.y-1));
                
            }
        }
    }

    bool InRange(int x,int y)
    {
        return x>=0&&y>=0&&x<Width&&y<Height;
    }

    public Vector2Int GetTopCorner()
    {
        return Position+new Vector2Int(Width,Height);   
    }

    bool isDirectlyAdjacent(int v1,int v2)
    {
        if (v2 == (v1 + 1) || v2 == (v1 - 1)){
            return true;
        }
        return false;
    }

    public bool GetValidStartPosition(Vector2Int size,out Vector2Int start,out Vector2Int mod)
    {
        if (Edges != null)
        {
            int index = 0;
            for (int q = 0; q < Edges.Count; q++)
            {
                index = Random.Range(0, Edges.Count);
                bool valid = true;
                start = Edges[index];

                if (start.x + size.x < Width && start.y + size.y < Height)
                {
                    for (int x = start.x; x < start.x + size.x; x++)
                    {
                        for (int y = start.y; y < start.y + size.y; y++)
                        {
                            if (HasAnything(x, y))
                            {
                                valid = false;
                                break;
                            }
                        }
                    }
                    if (valid)
                    {
                        mod = new Vector2Int(0, 0);
                        start = Edges[index];
                        return true;
                    }
                }
                if(start.x-size.x>0 && start.y + size.y < Height)
                {
                    valid = true;
                    for (int x = start.x; x > start.x - size.x; x--)
                    {
                        for (int y = start.y; y < start.y + size.y; y++)
                        {
                            if (HasAnything(x, y))
                            {
                                valid = false;
                                break;
                            }
                        }
                    }
                    if (valid)
                    {
                        mod = new Vector2Int(-1, 0);

                        start = Edges[index];
                        return true;
                    }
                }

                if (start.x + size.x < Width && start.y - size.y > 0)
                {
                    valid = true;

                    for (int x = start.x; x < start.x + size.x; x++)
                    {
                        for (int y = start.y; y > start.y - size.y; y--)
                        {
                            if (HasAnything(x, y))
                            {
                                valid = false;
                                break;
                            }
                        }
                    }
                    if (valid)
                    {
                        mod = new Vector2Int(0, -1);

                        start = Edges[index];
                        return true;
                    }
                }

                if (start.x - size.x > 0 && start.y - size.y > 0)
                {
                    valid = true;
                    for (int x = start.x; x > start.x - size.x; x--)
                    {
                        for (int y = start.y; y > start.y - size.y; y--)
                        {
                            if (HasAnything(x, y))
                            {
                                valid = false;
                                break;
                            }
                        }
                    }
                    if (valid)
                    {
                        mod = new Vector2Int(-1, -1);

                        start = Edges[index];
                        return true;
                    }
                }
            }
        }
        mod = new Vector2Int(1, 1);

        start = Vector2Int.zero;// new Vector2Int(Random.Range(0, Width - size.x), Random.Range(0, Height - size.y));
        return Edges==null|| Edges.Count==0;
    }

    public void SetTileAsCorridor(Vector2Int coords,string floor)
    {
        if (!InRange(coords.x, coords.y))
        {
            return;
        }
        if (Tiles[coords.x, coords.y] == null)
        {
            Tiles[coords.x, coords.y] = new RoomTile();
        }
        Tiles[coords.x, coords.y].IsCorridor = true;
        Tiles[coords.x, coords.y].HasFloor = true;
        Tiles[coords.x, coords.y].FloorTile = floor;
    }

    public bool IsValid(Vector2Int start,Vector2Int size)
    {
        if (start.x + size.x < Width && start.y + size.y < Height)
        {
            for (int x = start.x; x < start.x + size.x; x++)
            {
                for (int y = start.y; y < start.y + size.y; y++)
                {
                    if (HasAnything(x, y))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        return false;
    }

    bool HasAnything(int x,int y)
    {

        if (y < 0 || x < 0 || x >= Width || y >= Height)
        {
            return false;
        }
        if (Tiles[x, y] == null)
        {
            return false;
        }
        return Tiles[x, y].HasFloor||Tiles[x,y].IsCorridor;
    }

    bool HasWall(int x,int y)
    {
        if (y < 0 || x < 0 || x >= Width || y >= Height)
        {
            return false;
        }
        if (Tiles[x,y] == null)
        {
            return false;
        }
        return Tiles[x, y].HasWall;
    }

    bool HasProp(int x, int y)
    {
        if (y < 0 || x < 0 || x >= Width || y >= Height)
        {
            return false;
        }
        if (Tiles[x, y] == null)
        {
            return false;
        }
        return Tiles[x, y].HasProp;
    }
    bool HasDoor(int x, int y)
    {
        if (y < 0 || x < 0 || x >= Width || y >= Height)
        {
            return false;
        }
        if (Tiles[x, y] == null)
        {
            return false;
        }
        return Tiles[x, y].HasDoor;
    }
    bool ValidForDoor(int x,int y)
    {
        bool above = HasWall(x, y + 1) && !HasDoor(x, y + 1) && !HasProp(x, y + 1);
        bool below = HasWall(x, y - 1) && !HasDoor(x, y - 1) && !HasProp(x, y - 1);
        bool left = HasWall(x - 1, y) && !HasDoor(x - 1, y) && !HasProp(x - 1, y);
        bool right = HasWall(x + 1, y) && !HasDoor(x + 1, y) && !HasProp(x + 1, y);
        if (above&&below && !left && !right)
        {
            return true;
        }else if(!above&&!below&&left&&right)
        {
            return true;
        }
        return false;
    }
    public void GenerateExteriorDoors()
    {
        for (int x = 0; x < Width ; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Tiles[x, y] == null || Tiles[x, y].HasWall == false || !ValidForDoor(x, y) || Tiles[x,y].IsValidForDoor==false
                    ||BuildingDataManager.Instance.RoomTemplates[MyRooms[ Tiles[x,y].RoomID].RoomType].CanHaveExternalDoor==false)
                {
                    continue;
                }
                else
                {
                    int id = Tiles[x, y].RoomID;
                    if (!Links.ContainsKey(id))
                    {
                        Links.Add(id, new RoomLink(id));
                    }
                    if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                    {
                        if ((BuildingDataManager.Instance.RoomTemplates[MyRooms[id].RoomType].CanHaveExternalDoor)
                                         && !Links[id].DoesLinkExist(-1))
                        {
                            Links[id].AddLink(-1, new Vector2Int(x, y));
                        }
                    }
                }
            }
        }
        int count = 0;
        int max = BuildingDataManager.Instance.BuildingTemplates[buildingType].MaxExternalDoors;
        foreach (KeyValuePair<int, RoomLink> kvp in Links)
        {
            if (kvp.Value.HasCorridorLink)
            {
                Tiles[kvp.Value.CorridorLink.x, kvp.Value.CorridorLink.y].HasDoor = true;
            }
            foreach (KeyValuePair<int, List<Vector2Int>> kvp2 in kvp.Value.Links)
            {
                Vector2Int val = kvp2.Value[Random.Range(0, kvp2.Value.Count)];
                Tiles[val.x, val.y].HasDoor = true;
                count++;
            }
            if (count >= max)
            {
                break;
            }
        }

    }

    public void GenerateDoors()
    {
        Links = new Dictionary<int, RoomLink>();
        for (int x = 1; x < Width-1; x++)
        {
            for (int y = 1; y < Height-1; y++)
            {
                if (Tiles[x, y] == null || Tiles[x,y].HasWall==false||!ValidForDoor(x,y)||!Tiles[x, y].IsValidForDoor)
                {
                   
                }
                else
                {
                    int id = Tiles[x, y].RoomID;
                    if (!Links.ContainsKey(id))
                    {
                        Links.Add(id, new RoomLink(id));
                    }
                    for (int x1 = x - 1; x1 <= x + 1; x1++)
                    {
                        for (int y1 = y - 1; y1 <= y + 1; y1++)
                        {
                            if (x1 == x && y1 == y || !isDirectlyAdjacent(x, x1) || !isDirectlyAdjacent(y, y1))
                            {
                            
                            }
                            else
                            {
                                if (InRange(x1, y1) )
                                {
                                    if (Tiles[x1, y1] != null)
                                    {
                                        if (Tiles[x1, y1].IsCorridor)
                                        {
                                            if (!Links[id].HasCorridorLink)
                                            {
                                                Links[id].CorridorLink = new Vector2Int(x, y);
                                                Links[id].HasCorridorLink = true;
                                            }
                                        }
                                        else if (Tiles[x1, y1].RoomID != Tiles[x, y].RoomID)
                                        {
                                            Links[id].AddLink(Tiles[x1, y1].RoomID, new Vector2Int(x, y));

                                           
                                        }
                                    }
                                    
                                  
                                    }
                            }
                        }
                    }

                }
            }
            GenerateExteriorDoors();
        }
        foreach (KeyValuePair<int,RoomLink> kvp in Links)
        {

            if (kvp.Value.HasCorridorLink)
            {
                Tiles[kvp.Value.CorridorLink.x, kvp.Value.CorridorLink.y].HasDoor = true;
            }
            foreach(KeyValuePair<int,List<Vector2Int>> kvp2 in kvp.Value.Links)
            {
                if (kvp2.Key == kvp.Key)
                {
                    continue;
                }
                int count = 0;
                while (count < kvp2.Value.Count)
                {
                    Vector2Int val = kvp2.Value[Random.Range(0, kvp2.Value.Count)];
                    if (!Tiles[val.x, val.y].HasDoor)
                    {
                        Tiles[val.x, val.y].HasDoor = true;
                        count = kvp2.Value.Count;
                    }
                    count++;
                }
              

                }
        }
    
    }

    public Vector2Int GetEdgeOrStart(Vector2Int roomSize)
    {
        if (hasAnything&&Edges.Count>0)
        {
            return Edges[Random.Range(0, Edges.Count)];
        }
        else
        {
            return new Vector2Int(Random.Range(0, Width - roomSize.x), Random.Range(0, Height - roomSize.y));
        }
    }

    public void UpdateEdgeTiles()
    {
        Edges = new List<Vector2Int>();
        Vector2Int neighbour = Vector2Int.zero;
        List<Vector2Int> tileEdges = new List<Vector2Int>();
        int nullNeighbours = 0, nonNullNeighbours = 0, total = 0;
        bool hasCorridorNeighbour = false;
        for (int x=0; x<Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Tiles[x, y] == null)
                {
                    continue;
                }
                else
                {
                    tileEdges.Clear();
                    nullNeighbours = 0;
                    nonNullNeighbours = 0;
                    total = 0;
                    hasCorridorNeighbour = false;
                    for (int x1 = x - 1; x1 <= x + 1; x1++)
                    {
                        for (int y1 = y - 1; y1 <= y + 1; y1++)
                        {
                            if (x1 == x && y1 == y)
                            {
                                continue;
                            }
                            else
                            {
                                if (InRange(x1, y1))
                                {
                                   
                                    total++;
                                    if (Tiles[x1, y1] == null)
                                    {
                                        nullNeighbours++;
                                        neighbour.x = x1;
                                        neighbour.y = y1;
                                        if (!Edges.Contains(neighbour))
                                        {
                                            tileEdges.Add(neighbour);
                                        }
                                    }
                                    else
                                    {
                                        if(Tiles[x1, y1].IsCorridor)
                                        {
                                            hasCorridorNeighbour = true;
                                        }
                                        nonNullNeighbours++;
                                    }
                                }
                            }
                        }
                    }

                    if (hasCorridorNeighbour)
                    {
                        if (nullNeighbours > 0 || total < 8)
                        {
                            Edges.AddRange(tileEdges);
                            Tiles[x, y].IsEdge = true;
                        }
                        else
                        {
                            Tiles[x, y].IsEdge = false;
                        }
                    }
                    else
                    {
                        Tiles[x, y].IsEdge = false;

                    }
                }
            }
        }
    }

    public void UpdateCorridorEdgeTiles(BuildingTemplate template)
    {
        Vector2Int neighbour = Vector2Int.zero;
        int nullNeighbours = 0, nonNullNeighbours = 0, total = 0;
        List<Vector2Int> tileEdges = new List<Vector2Int>();
        bool hasCorridorNeighbour = false;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                //if (Tiles[x, y] != null)
                //{
                //    continue;
                //}
              // else
                {
                    nullNeighbours = 0;
                    nonNullNeighbours = 0;
                    total = 0;
                    tileEdges.Clear();
                    hasCorridorNeighbour = false;
                    for (int x1 = x - 1; x1 <= x + 1; x1++)
                    {
                        for (int y1 = y - 1; y1 <= y + 1; y1++)
                        {
                            if (x1 == x && y1 == y)
                            {
                                continue;
                            }
                            else
                            {
                                if (InRange(x1, y1))
                                {

                                    total++;
                                    if (Tiles[x1, y1] == null)
                                    {
                                        nullNeighbours++;
                                        neighbour.x = x1;
                                        neighbour.y = y1;
                                        if (!Edges.Contains(neighbour))
                                        {
                                            tileEdges.Add(neighbour);
                                        }
                                    }
                                    else
                                    {
                                        if (Tiles[x1, y1].IsCorridor)
                                        {
                                            hasCorridorNeighbour = true;
                                        }
                                        nonNullNeighbours++;
                                    }
                                }
                            }
                        }
                    }

                    if (hasCorridorNeighbour && (Tiles[x, y] == null || Tiles[x,y].IsCorridor==false)||total<8&& (Tiles[x, y] != null&& Tiles[x, y].IsCorridor))
                    {
                        if (Tiles[x, y] == null)
                        {
                            Tiles[x, y] = new RoomTile();
                        }
                            Tiles[x, y].HasWall = true;
                        Tiles[x, y].HasFloor = true;
                        Tiles[x, y].FloorTile = template.CorridorFloor;
                        Tiles[x, y].WallTile = template.CorridorWall;
                        
                    }
                    
                }
            }
        }
    }

    public void ApplyRoom(GeneratedRoom room,bool CanOverwrite=false)
    {
        Vector2Int Origin =  room.Position;
       // Origin.x -= 1;
      //  Origin.y -= 1;
        Origin.x = Mathf.Clamp(Origin.x, 0, Tiles.GetLength(0) - 1);
        Origin.y = Mathf.Clamp(Origin.y,0,Tiles.GetLength(1) - 1);
        for(int x = 0; x < room.size.x; x++)
        {
            for(int y=0;y<room.size.y; y++)
            {
                try
                {
                    if (Tiles[x + Origin.x, y + Origin.y] == null||CanOverwrite)
                    {
                        Tiles[x + Origin.x, y + Origin.y] = room.RoomTiles[x, y];
                        hasAnything = true;
                    }
                }
                catch
                {
                    Debug.LogError("error applying " + x + "," + y + " origin " + Origin + 
                        " dims " + Tiles.GetLength(0) + "x" + Tiles.GetLength(1)+" room " +room.RoomTiles.GetLength(0)+"x"+room.RoomTiles.GetLength(1));
                }
            }
        }
    }

    public void AddRoom(GeneratedRoom room,bool CanOverwrite=false)
    {
        MyRooms.Add(room);
        ApplyRoom(room,CanOverwrite);
        UpdateEdgeTiles();
    }

    public int GetQuantityOfRoomType(string roomType)
    {
        int count = 0;
        for(int x = 0; x < MyRooms.Count; x++)
        {
            if (MyRooms[x].RoomType == roomType)
            {
                count++;
            }
        }
        return count;
    }


    public RoomTemplate GetRoomToGenerate(BuildingTemplate template)
    {
        int index = 0;
        int count = 0;
        while (count < 50)
        {
            index = Random.Range(0, template.PotentialRooms.Count);
            if (NeedsMoreOfRoomType(template.PotentialRooms[index].roomTemplate, template))
            {
                return template.PotentialRooms[index].roomTemplate;
            }
            count++;
        }
        for(int x = 0; x < template.PotentialRooms.Count; x++)
        {
            if (NeedsMoreOfRoomType(template.PotentialRooms[x].roomTemplate, template))
            {
                return template.PotentialRooms[x].roomTemplate;
            }
        }
        return GetRandomRoomWeCouldStillGenerate(template);
    }

    RoomTemplate GetRandomRoomWeCouldStillGenerate(BuildingTemplate template)
    {
        List<RoomTemplate> potentialRooms = new List<RoomTemplate>();
        for (int x = 0; x < template.PotentialRooms.Count; x++)
        {
            if (CouldHaveMoreOfRoom(template.PotentialRooms[x].roomTemplate, template))
            {
                potentialRooms.Add( template.PotentialRooms[x].roomTemplate);
            }
        }
        if (potentialRooms.Count > 0)
        {
            return potentialRooms[Random.Range(0, potentialRooms.Count)];
        }
        return null;
    }
    public bool NeedsMoreOfRoomType(RoomTemplate room,BuildingTemplate building)
    {
        int count = GetQuantityOfRoomType(room.RoomID);
        BuildingRoomData data = building.GetDataByID(room.RoomID);
        return count < data.Min && data.Min > 0;
    }

    public bool CouldHaveMoreOfRoom(RoomTemplate room, BuildingTemplate building)
    {
        int count = GetQuantityOfRoomType(room.RoomID);
        BuildingRoomData data = building.GetDataByID(room.RoomID);
        return count < data.Max || data.Max<0;
    }
    public bool HasFinishedBuildingGen(BuildingTemplate template)
    {
        if (MyRooms.Count >= template.MaxRooms && template.MaxRooms > 0)
        {
            return true;
        }

        if (MyRooms.Count < template.MinRooms && template.MinRooms > 0)
        {
            return false;
        }

        for (int x = 0; x < template.PotentialRooms.Count; x++)
        {
            int count = GetQuantityOfRoomType(template.PotentialRooms[x].roomTemplate.RoomID);
            if (count < template.PotentialRooms[x].Min)
            {
                return false;
            }
        }
        return true;   
    }
}

public class RoomLink
{
    public int MyID;
    public Dictionary<int, List<Vector2Int>> Links;
    public Vector2Int CorridorLink;
    public bool HasCorridorLink = false;
    public RoomLink(int id)
    {
        MyID = id;
        Links = new Dictionary<int, List<Vector2Int>>();
    }

    public bool DoesLinkExist(int id)
    {
        return Links.ContainsKey(id);
    }
    public void AddLink(int id,Vector2Int coords)
    {
        if (!Links.ContainsKey(id))
        {
            Links.Add(id,new List<Vector2Int>());
        }
        Links[id].Add(coords);
    }
}