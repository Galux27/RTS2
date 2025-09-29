using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.Timeline;

public class WorldChunkManager : MonoBehaviour
{
    public const int ChunkSize = 16;
    public const int ChunksPerBatch = 16;
    public const int ChunkBatchSize = ChunkSize * ChunksPerBatch;
    static WorldChunkManager instance;
    public Dictionary<Vector2Int, WorldChunkBatch> ChunkBatches;
    public List<Vector2Int> ChunksLoaded;
    public static WorldChunkManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WorldChunkManager>();
                if (instance.IsInit==false)
                {
                    instance.Init();
                }
            }
            return instance;
        }
    }
    public bool IsInit = false;
    public void Init()
    {
       
        InitWorldChunks();
        IsInit = true;
    }
    int Width, Height;
    public void InitWorldChunks()
    {
        ChunkBatches = new Dictionary<Vector2Int, WorldChunkBatch>();
        CreateChunkBatch(new Vector2Int());       
    }
    
    Vector2Int ConvertWorldCoordsToOverworldCoords(Vector2Int coords)
    {
        Vector2Int retVal = OverworldGenerator.Instance.GetOverworldStartingCoords()+ new Vector2Int(coords.x/ChunkBatchSize,coords.y/ChunkBatchSize);


        return retVal;
    }


    void CreateChunkBatch(Vector2Int coords,bool init=true)
    {
        ChunkBatches.Add(coords, WorldChunkBatch.CreateWorldChunkBatch(coords,ConvertWorldCoordsToOverworldCoords(coords)));
        if (init)
        {
            if (ExistingChunkData != null)
            {
                ChunkBatches[coords].NeedsGeneration = !ExistingChunkData.ContainsKey(coords);
            }

            ChunkBatches[coords].InitWorldChunks();
            MapGenerator.Instance.GenerateMap(ChunkBatches[coords]);

        }
    }
    public bool IsChunkInWorkingCopy(Vector2Int v)
    {
        if (ChunksLoaded == null)
        {
            return false;
        }
        return ChunksLoaded.Contains(v);
    }
    public void AddChunkStoredInWorkingCopy(Vector2Int v)
    {
        if (ChunksLoaded == null)
        {
            ChunksLoaded = new List<Vector2Int>();
        }
        ChunksLoaded.Add(v);
    }
    
    public void PerformCreateNewChunksCheck()
    {
        Vector3 cameraPos = CameraController.Instance.transform.position;
        Vector2Int cameraCoords = ConvertPositionToChunkBatchCoords(cameraPos);
        List<Vector2Int> coords = GetAdjacentBatchCoords(cameraCoords);
        coords.Add(cameraCoords);
        bool needToRender = false;
        for(int x = 0; x < coords.Count; x++)
        {
            if (!ChunkBatches.ContainsKey(coords[x]))
            {
                CreateChunkBatch(coords[x]);
                needToRender = true;
            }
        }
        if (needToRender||DoWeHaveUndrawnChunks())
        {
            RenderWorldChunks();
        }
    }

    void PerformUnloadChunksCheck()
    {
        foreach(KeyValuePair<Vector2Int,WorldChunkBatch> kvp in ChunkBatches)
        {
            if (kvp.Value.CheckToUnloadChunkData())
            {
                return;
            }
        }
    }

    bool DoWeHaveUndrawnChunks()
    {
        foreach(KeyValuePair<Vector2Int,WorldChunkBatch> kvp in ChunkBatches)
        {
            if (kvp.Value.IsRendered == false)
            {
                return true;
            }
        }
        return false;
    }

    List<Vector2Int> GetAdjacentBatchCoords(Vector2Int coords)
    {
        List<Vector2Int> retVal = new List<Vector2Int>();

        retVal.Add(coords + new Vector2Int(ChunkBatchSize, 0));
        retVal.Add(coords + new Vector2Int(-ChunkBatchSize, 0));
        retVal.Add(coords + new Vector2Int(0, ChunkBatchSize));
        retVal.Add(coords + new Vector2Int(0,-ChunkBatchSize));

        return retVal;
    }
    Vector2Int ConvertPositionToChunkBatchCoords(Vector2Int pos)
    {
        int x = 0;

        if (pos.x < 0)
        {
            x = RoundToMultiple(pos.x, ChunkBatchSize);
        }
        else
        {
            x = RoundToMultiple(pos.x, ChunkBatchSize);
        }
        int y = 0;

        if (pos.y < 0)
        {
            y = RoundToMultiple(pos.y, ChunkBatchSize);
        }
        else
        {
            y = RoundToMultiple(pos.y, ChunkBatchSize);
        }
        return new Vector2Int(x - ChunkBatchSize, y - ChunkBatchSize);
    }

    Vector2Int ConvertPositionToChunkBatchCoords(Vector3 pos)
    {
        int x = 0;

        if (pos.x < 0)
        {
            x = RoundToMultiple(pos.x, ChunkBatchSize);
        }
        else
        {
            x = RoundToMultiple(pos.x, ChunkBatchSize);
        }
        int y = 0;

        if (pos.y < 0)
        {
            y = RoundToMultiple(pos.y, ChunkBatchSize);
        }
        else
        {
            y = RoundToMultiple(pos.y, ChunkBatchSize);
        }
        return new Vector2Int(x-ChunkBatchSize, y-ChunkBatchSize);
    }
    public int RoundToMultiple(float value, int roundTo)
    {
        return  Mathf.CeilToInt(value / roundTo) * roundTo;
    }
    public void RenderWorldChunks()
    {
       
        foreach(KeyValuePair<Vector2Int,WorldChunkBatch> kvp in ChunkBatches)
        {
            if (kvp.Value.RenderChunk())
            {

            }
            
             kvp.Value.CheckForCleanup();
            
        }
    }
    public Dictionary<Vector2Int, string> ExistingChunkData;

    public bool DoesChunkExist(Vector2Int coords)
    {
        if (ExistingChunkData == null)
        {
            return false;
        }
        return ExistingChunkData.ContainsKey(coords);
    }


    public bool DoesChunkExistInWorkingCopy(Vector2Int coords)
    {
        if (ChunksLoaded == null)
        {
            return false;
        }
        return ChunksLoaded.Contains(coords);
    }
    
    public void LoadChunksFromFile(string name)
    {
        Debug.Log("World Load: loading chunks from file " + name);
        ExistingChunkData = new Dictionary<Vector2Int, string>();
        string path = SerializationHelpers.GetSaveDirectory(name);
        string[] dir = Directory.GetFiles(path);
        string[] split = null;
        string fileName = "";
        Vector2Int coords = Vector2Int.zero;
        for(int x=0;x<dir.Length; x++)
        {

            if (dir[x].Contains(SerializationHelpers.WorldSectionExtension))
            {

                fileName = Path.GetFileNameWithoutExtension(dir[x]);
                split = fileName.Split("_",System.StringSplitOptions.RemoveEmptyEntries);
                
                Debug.Log("Loading " + dir[x]+" "+fileName);
                coords.x = int.Parse(split[0]);
                coords.y = int.Parse(split[1]);
                ExistingChunkData.Add(coords, dir[x]);
            }
        }
        Debug.Log("World Load: final count " + ExistingChunkData.Count );

        // ChunkBatches[new Vector2Int()].LoadChunksFromFile(name);
    }
    public WorldChunkBatch GetWorldChunkBatchFromPosition(Vector2Int pos,bool canCreateNew=false)
    {
        Vector2Int coords = ConvertPositionToChunkBatchCoords(pos);
        
            if (!ChunkBatches.ContainsKey(coords))
            {
            if (canCreateNew)
            {
                CreateChunkBatch(coords);
            }
            else
            {
                return null;
            }
            }

            return ChunkBatches[coords];
    }

    public WorldChunkBatch GetWorldChunkBatchFromPosition(Vector3 pos,bool canCreateNew = false)
    {
        Vector2Int coords = ConvertPositionToChunkBatchCoords(pos);
        if (!ChunkBatches.ContainsKey(coords))
        {
            if (canCreateNew)
            {
                CreateChunkBatch(coords);
            }
            else
            {
                return null;
            }
        }

            return ChunkBatches[coords];
    }


    public void OnBuildableFinished(BuildableStructure bs)
    {
        GetWorldChunkBatchFromPosition(bs.GetPosition()).OnBuildableFinished(bs);
       
    }

   public static int NewCalculateBatchCoords(float val)
    {
        bool isNegative = val < 0;
        
       
        if (isNegative)
        {
            int retVal = 0;
            while (retVal > val)
            {
                retVal -= WorldChunkManager.ChunkBatchSize;
            }
            return retVal;
        }
        else
        {
            float chunkCoord = RoundToMultiple(val, WorldChunkManager.ChunkBatchSize);
            if (chunkCoord > val)
            {
                chunkCoord -= ChunkBatchSize;
            }
            return Mathf.FloorToInt(chunkCoord);

        }
    }

    static float RoundToMultiple(float value, float roundTo)
    {
        return  Mathf.Floor(value / roundTo) * roundTo;
    }


    int localX = 0, localY = 0;
    int BatchCoordsX = 0, BatchCoordsY = 0;
    int ChunkCoordsX = 0, ChunkCoordsY = 0;
    int xMod, yMod;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">x pos/coord</param>
    /// <param name="y">y pos/coord</param>
    /// <param name="chunkBatch">Coords of the chunk batch that the position was in</param>
    /// <param name="chunkCoords">Coords of the chunk within the batch that the position was in</param>
    /// <param name="coords">Coords within the chunk that the position was in</param>
    public void ConvertPositionToChunkAndLocalCoords(float x, float y, out Vector2Int chunkBatch,out Vector2Int chunkCoords, out Vector2Int coords)
    {
        float mod = WorldChunkManager.ChunkBatchSize;
        xMod = Mathf.RoundToInt(x % mod);
        localX =xMod;
        ChunkCoordsX = Mathf.CeilToInt(localX / WorldChunkManager.ChunkSize);
        if (xMod != 0)
        {
            BatchCoordsX = Mathf.FloorToInt(x - localX);
            if (x < 0)
            {
                BatchCoordsX -= WorldChunkManager.ChunkBatchSize;
                localX = WorldChunkManager.ChunkBatchSize + localX;

            }
        }
        else
        {
            BatchCoordsX = Mathf.RoundToInt( x);
        }
       
        ChunkCoordsX = Mathf.CeilToInt(localX / WorldChunkManager.ChunkSize);
        localX -= WorldChunkManager.ChunkSize * ChunkCoordsX;
        mod = WorldChunkManager.ChunkBatchSize;
        yMod = Mathf.RoundToInt(y % mod);
        localY = yMod;
        if (yMod != 0)
        {
            BatchCoordsY = Mathf.FloorToInt(y - localY);
            if (y < 0)
            {
                BatchCoordsY -= WorldChunkManager.ChunkBatchSize;
                localY = WorldChunkManager.ChunkBatchSize + localY;
            }
        }
        else
        {
            BatchCoordsY = Mathf.RoundToInt(y);
        }
       
        ChunkCoordsY = Mathf.CeilToInt(localY / WorldChunkManager.ChunkSize);
        localY -= WorldChunkManager.ChunkSize * ChunkCoordsY;

        chunkBatch = new Vector2Int(NewCalculateBatchCoords(x), NewCalculateBatchCoords(y));
        chunkCoords = new Vector2Int(Mathf.Clamp( ChunkCoordsX,0, WorldChunkManager.ChunkSize - 1), Mathf.Clamp(ChunkCoordsY, 0, WorldChunkManager.ChunkSize - 1));
        coords = new Vector2Int(Mathf.Clamp(localX, 0, WorldChunkManager.ChunkSize-1), Mathf.Clamp(localY, 0, WorldChunkManager.ChunkSize - 1));
    }

    public WorldChunk GetWorldChunkFromPos(Vector3 pos)
    {
        Vector2Int chunkCoords = GetChunkCoordsFromWorldPos(pos);
        WorldChunkBatch batch = GetWorldChunkBatchFromPosition(pos);
        if (batch != null)
        {
            return batch.Chunks[chunkCoords.x, chunkCoords.y];
        }
        else
        {
            return null;
        }
        }

        public Vector2Int GetChunkCoordsFromWorldPos(Vector3 worldPos)
    {
        WorldChunkBatch batch = GetWorldChunkBatchFromPosition(worldPos);
        if (batch != null)
        {
            return batch.GetChunkCoordsFromWorldPos(worldPos);
        }
        else
        {
            return Vector2Int.zero;
        }
        }

        public WorldChunk GetWorldChunkFromTileCoords(Vector2Int coords,bool canCreateNew=false,bool debug=false)
    {
        WorldChunkBatch batch = GetWorldChunkBatchFromPosition(coords, canCreateNew);
        if (batch != null)
        {
            Vector2Int chunkCoords = batch.GetChunkCoordsFromTileCoords(coords,debug);
           
                return batch.Chunks[chunkCoords.x, chunkCoords.y];
        }
        else
        {
            return null;
        }
    }


    public Vector2Int GetChunkCoordsFromTileCoords(Vector2Int coords)
    {
        WorldChunkBatch batch = GetWorldChunkBatchFromPosition(coords);
        if (batch != null)
        {
            return batch.GetChunkCoordsFromTileCoords(coords);
        }
        else
        {
            return Vector2Int.zero;
        }
    }

        public void AddEnvironmentObjectInstanceToChunk(EnvironmentObjectInstance obj)
    {
        GetWorldChunkBatchFromPosition(obj.Position()).AddEnvironmentObject(obj, obj.Position());

    }

    public void AddContainerObject(Inventory toAdd)
    {
        GetWorldChunkBatchFromPosition(toAdd.transform.position).AddContainerObject(toAdd);
    }

    public void RemoveContainerObject(Inventory toRemove)
    {
        GetWorldChunkBatchFromPosition(toRemove.transform.position).RemoveContainerObject(toRemove);

    }

    public void AddResourceObject(ResourceInstance res)
    {
        GetWorldChunkBatchFromPosition(res.transform.position).AddResourceObject(res);
    }

    public void RemoveResourceObject(ResourceInstance res)
    {
        GetWorldChunkBatchFromPosition(res.transform.position).RemoveResourceObject(res);
    }

    public void AddConstructable(Constructable bs)
    {
        GetWorldChunkBatchFromPosition(bs.GetPosition()).AddConstructable(bs);
    }
    public void RemoveConstructable(Constructable bs, bool needsCleanup = true)
    {
        GetWorldChunkBatchFromPosition(bs.GetPosition()).RemoveConstructable(bs,needsCleanup);
    }
    private void Update()
    {
        PerformCreateNewChunksCheck();
        PerformUnloadChunksCheck();
    }


   public bool CoordsValid(int x,int y)
    {
        return x>=0&&y>=0&&x<Width&&y<Height;
    }
    Vector2Int batchCoords = new Vector2Int(), chunkCoords = new Vector2Int(), localCoords = new Vector2Int();
    public List<WorldChunk> GetChunksInRadius(float radius,Vector3 searchCenter)
    {
        List<WorldChunk> Chunks = new List<WorldChunk>();
        List<Vector2Int> batchesChecked = new List<Vector2Int>();

        WorldChunkBatch batch = null;
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(searchCenter.x, searchCenter.y, out batchCoords, out chunkCoords, out localCoords);
        if (ValidateCoords())
        {
             batch = WorldChunkManager.instance.ChunkBatches[batchCoords];
            if (!batchesChecked.Contains(batchCoords))
            {
                Chunks.AddRange(batch.GetChunksInRadius(radius, searchCenter));
            }
        }

        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(searchCenter.x+radius, searchCenter.y, out batchCoords, out chunkCoords, out localCoords);
        if (ValidateCoords())
        {
            batch = WorldChunkManager.instance.ChunkBatches[batchCoords];
            if (!batchesChecked.Contains(batchCoords))
            {
                Chunks.AddRange(batch.GetChunksInRadius(radius, searchCenter));
            }
        }
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(searchCenter.x-radius, searchCenter.y, out batchCoords, out chunkCoords, out localCoords);
        if (ValidateCoords())
        {
            batch = WorldChunkManager.instance.ChunkBatches[batchCoords];
            if (!batchesChecked.Contains(batchCoords))
            {
                Chunks.AddRange(batch.GetChunksInRadius(radius, searchCenter));
            }
        }
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(searchCenter.x, searchCenter.y+radius, out batchCoords, out chunkCoords, out localCoords);
        if (ValidateCoords())
        {
            batch = WorldChunkManager.instance.ChunkBatches[batchCoords];
            if (!batchesChecked.Contains(batchCoords))
            {
                Chunks.AddRange(batch.GetChunksInRadius(radius, searchCenter));
            }
        }
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(searchCenter.x, searchCenter.y-radius, out batchCoords, out chunkCoords, out localCoords);
        if (ValidateCoords())
        {
            batch = WorldChunkManager.instance.ChunkBatches[batchCoords];
            if (!batchesChecked.Contains(batchCoords))
            {
                Chunks.AddRange(batch.GetChunksInRadius(radius, searchCenter));
            }
        }
        return Chunks;
    }
    bool ValidateCoords()
    {
        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(batchCoords) == false)
        {
            return false;
        }
        return true;
    }
    const bool DrawNodeWalkable = false, DrawNodeNeighbours = false;
    void DebugDrawChunks()
    {
        foreach (KeyValuePair<Vector2Int, WorldChunkBatch> kvp in ChunkBatches)
        {
            kvp.Value.DebugDrawChunks();
        }
       
    }





  


    public void OnUnitCreated(Unit u)
    {
        GetWorldChunkBatchFromPosition(u.transform.position).OnUnitCreated(u);
      
    }

    public void OnUnitMove(Unit u)
    {
        GetWorldChunkBatchFromPosition(u.transform.position).OnUnitMove(u);

      
    }

    public void OnUnitDeath(Unit u)
    {
        GetWorldChunkBatchFromPosition(u.transform.position).OnUnitDeath(u);
    }

}
