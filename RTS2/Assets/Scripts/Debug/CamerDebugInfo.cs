using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CamerDebugInfo : MonoBehaviour
{
    public WorldChunkBatch BatchImIn;
    public WorldChunk ChunkImIn;
    public OverworldTile OverworldTileImIn;
   

    // Update is called once per frame
    void Update()
    {
        BatchImIn = WorldChunkManager.Instance.GetWorldChunkBatchFromPosition(new Vector2Int((int) this.transform.position.x, (int)this.transform.position.y));
        if(BatchImIn != null)
        {
            OverworldTileImIn = OverworldGenerator.Instance.GetOverworldTile(BatchImIn.OverworldCoords);

            Vector2Int coords = BatchImIn.GetChunkCoordsFromWorldPos(this.transform.position);
            ChunkImIn = BatchImIn.Chunks[coords.x, coords.y];
        }
        else
        {
            ChunkImIn = null;
        }
    }
}
