using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CamerDebugInfo : MonoBehaviour
{
    public WorldChunkBatch BatchImIn;
    public WorldChunk ChunkImIn;
    public OverworldTile OverworldTileImIn;
    public WorldTile TileImIn;
    public int NodeImOver;
    public PathNodeID IdImIn;
    public bool DrawGrid = false;

    // Update is called once per frame
    void Update()
    {
        BatchImIn = WorldChunkManager.Instance.GetWorldChunkBatchFromPosition(new Vector2Int(Mathf.CeilToInt( this.transform.position.x),Mathf.CeilToInt( this.transform.position.y)));
        if(BatchImIn != null)
        {
            OverworldTileImIn = OverworldGenerator.Instance.GetOverworldTile(BatchImIn.OverworldCoords);

            Vector2Int coords = BatchImIn.GetChunkCoordsFromWorldPos(this.transform.position);
            ChunkImIn = BatchImIn.Chunks[coords.x, coords.y];
            TileImIn = BatchImIn.GetTileFromPosition(this.transform.position);
            if (ChunkImIn == null)
            {
                return;
            }
            NodeImOver = ChunkImIn.PathfindingNodes[TileImIn.Local.x, TileImIn.Local.y].PathNodeGroupID;
            IdImIn = NodeIDPathing.GetPathNodeID(NodeImOver);
            if (TileImIn != null)
            {
                Debug.DrawLine(this.transform.position, new Vector3(TileImIn.Coords().x, TileImIn.Coords().y, 0), Color.cyan);
            }
        }
        else
        {
            ChunkImIn = null;
        }

        if (DrawGrid)
        {
            Vector2Int start = BatchImIn.coords;
            Vector2Int pos = Vector2Int.zero;
            Vector2Int dest = Vector2Int.zero;
            for(int x = -5; x < 5; x++)
            {
                for (int y = -5; y < 5; y++)
                {
                    pos.x = start.x + (x * WorldChunkManager.ChunkBatchSize);
                    pos.y = start.y+(y * WorldChunkManager.ChunkBatchSize);
                    DrawGridForChunkBatch(pos);

                    dest = pos;
                    dest.x += WorldChunkManager.ChunkBatchSize;
                    Debug.DrawLine((Vector2) pos, (Vector2)dest, Color.yellow);
                    dest = pos;
                    dest.y += WorldChunkManager.ChunkBatchSize;
                    Debug.DrawLine((Vector2)pos, (Vector2)dest, Color.yellow);
                  
                }
            }
        }
    }

    void DrawGridForChunkBatch(Vector2Int batch)
    {
        Vector2Int start = batch;
        Vector2Int pos = Vector2Int.zero;
        Vector2Int dest = Vector2Int.zero;
        for (int x = 0; x < WorldChunkManager.ChunkSize; x++)
        {
            for (int y = 0; y < WorldChunkManager.ChunkSize; y++)
            {
                pos.x = start.x + (x * WorldChunkManager.ChunkSize);
                pos.y = start.y + (y * WorldChunkManager.ChunkSize);

                dest = pos;
                dest.x += WorldChunkManager.ChunkSize;
                Debug.DrawLine((Vector2)pos, (Vector2)dest, Color.green);
                dest = pos;
                dest.y += WorldChunkManager.ChunkSize;
                Debug.DrawLine((Vector2)pos, (Vector2)dest, Color.green);
            }
        }
    }
    
}
