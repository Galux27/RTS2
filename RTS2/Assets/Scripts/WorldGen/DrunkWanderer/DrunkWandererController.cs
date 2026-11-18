using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class DrunkWandererController : MonoBehaviour
{
    
    static DrunkWandererController instance;
    public static DrunkWandererController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DrunkWandererController>(true);
            }
            return instance;
        }
    }

    Dictionary<Vector2Int, DrunkWandererResult> RiverData = new Dictionary<Vector2Int, DrunkWandererResult>();

    public Vector2Int ConvertWorldCoordsToDrunkCoords(Vector2Int position)
    {
        Vector2Int retVal = new Vector2Int(RoundToMultiple(position.x, DrunkWanderer.Width()), RoundToMultiple(position.y, DrunkWanderer.Height()));
        if (retVal.x > position.x)
        {
            retVal.x -= DrunkWanderer.Width();
        }

        if (retVal.y > position.y)
        {
            retVal.y-=DrunkWanderer.Height();
        }

        return retVal;
    }
    int RoundToMultiple(int value, int roundTo)
    {
        return (value / roundTo) * roundTo;
    }

    public DrunkWandererResult GetRiverData(WorldChunkBatch batch)
    {
        //convert the world coords to overworld coords
        Vector2Int overworldCoords = batch.OverworldCoords;
        Vector2Int batchChunkIsIn = ConvertWorldCoordsToDrunkCoords(batch.coords);
        //work out the overworld coords of the start point of the drunk wander dataset
        Vector2Int overworldCoordsOfBatchStart = new Vector2Int(batchChunkIsIn.x / WorldChunkManager.ChunkBatchSize, batchChunkIsIn.y / WorldChunkManager.ChunkBatchSize);
        Debug.Log("River Gen: looking for river at " + overworldCoords + " from " + batch.coords + " is in " + batchChunkIsIn + " overworld coords of corner " + overworldCoordsOfBatchStart);
        //Create data for the rivers of the overworld tiles that it covers
        //call the drunk wander generate method
        if (!RiverData.ContainsKey(overworldCoordsOfBatchStart))
        {
            GenerateRiverData(overworldCoordsOfBatchStart, batchChunkIsIn);
        }

        return RiverData[overworldCoordsOfBatchStart];
        //write something to convert batch,chunk,tile into coords that can sample the drunk wander
        //should just be drunk wander start - batch + ((chunk-1)*chunksize) + tile

    }

  

    void GenerateRiverData(Vector2Int overworldStartCorner,Vector2Int cornerAsBatchCoords)
    {
        Dictionary<Vector2Int, Vector2Int> RiverPoints = new Dictionary<Vector2Int, Vector2Int>();
        int width = DrunkWanderer.Width()/WorldChunkManager.ChunkBatchSize;
        int height = DrunkWanderer.Height()/WorldChunkManager.ChunkBatchSize;
        Vector2Int pos = new Vector2Int();
        Vector2Int halfChunk = new Vector2Int(WorldChunkManager.ChunkBatchSize / 2, WorldChunkManager.ChunkBatchSize / 2);
        List<List<Vector2Int>> Positions = new List<List<Vector2Int>>();

        for (int x = overworldStartCorner.x; x < overworldStartCorner.x + width; x++) 
        {
            for (int y = overworldStartCorner.y; y < overworldStartCorner.y + height; y++)
            {
                pos.x = x;
                pos.y = y;
                if (!OverworldGenerator.Instance.GetOverworldTile(pos).Features.Contains(OverworldFeature.River))
                {
                    continue;
                }
                else
                {
                    RiverPoints.Add(pos, (pos * WorldChunkManager.ChunkBatchSize) + halfChunk + new Vector2Int(Random.Range(-10, 10), Random.Range(-10, 10)));
                }
               
            }
        }
        Vector2Int pos2 = new Vector2Int();

        foreach(KeyValuePair<Vector2Int,Vector2Int> kvp in RiverPoints)
        {
            pos = kvp.Key;
            Debug.Log("River Gen: Point found " + kvp.Key+"->"+kvp.Value);
            if (pos.x < OverworldGenerator.Instance.OverworldWidth - 1)
            {
                pos2 = pos;
                pos2.x += 1;
                if (RiverPoints.ContainsKey(pos) && RiverPoints.ContainsKey(pos2))
                {
                    List<Vector2Int> toAdd = new List<Vector2Int>();
                    toAdd.Add(RiverPoints[pos] - cornerAsBatchCoords); 
                    toAdd.Add(RiverPoints[pos2] - cornerAsBatchCoords);
                    Positions.Add(toAdd);
                }
            }
            if (pos.y < OverworldGenerator.Instance.OverworldHeight - 1)
            {
                pos2 = pos;
                pos2.y += 1;
                if (RiverPoints.ContainsKey(pos) && RiverPoints.ContainsKey(pos2))
                {
                    List<Vector2Int> toAdd = new List<Vector2Int>();
                    toAdd.Add(RiverPoints[pos] - cornerAsBatchCoords);
                    toAdd.Add(RiverPoints[pos2] - cornerAsBatchCoords);
                    Positions.Add(toAdd);
                }
            }

        }
        int resultWidth = DrunkWanderer.Width();
        int resultHeight = DrunkWanderer.Height();
        DrunkWandererResult result = new DrunkWandererResult(resultWidth, resultHeight, Positions, cornerAsBatchCoords);

        DrunkWandererResult neighbour = GetNeighbourIfExists(Vector2Int.up, overworldStartCorner);
        if (neighbour != null)
        {
            for(int x = 0; x < resultWidth; x+=WorldChunkManager.ChunkBatchSize)
            {
                if (OverworldGenerator.Instance.GetOverworldTile( result.Data[x,resultHeight-1].OverworldCoords).Features.Contains(OverworldFeature.River) 
                    && OverworldGenerator.Instance.GetOverworldTile(neighbour.Data[x, 0].OverworldCoords).Features.Contains(OverworldFeature.River))
                {
                    //create an extra drunk wanderer and run it through between the two points in the drunk wanderers
                }
            }
        }

        neighbour = GetNeighbourIfExists(Vector2Int.down, overworldStartCorner);
        if (neighbour != null)
        {
            for (int x = 0; x < resultWidth; x += WorldChunkManager.ChunkBatchSize)
            {
                if (OverworldGenerator.Instance.GetOverworldTile(neighbour.Data[x, resultHeight - 1].OverworldCoords).Features.Contains(OverworldFeature.River) 
                    && OverworldGenerator.Instance.GetOverworldTile(result.Data[x, 0].OverworldCoords).Features.Contains(OverworldFeature.River))
                {
                    //create an extra drunk wanderer and run it through between the two points in the drunk wanderers
                }
            }
        }

        neighbour = GetNeighbourIfExists(Vector2Int.right, overworldStartCorner);
        if (neighbour != null)
        {
            for (int y = 0; y < resultHeight; y += WorldChunkManager.ChunkBatchSize)
            {
                if (OverworldGenerator.Instance.GetOverworldTile(neighbour.Data[0, y].OverworldCoords).Features.Contains(OverworldFeature.River)
                    && OverworldGenerator.Instance.GetOverworldTile(result.Data[resultWidth-1, y].OverworldCoords).Features.Contains(OverworldFeature.River))
                {
                    //create an extra drunk wanderer and run it through between the two points in the drunk wanderers
                }
            }
        }

        neighbour = GetNeighbourIfExists(Vector2Int.left, overworldStartCorner);
        if (neighbour != null)
        {
            for (int y = 0; y < resultHeight; y += WorldChunkManager.ChunkBatchSize)
            {
                if (OverworldGenerator.Instance.GetOverworldTile(result.Data[0, y].OverworldCoords).Features.Contains(OverworldFeature.River)
                    && OverworldGenerator.Instance.GetOverworldTile(neighbour.Data[resultWidth - 1, y].OverworldCoords).Features.Contains(OverworldFeature.River))
                {
                    //create an extra drunk wanderer and run it through between the two points in the drunk wanderers
                }
            }
        }
        RiverData.Add(overworldStartCorner, result);
    }

    

    DrunkWandererResult GetNeighbourIfExists(Vector2Int direction,Vector2Int coords)
    {
        Vector2Int pos = coords + new Vector2Int(direction.x * DrunkWanderer.Width(), direction.y * DrunkWanderer.Height());
        if (RiverData.ContainsKey(pos))
        {
            return RiverData[pos];
        }
        return null;
    }

    
}
