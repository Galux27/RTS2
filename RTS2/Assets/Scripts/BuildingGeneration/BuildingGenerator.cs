using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

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
    public static bool BuildingDebug = false;
    public void GenerateTestBuilding()
    {
        IsGenerating = true;
        BuildingDebug = true;
        int width = Random.Range(testTemplate.MinWidth, testTemplate.MaxWidth);
        int height = Random.Range(testTemplate.MinHeight, testTemplate.MaxHeight);
        Vector2Int camPos = new Vector2Int((int)CameraController.Instance.transform.position.x, 
            (int)CameraController.Instance.transform.position.y);
        BuildingFloorplan floorplan = BuildingFloorplan.GetFloorplanByType(testTemplate.FloorplanType);
        GeneratedBuilding building = floorplan.Generate(RoomGen, width, height, camPos - new Vector2Int(width / 2, height / 2), testTemplate, MaxGenerationPasses);
        for(int x = 0; x < building.MyRooms.Count; x++)
        {
            Color c = Color.green;
            for (int q = 0; q < building.MyRooms.Count; q++)
            {
                if (q == x)
                {
                    continue;
                }
                if (LineUtil.SquareIntersection(building.MyRooms[q].Position, building.MyRooms[q].Position+ building.MyRooms[q].size, building.MyRooms[x].Position, building.MyRooms[x].Position + building.MyRooms[x].size))
                {
                    c = Color.blue;
                }
            }


                BuildingSplitUtil.DebugDraw(building.MyRooms[x].Position + building.Position, building.MyRooms[x].size,c);
        }
        Dictionary<Vector2Int,GeneratedBuilding> splits= BuildingSplitUtil.SplitBuildingIntoChunks(building);
        foreach (KeyValuePair<Vector2Int,GeneratedBuilding> kvp in splits)
        {
            ApplyBuidlingToWorld(kvp.Value);
           // RoomTileDebug.DebugDrawAllTiles(kvp.Value);

        }
        RoomTileDebug.DebugDrawAllTiles(building);
        // ApplyBuidlingToWorld(building);
        BuildingDebug = false;
        IsGenerating = false;
       // RoomTileDebug.DebugDrawAllTiles(building);
    }

    public void GenerateBuilding(BuildingZoneBuilding building)
    {
        try
        {
            BuildingFloorplan floorplan = new SquareBuildingFloorplan(10, new Vector2Int(5, 5));
            GeneratedBuilding buildingGenerated = floorplan.Generate(RoomGen, building.Size.x, building.Size.y, building.Position, BuildingDataManager.Instance.BuildingTemplates[building.Template], MaxGenerationPasses);

            Dictionary<Vector2Int, GeneratedBuilding> splits = buildingGenerated.SplitBuildingIntoChunks();

            foreach (KeyValuePair<Vector2Int, GeneratedBuilding> kvp in splits)
            {
                ApplyBuidlingToWorld(kvp.Value);
                RoomTileDebug.DebugDrawAllTiles(kvp.Value);

            }

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
                    Debug.LogError("Error trying to get object in batch " + (WorldChunkManager.Instance.GetChunkBatch(batchCoords) == null)+b.Position+","+b.Width+","+b.Height);
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

                if (cur.HasWall && cur.WallTile!=null)
                {

                    WallHelpers.CreateWallObject(pos.x, pos.y, WorldController.Instance.BuildingTilemap,
                        WallTypeManager.Instance.GetWallTile(cur.WallTile), new Vector3(pos.x, pos.y, 0), new Vector3(.5f, .5f, 0f));


                }

                if (cur.HasFloor && cur.FloorTile!=null)
                {
                    
                     WorldChunkManager.Instance.ChunkBatches[batchCoords].Chunks[chunkCoords.x, chunkCoords.y].UpdateTile(localCoords.x, localCoords.y, cur.FloorTile, ID);
                    
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
                

                if (cur.HasDoor && cur.DoorTile != null)
                {
                    WallHelpers.CreateDoorObject(pos.x, pos.y, WorldController.Instance.DoorTilemap, WallTypeManager.Instance.GetWallTileForDoor(cur.WallTile));
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
                   if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(batchCoords)) {
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
                    if (kvp.Value.IsFromSlice)
                    {
                        envObjPos = b.MyRooms[x].Position + kvp.Value.pos;

                    }
                    else
                    {
                        envObjPos = b.Position + b.MyRooms[x].Position + kvp.Value.pos;

                    }
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
    Vector2Int size;
    public Vector2Int Size
    {
        get
        {
            if (size.x != Width||size.y!=Height)
            {
                size = new Vector2Int(Width, Height);
            }
            return size;
        }
    }
    public RoomTile[,] Tiles;
    List<Vector2Int> Edges;
    public Dictionary<int, RoomLink> Links;
    bool hasAnything = false;
    public string buildingType;

    public string GetRoomIds()
    {
        string retVal = "";
        List<int> idsInRoom = new List<int>();
        for (int i = 0; i < MyRooms.Count; i++)
        {
            idsInRoom.Clear();
            retVal += MyRooms[i].RoomID + ":";

            for(int x = 0; x < MyRooms[i].RoomTiles.GetLength(0); x++)
            {
                for (int y = 0; y < MyRooms[i].RoomTiles.GetLength(1); y++)
                {
                    if (MyRooms[i].RoomTiles[x, y] != null)
                    {
                        if(!idsInRoom.Contains(MyRooms[i].RoomTiles[x, y].RoomID))
                        {
                            idsInRoom.Add(MyRooms[i].RoomTiles[x, y].RoomID);
                        }
                    }
                }
            }
            for(int x = 0; x < idsInRoom.Count; x++)
            {
                retVal += idsInRoom[x] + ",";
            }
            retVal += " - ";
        }

            return retVal;
    }


    public GeneratedBuilding(int width, int height, Vector2Int pos,string type)
    {
        Width = width;
        Height = height;
        Position =pos;
        MyRooms = new List<GeneratedRoom>();
        Tiles=new RoomTile[width,height];   
        buildingType=type;
    }

    public void ClearAreaForRoom(GeneratedRoom room)
    {
        if (!MyRooms.Contains(room))
        {
            return;
        }
        Vector2Int localCoords = room.Position;
        if (localCoords.x > this.Width || localCoords.y > this.Height)
        {
            localCoords -= Position;
        }
        int count = 0,envCount=0;
        List<RoomTile> tilesReset = new List<RoomTile>();
        for(int x = localCoords.x; x < localCoords.x + room.size.x; x++)
        {
            for (int y = localCoords.y; y < localCoords.y + room.size.y; y++)
            {
                if (Tiles[x, y] != null)
                {
                    //if(Tiles[x, y].HasProp || Tiles[x,y].RoomID!=-1)
                    {
                        Tiles[x, y].HasProp = false;//ClearProp();
                        Tiles[x, y].RoomID = -1 ;//ClearProp();

                        count++;
                        tilesReset.Add(Tiles[x, y]);
                    }
                }
            }
        }
        for(int x = 0; x < MyRooms.Count; x++)
        {
            if (MyRooms[x].EnvObjects != null)
            {
                for (int y = 0; y < tilesReset.Count; y++)
                {
                    if (MyRooms[x].EnvObjects.ContainsKey(tilesReset[y]))
                    {
                        MyRooms[x].EnvObjects.Remove(tilesReset[y]);
                        envCount++;
                    }
                }
            }
        }
       
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

    public void ShiftAllFloorTiles(BuildingTemplate template)
    {
        if (template.ShiftHorizontalFloorTiles)
        {
            string[,] newFloorTiles = new string[Width, Height];
            bool[,] swapped = new bool[Width, Height];
            for (int x = Width - 1; x >= 0; x--)
            {
                for (int y = Height - 1; y >= 0; y--)
                {
                    if (Tiles[x, y] != null)
                    {
                        newFloorTiles[x, y] = Tiles[x, y].FloorTile;
                    }
                }
            }

            for (int x = Width - 2; x >= 0; x--)
            {
                for (int y = Height - 1; y >= 0; y--)
                {


                    if (swapped[x, y] == false && TileValidForFloorShift(Tiles[x, y]))
                    {

                        if (TileValidForFloorShift(Tiles[x + 1, y], Tiles[x, y].FloorTile))
                        {
                            newFloorTiles[x, y] = Tiles[x + 1, y].FloorTile; //"GreenCarpet";// 
                            newFloorTiles[x + 1, y] = Tiles[x + 1, y].FloorTile; //"IndustrialRusty";//Tiles[x, y].FloorTile;
                        }

                    }
                }
            }



            for (int x = Width - 1; x >= 0; x--)
            {
                for (int y = Height - 1; y >= 0; y--)
                {
                    if (newFloorTiles[x, y] != string.Empty)
                    {
                        if (Tiles[x, y] == null)
                        {
                            Tiles[x, y] = new RoomTile();
                        }
                        Tiles[x, y].SetFloor(newFloorTiles[x, y]);
                    }
                }
            }
        }
        if (template.ShiftVerticalFloorTiles)
        {
            ShiftVerticalFloorTiles();
        }
    }

    void ShiftVerticalFloorTiles()
    {
        string[,] newFloorTiles = new string[Width, Height];
        bool[,] swapped = new bool[Width, Height];
        for (int x = Width - 1; x >= 0; x--)
        {
            for (int y = Height - 1; y >= 0; y--)
            {

                newFloorTiles[x, y] = Tiles[x, y].FloorTile;
            }
        }

        for (int x = Width - 1; x >= 0; x--)
        {
            for (int y = Height - 2; y >= 0; y--)
            {

                if (TileValidForFloorShift(Tiles[x, y]))
                {

                    if (TileValidForFloorShift(Tiles[x, y + 1], Tiles[x, y].FloorTile))
                    {
                        newFloorTiles[x, y] = Tiles[x, y + 1].FloorTile; // "GreenCarpet";//

                        newFloorTiles[x, y + 1] = Tiles[x, y + 1].FloorTile;// "IndustrialRusty";// 
                        swapped[x, y] = true;

                    }

                }
            }
        }
        for (int x = Width - 1; x >= 0; x--)
        {
            for (int y = Height - 1; y >= 0; y--)
            {
                if (newFloorTiles[x, y] != string.Empty)
                {
                    Tiles[x, y].SetFloor(newFloorTiles[x, y]);
                }
            }
        }
    }

    bool TileValidForFloorShift(RoomTile tile)
    {
        return tile != null && tile.HasFloor;
    }

    bool TileValidForFloorShift(RoomTile tile,string newTileID)
    {
        return tile != null && tile.HasFloor&&tile.FloorTile!=newTileID;
    }

    public bool CoordsValid(int x,int y)
    {
        return x>=0&&x<Width&& y>=0 && y<Height;
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

    public void SetTilesToTemplate(List<Vector2Int> tiles,RoomTemplate template)
    {
        GeneratedRoom r = new GeneratedRoom(new Vector2Int(Width, Height), Vector2Int.zero, template.RoomID, MyRooms.Count);
        int count = 0;
        for(int x = 0; x < tiles.Count; x++)
        {
          
            if (this.Tiles[tiles[x].x, tiles[x].y] == null)
            {
                count++;
                this.Tiles[tiles[x].x, tiles[x].y] = new RoomTile();
            }
            this.Tiles[tiles[x].x, tiles[x].y].SetFloor(template.Floor);
            this.Tiles[tiles[x].x, tiles[x].y].IsCorridor = true;
            this.Tiles[tiles[x].x, tiles[x].y].RoomID = MyRooms.Count;
            r.RoomTiles[tiles[x].x, tiles[x].y] = this.Tiles[tiles[x].x, tiles[x].y];
        }
        AddRoom(r);
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


    public bool HasAnythingToBlockProps(int x,int y)
    {
        if (HasWall(x, y)|| HasDoor(x, y)|| HasProp(x, y))
        {
            return true;
        }

        return false;
    }
    public string HasAnythingToBlockPropsDebugInfo(int x, int y)
    {
        return HasWall(x, y).ToString()+ " "+ HasDoor(x, y).ToString()+" "+ HasProp(x, y).ToString();
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
    
    bool CanRoomTemplateHaveExternalDoor(int id,int x,int y)
    {
        if (id < MyRooms.Count)
        {
            return BuildingDataManager.Instance.RoomTemplates[MyRooms[Tiles[x, y].RoomID].RoomType].CanHaveExternalDoor;
        }
        return false;
    }
    
    
    public void GenerateExteriorDoors()
    {
        Debug.Log("Exterior doors: " + Width + "," + Height + " " + Tiles.GetLength(0) + "," + Tiles.GetLength(1));
        for (int x = 0; x < Width ; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Tiles[x, y] == null || Tiles[x, y].HasWall == false || !ValidForDoor(x, y) || Tiles[x,y].IsValidForDoor==false
                    || CanRoomTemplateHaveExternalDoor( Tiles[x,y].RoomID,x,y)==false)
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

    bool FarEnoughFromExisting(Vector2Int pos,List<Vector2Int> windows,float minDist)
    {
        for(int x = 0; x < windows.Count; x++)
        {
            if (Vector2Int.Distance(pos, windows[x]) <= minDist)
            {
                return false;
            }
        }
        return true;
    }

    public void GenerateWindows(BuildingWallMap map)
    {
        RoomTemplate template = null;
        List<Vector2Int> WindowsPlaced = new List<Vector2Int>();
        foreach(KeyValuePair<int,List<Vector2Int>> walls in map.Walls)
        {
            template = BuildingDataManager.Instance.RoomTemplates[MyRooms[walls.Key].RoomType];

            if (template.CanHaveWindows)
            {
                for(int x = 0; x < walls.Value.Count; x++)
                {
                    if (FarEnoughFromExisting(walls.Value[x], WindowsPlaced, 2f))
                    {
                        Tiles[walls.Value[x].x, walls.Value[x].y].SetWall("Window");
                        WindowsPlaced.Add(new Vector2Int(walls.Value[x].x, walls.Value[x].y));
                    }
                }
            }
        }
    }

    bool WallNextTo(Vector2Int tile,Vector2Int dir)
    {
        return this.Tiles[tile.x + dir.x, tile.y + dir.x]!=null&& this.Tiles[tile.x + dir.x, tile.y + dir.x].HasWall;
    }

    bool HorizontalWalls(Vector2Int tile)
    {
        return this.Tiles[tile.x + 1, tile.y]!=null&& this.Tiles[tile.x - 1, tile.y]!=null&&this.Tiles[tile.x + 1, tile.y].HasWall && this.Tiles[tile.x - 1, tile.y].HasWall;
    }


    bool VerticalWalls(Vector2Int tile)
    {
        return this.Tiles[tile.x, tile.y + 1]!=null && this.Tiles[tile.x, tile.y - 1] !=null&& this.Tiles[tile.x, tile.y + 1].HasWall && this.Tiles[tile.x, tile.y - 1].HasWall;
    }

    public bool IsValidForDoor(Vector2Int tile)
    {
        if(this.Tiles[tile.x, tile.y] == null)
        {
            return false;
        }
        if (this.Tiles[tile.x, tile.y].HasWall)
        {
            if (tile.x == 0)
            {
                if (tile.y == Height - 1)
                {
                    return false;
                }
                else if (tile.y == 0)
                {
                    return false;
                }
                else
                {
                    return VerticalWalls(tile) && !WallNextTo(tile, Vector2Int.right);
                }
            }
            else if (tile.x == Width - 1)
            {
                if (tile.y == Height - 1)
                {
                    return false;
                }
                else if (tile.y == 0)
                {
                    return false;
                }
                else
                {
                    return VerticalWalls(tile) &&!WallNextTo(tile,Vector2Int.left);

                }
            }
            else
            {
                if (tile.y == Height - 1)
                {
                    return HorizontalWalls(tile) && !WallNextTo(tile, Vector2Int.down);

                }
                else if (tile.y == 0)
                {
                    return HorizontalWalls(tile) && !WallNextTo(tile, Vector2Int.up);

                }
                else
                {
                    return HorizontalWalls(tile)&&!VerticalWalls(tile)||
                        !HorizontalWalls(tile) && VerticalWalls(tile);

                }
            }
         
        }
        
        return false;
        
    }

    bool CanTemplatesConnect(RoomTemplate temp1,RoomTemplate temp2)
    {
        if(temp1.GenerateDoorsToOtherRooms || temp2.GenerateDoorsToOtherRooms)
        {
            return true;
        }
        return false;
    }

    bool DoesConnectionExist(int room1,int room2)
    {
        if (room1 == room2)
        {
            return true;
        }
        if(!Links.ContainsKey(room1) && !Links.ContainsKey(room2))
        {
            return false;
        }
        else if (Links.ContainsKey(room1))
        {
           if(Links[room1].DoesLinkExist(room2))
            {
                return true;
            }
        }else if (Links.ContainsKey(room2))
        {
            if (Links[room2].DoesLinkExist(room1))
            {
                return true;
            }
        }
        return false;
    }

    void AddLink(int room1,int room2,Vector2Int position)
    {
        if (!Links.ContainsKey(room1))
        {
            Links.Add(room1, new RoomLink(room1));
        }
        Links[room1].AddLink(room2,position);

        GeneratedRoom room1obj = MyRooms[room1];
        GeneratedRoom room2obj = MyRooms[room2];
        RoomTemplate template1 = BuildingDataManager.Instance.RoomTemplates[room1obj.RoomType];
        RoomTemplate template2 = BuildingDataManager.Instance.RoomTemplates[room2obj.RoomType];

        Debug.Log("Setting Door Tile " + position + " between " + room1obj.Position +"("+room1obj.size+")"+ " and " + room2obj.Position + "(" + room2obj.size + ")"+this.Size );
        if (room1obj.size != this.Size)
        {
            room1obj.SetTileHasDoor(position);
        }
        if (room2obj.size != this.Size)
        {
            room2obj.SetTileHasDoor(position);

        }

    }

    void GenerateDoorsFromLinks(int key,RoomTemplate template)
    {
        Vector2Int door = Vector2Int.zero;
        if (Links.ContainsKey(key))
        {
            foreach(KeyValuePair<int,List<Vector2Int>> kvp in Links[key].Links)
            {
                door = kvp.Value[Random.Range(0, kvp.Value.Count)];
                Debug.Log("Door: generating door at " + door);
                 Tiles[door.x, door.y].SetDoor(template.Door);

            }
        }
    }

    void GenerateAllLinks()
    {
        foreach (KeyValuePair<int, RoomLink> kvp in Links)
        {
            GenerateDoorsFromLinks(kvp.Key, BuildingDataManager.Instance.RoomTemplates[MyRooms[kvp.Key].RoomType]);
        }
    }

    public void GenerateInteriorDoors(BuildingWallMap map)
    {
        Links = new Dictionary<int, RoomLink>();
        Vector2Int curPos = new Vector2Int(), NeighbourPos = new Vector2Int();
        RoomTemplate template = null, otherTemplate = null;
        RoomTile checking = null;
        Debug.Log("Total interior walls " + map.Walls.Count);
        foreach (KeyValuePair<int, List<Vector2Int>> walls in map.Walls)
        {
            if (walls.Key == -1)
            {
                continue;
            }
            try
            {
                template = BuildingDataManager.Instance.RoomTemplates[MyRooms[walls.Key].RoomType];
            for (int x = 0; x < walls.Value.Count; x++)
            {
                curPos.x = walls.Value[x].x;
                curPos.y = walls.Value[x].y;
                
                for (int x1 = curPos.x - 1; x1 <= curPos.x + 1; x1++)
                {
                    for (int y1 = curPos.y - 1; y1 <= curPos.y + 1; y1++)
                    {
                        NeighbourPos.x = x1;
                        NeighbourPos.y = y1;
                        if (InRange(x1, y1)&&NeighbourPos!=curPos)
                        {
                            checking = Tiles[NeighbourPos.x, NeighbourPos.y];
                            if ( checking.RoomID != walls.Key && checking.RoomID>=0)
                            {
                                otherTemplate = BuildingDataManager.Instance.RoomTemplates[MyRooms[checking.RoomID].RoomType];
                                if (!DoesConnectionExist(walls.Key, checking.RoomID) 
                                    && IsValidForDoor(curPos) 
                                    &&  NeighbourPos.y == curPos.y)
                                {
                                    AddLink(walls.Key, checking.RoomID, curPos);
                                  //  AddLink(checking.RoomID, walls.Key,  NeighbourPos);

                                }
                            }
                        }
                    }

                }
                
            }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error placing exterior doors in " + walls.Key + " " + walls.Value.Count + " total rooms " + MyRooms.Count+" other room "+ checking.RoomID);
                Debug.LogError(e.ToSafeString());
            }
        }

        GenerateAllLinks();
    }

    public void GenerateExteriorDoors(BuildingWallMap map)
    {
        RoomTemplate template = null;
        List<Vector2Int> DoorsPlaced = new List<Vector2Int>();
        Vector2Int curPos = new Vector2Int() ;
        List<Vector2Int> potentialDoors = new List<Vector2Int>();
        List<int> doorRooms = new List<int>();
        foreach (KeyValuePair<int, List<Vector2Int>> walls in map.Walls)
        {
            if (walls.Key == -1)
            {
                continue;
            }
            try
            {
                template = BuildingDataManager.Instance.RoomTemplates[MyRooms[walls.Key].RoomType];

                if (template.CanHaveExternalDoor)
                {
                    for (int x = 0; x < walls.Value.Count; x++)
                    {
                        curPos.x = walls.Value[x].x;
                        curPos.y = walls.Value[x].y;



                        if (IsValidForDoor(curPos))
                        {
                            potentialDoors.Add(curPos);
                            doorRooms.Add(walls.Key);
                        }
                    }
                }
            }catch(System.Exception e)
            {
                Debug.LogError("Error placing exterior doors in " + walls.Key + " " + walls.Value.Count + " total rooms " + MyRooms.Count);
                Debug.LogError(e.ToSafeString());
            }
        }
        if (potentialDoors.Count > 0)
        {
            int index = Random.Range(0, potentialDoors.Count);
            Vector2Int exteriorDoor = potentialDoors[index];
            Tiles[exteriorDoor.x, exteriorDoor.y].SetDoor(template.Door);
            //Debug.Log("Setting Door Tile ext" + exteriorDoor.ToSafeString() + " in " + this.MyRooms[index].Position +" size "+ this.MyRooms[index].size+ " building pos " + this.Position+","+this.Width+","+this.Height);
        }
    }

    

    public void GenerateDoors(bool GenerateExternal = false)
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
                    RoomTemplate template = BuildingDataManager.Instance.RoomTemplates[MyRooms[id].RoomType];

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
                                            if (!Links[id].HasCorridorLink && template.GenerateDoorsToCorridors)
                                            {
                                                Links[id].CorridorLink = new Vector2Int(x, y);
                                                Links[id].HasCorridorLink = true;
                                            }
                                        }
                                        else if (Tiles[x1, y1].RoomID != Tiles[x, y].RoomID&&template.GenerateDoorsToOtherRooms)
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
            if (GenerateExternal)
            {
                GenerateExteriorDoors();
            }
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


    public Vector2Int ConvertRoomCoordsToBuildingCoords(Vector2Int coords,GeneratedRoom room)
    {
        Vector2Int Origin = room.Position + coords; ;
        Origin.x = Mathf.Clamp(Origin.x, 0, Tiles.GetLength(0) - 1);
        Origin.y = Mathf.Clamp(Origin.y, 0, Tiles.GetLength(1) - 1);
        return Origin;
    }


    public void ApplySlicedRoom(GeneratedRoom room,GeneratedBuilding slicedFrom)
    {
        //code isn't working because the the conversion is done from the original building and the application is done in the sliced building
        //need to write something to convert between the building passed into here and the building its applied to
        
        
        Vector2Int Origin = Position;
        Vector2Int Difference = room.Position-Origin;
        //work out difference between the two rooms starting positions, then subtract from the origin used?
       // Origin.x = Mathf.Clamp(Origin.x, 0, Tiles.GetLength(0) - 1);
       // Origin.y = Mathf.Clamp(Origin.y, 0, Tiles.GetLength(1) - 1);
        //Debug.LogError("error applying room original building pos "+ Origin +" difference between slice from and new building  "+Difference+
        //               " building size " + Tiles.GetLength(0) + "x" + Tiles.GetLength(1) + " room size " + room.RoomTiles.GetLength(0) + "x" + room.RoomTiles.GetLength(1) + " error " 
        //               + " room pos " + room.Position +" this pos " + Position);
        int xi = 0, yi = 0;
        for (int x = 0; x < room.size.x; x++)
        {
            for (int y = 0; y < room.size.y; y++)
            {
                xi = x + Difference.x;
                yi= y + Difference.y;
                if (xi >= 0 && yi >= 0)
                {
                    try
                    {
                       // if (Tiles[xi, yi] == null)
                        {
                            Tiles[xi, yi] = room.RoomTiles[x, y];
                            hasAnything = true;
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("error applying " + x+"("+xi+")" + "," + y + "(" + yi + ")" + " origin " + Origin + " difference " + Difference +
                            " dims " + Tiles.GetLength(0) + "x" + Tiles.GetLength(1) + " room " + room.RoomTiles.GetLength(0) + "x" + room.RoomTiles.GetLength(1) + " error " + e.ToString()
                            + " room pos " + room.Position);
                    }
                }
            }
        }
    }

    public void ApplyRoomProps(GeneratedRoom room)
    {
        Vector2Int Origin = room.Position;
        // Origin.x -= 1;
        //  Origin.y -= 1;
        Origin.x = Mathf.Clamp(Origin.x, 0, Tiles.GetLength(0) - 1);
        Origin.y = Mathf.Clamp(Origin.y, 0, Tiles.GetLength(1) - 1);
        for (int x = 0; x < room.size.x; x++)
        {
            for (int y = 0; y < room.size.y; y++)
            {
                try
                {
                    if (Tiles[x + Origin.x, y + Origin.y].HasProp)
                    {
                        Tiles[x + Origin.x, y + Origin.y].SetProp();// = room.RoomTiles[x, y];
                        //Tiles[x + Origin.x, y + Origin.y].RoomID = room.RoomID;
                        hasAnything = true;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("error applying " + x + "," + y + " origin " + Origin +
                        " dims " + Tiles.GetLength(0) + "x" + Tiles.GetLength(1) + " room " + room.RoomTiles.GetLength(0) + "x" + room.RoomTiles.GetLength(1) + " error " + e.ToString());
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
                        Tiles[x + Origin.x, y + Origin.y].RoomID= room.RoomID;
                        hasAnything = true;
                    }
                }
                catch(System.Exception e)
                {
                    Debug.LogError("error applying " + x + "," + y + " origin " + Origin + 
                        " dims " + Tiles.GetLength(0) + "x" + Tiles.GetLength(1)+" room " +room.RoomTiles.GetLength(0)+"x"+room.RoomTiles.GetLength(1)+" error " + e.ToString());
                }
            }
        }
    }

    public List<GeneratedRoomProp> GetPropsFromSlicedRoom(List<RoomTile> tilesUsed)
    {
        List<GeneratedRoomProp> props = new List<GeneratedRoomProp>();
        GeneratedRoomProp prop = null;
        for (int x = 0; x < tilesUsed.Count; x++)
        {
            if (tilesUsed[x] == null)
            {
                continue;
            }
            for(int i = 0; i < MyRooms.Count; i++)
            {
                if (MyRooms[i].EnvObjects == null )
                {
                    continue;
                }
                if (MyRooms[i].EnvObjects.ContainsKey(tilesUsed[x]))
                {
                    prop = MyRooms[i].EnvObjects[tilesUsed[x]];

                    props.Add(new GeneratedRoomProp(prop.ID, prop.pos + MyRooms[i].Position+Position,true));
                    
                }
            }
           
        }
        return props;
    }


    public Dictionary<int, Dictionary<Vector2Int, string>> PopulatePropsInRooms(RoomGenerator roomGen)
    {
        Dictionary<int, Dictionary<Vector2Int, string>> retVal = new Dictionary<int, Dictionary<Vector2Int, string>>();
        for (int x = 0; x < MyRooms.Count; x++)
        {
            
            Debug.Log("Building Split: generating room in building " + this.Position + " room " + MyRooms[x].Position + " type " + MyRooms[x].RoomType);
            retVal.Add(x, roomGen.PopulateRoomEnvObjects(MyRooms[x], this));

          
            Debug.Log("Building Split: generating room in building " + this.Position + " room " + MyRooms[x].Position + " type " + MyRooms[x].RoomType + " props made " + retVal[x].Count);
           
            // ApplyRoomProps(MyRooms[x]);
        }
        return retVal;
    }
    /// <summary>
    /// For when your're adding a room to a building thats beign sliced to fit the chunk system
    /// </summary>
    /// <param name="room"></param>
    /// <param name="originalBuilding"></param>
    /// <param name="CanOverwrite"></param>
    public void AddRoom(GeneratedRoom room,GeneratedBuilding originalBuilding, bool CanOverwrite = false)
    {
        if (room == null)
        {
            return;
        }
        MyRooms.Add(room);
        UpdateRoomCount(room.RoomType);
        ApplySlicedRoom(room,originalBuilding);
        UpdateEdgeTiles();
    }

    public void AddRoom(GeneratedRoom room,bool CanOverwrite=false)
    {
        if (room == null)
        {
            return;
        }
        MyRooms.Add(room);
        UpdateRoomCount(room.RoomType);
        ApplyRoom(room,CanOverwrite);
        UpdateEdgeTiles();
    }

    void UpdateRoomCount(string type)
    {
        if (!RoomTypeCounts.ContainsKey(type))
        {
            RoomTypeCounts.Add(type, 0);
        }
        RoomTypeCounts[type]++;
    }

    Dictionary<string, int> RoomTypeCounts = new Dictionary<string, int>();


    public int GetQuantityOfRoomType(string roomType)
    {
        if (RoomTypeCounts.ContainsKey(roomType))
        {
            return RoomTypeCounts[roomType];
        }
        return 0;
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

   public RoomTemplate GetRandomRoomWeCouldStillGenerate(BuildingTemplate template)
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

    /// <summary>
    /// Split building into more buidlings that match up with the chunk sizes
    /// </summary>
    /// <returns></returns>
    public Dictionary<Vector2Int,GeneratedBuilding> SplitBuildingIntoChunks()
    {
        Dictionary<Vector2Int, GeneratedBuilding> Buildings = new Dictionary<Vector2Int, GeneratedBuilding>();
        //work out lowest batch coord needed and highest
        Vector2Int RoundedPositionLow = new Vector2Int(RoundToMultiple(Position.x, WorldChunkManager.ChunkBatchSize), RoundToMultiple(Position.y, WorldChunkManager.ChunkBatchSize));
        Vector2Int RoundedPositionHigh = new Vector2Int(RoundToMultiple(Position.x+Width, WorldChunkManager.ChunkBatchSize), RoundToMultiple(Position.y+Height, WorldChunkManager.ChunkBatchSize));
        //check if rounding needs to be altered
        if (RoundedPositionLow.x > Position.x)
        {
            RoundedPositionLow.x -= WorldChunkManager.ChunkBatchSize;
        }

        if (RoundedPositionLow.y > Position.y)
        {
            RoundedPositionLow.y -= WorldChunkManager.ChunkBatchSize;
        }

        if (RoundedPositionHigh.x > Position.x + Width)
        {
            RoundedPositionHigh.x -= WorldChunkManager.ChunkBatchSize;
        }
        if (RoundedPositionHigh.x > Position.x + Width)
        {
            RoundedPositionHigh.x -= WorldChunkManager.ChunkBatchSize;
        }

        Vector2Int batchCoords = RoundedPositionLow;
        GeneratedBuilding buidlingGenerated = null;
        //go through each chunk batch encompassing the building
        for(int x = RoundedPositionLow.x; x <= RoundedPositionHigh.x; x += WorldChunkManager.ChunkBatchSize)
        {
            for (int y = RoundedPositionLow.y; y <= RoundedPositionHigh.y; y += WorldChunkManager.ChunkBatchSize)
            {
                batchCoords.x = x;
                batchCoords.y = y;
                buidlingGenerated = GenerateBuildingFromSplit(batchCoords);
                if (buidlingGenerated != null)
                {
                    Buildings.Add(batchCoords, buidlingGenerated);
                }
            }
        }

        return Buildings;   
    }

    GeneratedBuilding GenerateBuildingFromSplit(Vector2Int chunkBatchToPutIn)
    {
        Vector2Int ChunkEndPosition = chunkBatchToPutIn + new Vector2Int(WorldChunkManager.ChunkBatchSize+1, WorldChunkManager.ChunkBatchSize+1);
        //think this code is wrong try adding the square intersection from the room slicing to work out what size the building should be
        //local coordinates to start the split from
        int xStart = 0, yStart = 0 ;

      
        Vector2Int buildingMaxPosition = Position + new Vector2Int(Width, Height);
        //local coordinates to end the split on
        Vector2Int buildingMinPosition = Position - Vector2Int.one;

        int x5 = Mathf.Max(chunkBatchToPutIn.x, Position.x);
        int y5 = Mathf.Max(chunkBatchToPutIn.y, Position.y);
        int x6 = Mathf.Min(ChunkEndPosition.x, buildingMaxPosition.x+Width);
        int y6 = Mathf.Min(ChunkEndPosition.y, buildingMaxPosition.y+Height);
        if (x5 >= x6 || y5 >= y6)
        {
            return null;
        }

        //create new building for chunk batch
        //copy over any relevant data

        int newBuildingWidth = x6 - x5;
        int newBuildingHeight = y6 - y5;
        Vector2Int min = new Vector2Int(x5, y5);
        Vector2Int max = new Vector2Int(x6, y6);
        GeneratedBuilding building = TakeSliceOfBuilding(this, chunkBatchToPutIn, new Vector2Int(newBuildingWidth, newBuildingHeight)
            , new Vector2Int(x5, y5));//new GeneratedBuilding(newBuildingWidth, newBuildingHeight, new Vector2Int(x5, y5), this.buildingType);

        GeneratedRoom room = null;
        for(int x = 0; x < MyRooms.Count; x++)
        {
            //if (MyRooms[x].IsPartOfRoomInArea(min, max, this))
            {
                room = MyRooms[x].TakeSliceOfRoom(min, max, this);
                if (room != null)
                {
                    building.AddRoom(room,this);
                    room = null;
                }
            }
        }

        //check through each room and create a new room based off it with any tiles that should be in this chunk batch  
        return building;
    }

    bool InRange(Vector2Int min,Vector2Int max,Vector2Int val)
    {
        return val.x >= min.x && val.x < max.x && val.y >= min.y && val.y < max.y;
    }

    public GeneratedBuilding TakeSliceOfBuilding( GeneratedBuilding building,Vector2Int batchCoords,Vector2Int size,Vector2Int position)
    {
        GeneratedBuilding retVal = new GeneratedBuilding(size.x, size.y, position, building.buildingType);

        Vector2Int sampleStart = position;
        Vector2Int sampleEnd = position + size;
        Vector2Int tileWorldPos = building.Position;
        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                tileWorldPos = building.Position;
                tileWorldPos.x += x;
                tileWorldPos.y+= y;
                if (InRange(sampleStart, sampleEnd, tileWorldPos))
                {
                    tileWorldPos = tileWorldPos - sampleStart;
                    retVal.Tiles[tileWorldPos.x, tileWorldPos.y] = new RoomTile();
                    retVal.Tiles[tileWorldPos.x, tileWorldPos.y].CopyData(building.Tiles[x, y]);
                }

            }
        }

        return retVal;
        //need to add props & add something to refresh adjacent walls as some are jank
       /* Vector2Int size = new Vector2Int(Width, Height);

        Vector2Int RoomPosition = Position;
        Vector2Int roomStart = Vector2Int.zero;
        Vector2Int roomMax = building.Position + Position + size;
        //gets intersection of squares
        int x5 = Mathf.Max(min.x, roomStart.x);
        int y5 = Mathf.Max(min.y, roomStart.y);
        int x6 = Mathf.Min(max.x, roomMax.x);
        int y6 = Mathf.Min(max.y, roomMax.y);
        if (x5 > x6 || y5 > y6)
        {
            return null;
        }
        int width = x6 - x5;
        int height = y6 - y5;

        int localXStart = Mathf.RoundToInt(Mathf.InverseLerp(roomStart.x, roomMax.x, x5) * size.x);
        int localYStart = Mathf.RoundToInt(Mathf.InverseLerp(roomStart.y, roomMax.y, y5) * size.y);
        // GeneratedRoom room = new GeneratedRoom(new Vector2Int(width, height), new Vector2Int(x5, y5), RoomType, RoomID);
        GeneratedBuilding retVal = new GeneratedBuilding();
        //Debug.Log("Room Slice: room area " + roomStart + " to " + roomMax + " area checking " + min + " to " + max+" intersection "+ x5+","+y5+" to "+ x6+","+y6+" local "+localXStart+","+localYStart+" size " + width+","+height+" room size "+ size);

        //Debug.Log("Taking slice of room " + x5 + "," + y5 + " max " + x6 + "," + y6 + ",local" + localXStart + 
        //    "," + localYStart+" building pos " +building.Position+",size b "+building.Width+","+building.Height+", room size"+this.size
        //    +" area start " + min+" area end " +max+",slice size "+ width+","+height+" room pos "+ room.Position);

        //code isn't working because the the conversion is done from the original building and the application is done in the sliced building
        //need to write something to convert between the building passed into here and the building its applied to

        Vector2Int position = Vector2Int.zero;
        int newX = localXStart, newY = localYStart;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tiles[x, y].CopyData(retVal.Tiles[newX, newY]);

                //if (EnvObjects != null && EnvObjects.ContainsKey(RoomTiles[newX, newY]))
                //{
                //    //subtracting room position to get around stupid corner I've painted myself into with how they're placed in the world
                //    room.AddEnvObject(room.RoomTiles[x, y], new GeneratedRoomProp(EnvObjects[RoomTiles[newX, newY]].ID, new Vector2Int(x, y), true));
                //}
                newY++;
            }
            newX++;
            newY = localYStart;
        }





        return room;*/
    }

    public int RoundToMultiple(int value, int roundTo)
    {
        return (value / roundTo) * roundTo;
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