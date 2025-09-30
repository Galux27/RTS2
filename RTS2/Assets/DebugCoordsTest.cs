using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class DebugCoordsTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public int inBatchX, inBatchY;
    public int BatchCoordsX, BatchCoordsY;
    public int ChunkCoordsX, ChunkCoordsY;
    public int NewBatchX,NewBatchY;
    // Update is called once per frame
    void Update()
    {
        float mod = WorldChunkManager.ChunkBatchSize;
        inBatchX = Mathf.FloorToInt(this.transform.position.x % mod);
        ChunkCoordsX = Mathf.CeilToInt(inBatchX / WorldChunkManager.ChunkSize);
        BatchCoordsX = Mathf.FloorToInt(this.transform.position.x - inBatchX);
        if (this.transform.position.x < 0)
        {
            BatchCoordsX-=WorldChunkManager.ChunkBatchSize;
            inBatchX = WorldChunkManager.ChunkBatchSize + inBatchX;

        }
        ChunkCoordsX = Mathf.CeilToInt(inBatchX / WorldChunkManager.ChunkSize);
        inBatchX -= WorldChunkManager.ChunkSize * ChunkCoordsX;
        mod = WorldChunkManager.ChunkBatchSize; 
        inBatchY = Mathf.FloorToInt(this.transform.position.y % mod);
        BatchCoordsY = Mathf.FloorToInt( this.transform.position.y - inBatchY);
        if (this.transform.position.y < 0)
        {
            BatchCoordsY -= WorldChunkManager.ChunkBatchSize;
            inBatchY=WorldChunkManager.ChunkBatchSize+inBatchY;
        }
        ChunkCoordsY = Mathf.CeilToInt(inBatchY / WorldChunkManager.ChunkSize);
        inBatchY -= WorldChunkManager.ChunkSize * ChunkCoordsY;
        NewBatchX = WorldChunkManager.NewCalculateBatchCoords(this.transform.position.x);
        NewBatchY = WorldChunkManager.NewCalculateBatchCoords(this.transform.position.y);

    }

    public void ConvertPositionToChunkAndLocalCoords(float x, float y, out Vector2Int chunkBatch, out Vector2Int coords)
    {
        float mod = WorldChunkManager.ChunkBatchSize;
        int inBatchX, inBatchY;
        int BatchCoordsX, BatchCoordsY;
        inBatchX = Mathf.FloorToInt(x % mod);
        BatchCoordsX = Mathf.FloorToInt(x - inBatchX);

        if (x < 0)
        {
            BatchCoordsX -= WorldChunkManager.ChunkBatchSize;
            inBatchX = WorldChunkManager.ChunkBatchSize + inBatchX;
        }

        mod = WorldChunkManager.ChunkBatchSize;
        inBatchY = Mathf.FloorToInt(y % mod);
        BatchCoordsY = Mathf.FloorToInt(y - inBatchY);
        if (y < 0)
        {
            BatchCoordsY -= WorldChunkManager.ChunkBatchSize;
            inBatchY = WorldChunkManager.ChunkBatchSize + inBatchY;
        }
        chunkBatch = new Vector2Int(BatchCoordsX, BatchCoordsY);
        coords = new Vector2Int(inBatchX, inBatchY);
    }



    public int RoundToMultiple(float value, float roundTo)
    {
        return Mathf.CeilToInt( Mathf.RoundToInt(value / roundTo) * roundTo);
    }
}
