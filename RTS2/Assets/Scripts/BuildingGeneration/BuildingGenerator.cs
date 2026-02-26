using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
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
            }
            return instance;
        }
    }

    public RoomGenerator RoomGen;
    public RoomTemplate TestTemplate;
    public BuildingTemplate BuildingTemplate;
    const int MaxGenerationPasses = 50;
    public void GenerateBuilding()
    {
        int width = Random.Range(BuildingTemplate.MinWidth, BuildingTemplate.MaxWidth);
        int height = Random.Range(BuildingTemplate.MinHeight, BuildingTemplate.MaxHeight);
        Vector2Int camPos = new Vector2Int((int)CameraController.Instance.transform.position.x, 
            (int)CameraController.Instance.transform.position.y);
        
        GeneratedBuilding building = new GeneratedBuilding(width, height, camPos);
        int count = 0;
        RoomGen = new RoomGenerator();
        GeneratedRoom curRoom = null;
        Vector2Int startPosition = building.GetEdgeOrStart(new Vector2Int(building.Width,building.Height)) ;
        while (count <MaxGenerationPasses && !building.HasFinishedBuildingGen(BuildingTemplate))
        {
            TestTemplate = building.GetRoomToGenerate(BuildingTemplate);
            if (TestTemplate != null) {
                width = Random.Range(TestTemplate.MinWidth, TestTemplate.MaxWidth);
                height = Random.Range(TestTemplate.MinHeight, TestTemplate.MaxHeight);

                curRoom = RoomGen.GenerateRoom(startPosition, new Vector2Int(width, height), TestTemplate);
                building.AddRoom(curRoom);
                startPosition = building.GetEdgeOrStart(new Vector2Int(building.Width, building.Height));
            }
            
            count++;
        }
       ApplyBuidlingToWorld(building);

    }

    void ApplyBuidlingToWorld(GeneratedBuilding b)
    {
        Vector2Int pos = b.Position;
        RoomTile cur = null;
        Vector2Int batchCoords, chunkCoords, localCoords;
        uint ID = 0;

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
                ID = WorldRenderer.Instance.WorldTilesManager.GetTileID(cur.FloorTile);

                if (cur.HasWall)
                {
                    if (!cur.IsEdge)
                    {
                        WallHelpers.CreateWallBuildableStructure(pos.x, pos.y, WorldController.Instance.BuildingTilemap,
                            WallTypeManager.Instance.GetWallTile(cur.WallTile), new Vector3(pos.x, pos.y, 0), new Vector3(.5f, .5f, 0f));
                    }
                    else
                    {
                        WallHelpers.CreateWallBuildableStructure(pos.x, pos.y, WorldController.Instance.BuildingTilemap,
                            WallTypeManager.Instance.GetWallTile("Concrete"), new Vector3(pos.x, pos.y, 0), new Vector3(.5f, .5f, 0f));
                    }
                }

                if (cur.HasFloor&&!cur.HasWall)
                {
                    WorldChunkManager.Instance.ChunkBatches[batchCoords].Chunks[chunkCoords.x, chunkCoords.y].UpdateTile(localCoords.x, localCoords.y, cur.FloorTile, ID);
                }

            }
        }
    }

    void ApplyRoomToWorld(GeneratedRoom r)
    {
        Vector2Int pos = r.Position;
        RoomTile cur = null;
        Vector2Int batchCoords, chunkCoords, localCoords;
        uint ID = 0;

        for (int x = 0; x <  r.size.x; x++)
        {
            for (int y = 0; y < r.size.y; y++)
            {
                pos = r.Position;
                pos.x += x;
                pos.y += y;
                cur = r.RoomTiles[x,y];
                WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(pos.x, pos.y, out batchCoords, out chunkCoords, out localCoords);
                ID = WorldRenderer.Instance.WorldTilesManager.GetTileID(cur.FloorTile);

                if (cur.HasWall)
                {
                    WallHelpers.CreateWallBuildableStructure(pos.x, pos.y, WorldController.Instance.BuildingTilemap, 
                        WallTypeManager.Instance.GetWallTile(cur.WallTile),new Vector3(pos.x,pos.y,0), new Vector3(.5f, .5f, 0f));

                }

                if (cur.HasFloor)
                {
                    WorldChunkManager.Instance.ChunkBatches[batchCoords].Chunks[chunkCoords.x, chunkCoords.y].UpdateTile(localCoords.x, localCoords.y, cur.FloorTile, ID);
                }

            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateBuilding();
        }
    }
}

public class GeneratedBuilding
{
    public List<GeneratedRoom> MyRooms;
    public int Width, Height;
    public Vector2Int Position;
    public RoomTile[,] Tiles;
    List<Vector2Int> Edges;
    bool hasAnything = false;
    public GeneratedBuilding(int width, int height, Vector2Int pos)
    {
        Width = width;
        Height = height;
        Position =pos;
        MyRooms = new List<GeneratedRoom>();
        Tiles=new RoomTile[width,height];   
    }
    bool InRange(int x,int y)
    {
        return x>=0&&y>=0&&x<Width&&y<Height;
    }

    public bool IsValid(Vector2Int start,Vector2Int size)
    {
        if (start.x + size.x < Width && start.y + size.y < Height)
        {
            return true;
            for(int x=start.x;x<start.x+size.x;x++)
            {
                for(int y=start.y;y<start.y+size.y;y++)
                {
                    
                }
            }
        }
        return false;
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
        for(int x=0; x<Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Tiles[x, y] == null)
                {
                    continue;
                }
                else
                {
                    int nullNeighbours = 0, nonNullNeighbours = 0,total=0;
                    for(int x1 = x - 1; x1 <= x + 1; x1++)
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
                                    }
                                    else
                                    {
                                        nonNullNeighbours++;
                                    }
                                }
                            }
                        }
                    }

                    if (nullNeighbours > 0||total<8)
                    {
                        Edges.Add(new Vector2Int(x, y));
                        Tiles[x, y].IsEdge = true;
                    }
                    else
                    {
                        Tiles[x, y].IsEdge = false;

                    }
                }
            }
        }
    }


    public void ApplyRoom(GeneratedRoom room)
    {
        Vector2Int Origin = room.Position;
        for(int x = 0; x < room.size.x; x++)
        {
            for(int y=0;y<room.size.y; y++)
            {
                if (Tiles[x + Origin.x, y + Origin.y] == null)
                {
                    Tiles[x + Origin.x, y + Origin.y] = room.RoomTiles[x, y];
                    hasAnything = true;
                }
            }
        }
    }

    public void AddRoom(GeneratedRoom room)
    {
        MyRooms.Add(room);
        ApplyRoom(room);
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