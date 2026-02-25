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
        Vector2Int startPosition = camPos;
        while (count <MaxGenerationPasses && !building.HasFinishedBuildingGen(BuildingTemplate))
        {
            TestTemplate = building.GetRoomToGenerate(BuildingTemplate);
            if (TestTemplate != null) {
                width = Random.Range(TestTemplate.MinWidth, TestTemplate.MaxWidth);
                height = Random.Range(TestTemplate.MinHeight, TestTemplate.MaxHeight);

                curRoom = RoomGen.GenerateRoom(startPosition, new Vector2Int(width, height), TestTemplate);
                building.AddRoom(curRoom);
                startPosition = curRoom.GetEdgeCoord();
            }
            
            count++;
        }
        for(int x = 0; x < building.MyRooms.Count; x++)
        {
            ApplyRoomToWorld(building.MyRooms[x]);
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
    public GeneratedBuilding(int width, int height, Vector2Int pos)
    {
        Width = width;
        Height = height;
        Position =pos;
        MyRooms = new List<GeneratedRoom>();
    }

    public void AddRoom(GeneratedRoom room)
    {
        MyRooms.Add(room);
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