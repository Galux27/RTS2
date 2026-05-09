using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingDebugView : MonoBehaviour
{
    public WorldChunkBatch BatchImIn;
    public WorldChunk ChunkImIn;
    public OverworldTile OverworldTileImIn;
    public WorldTile TileImIn;
    public int RangeToDraw=50;
    // Update is called once per frame
    void Update()
    {
        if (!DebugCheats.Instance.DoWeDrawDebugPathfinidng())
        {
            return;
        }
        BatchImIn = WorldChunkManager.Instance.GetWorldChunkBatchFromPosition(new Vector2Int(Mathf.CeilToInt( this.transform.position.x), Mathf.CeilToInt(this.transform.position.y)));
        if (BatchImIn != null)
        {
            OverworldTileImIn = OverworldGenerator.Instance.GetOverworldTile(BatchImIn.OverworldCoords);

            Vector2Int coords = BatchImIn.GetChunkCoordsFromWorldPos(this.transform.position);
            ChunkImIn = BatchImIn.Chunks[coords.x, coords.y];
            TileImIn = BatchImIn.GetTileFromPosition(this.transform.position);
            //if (TileImIn != null)
            //{
            //    Debug.DrawLine(this.transform.position, new Vector3(TileImIn.Coords().x, TileImIn.Coords().y, 0), Color.cyan);
            //}
        }
        else
        {
            ChunkImIn = null;
        }
        float dist = 0f;
        for(int x=0;x<BatchImIn.Chunks.GetLength(0); x++)
        {
            for(int y=0;y<BatchImIn.Chunks.GetLength(1);y++)
            {
                dist = Vector2Int.Distance(BatchImIn.Chunks[x, y].WorldCoords, ChunkImIn.WorldCoords);
                if (dist < RangeToDraw)
                {
                    DrawPathfindingForChunk(BatchImIn.Chunks[x,y]);
                }
            }
        }
    }

    Color GetColourFromFeatures(WorldChunk chunk,Vector2Int coords)
    {
        if (chunk.ChunkTiles[coords.x, coords.y].TileContents.Contains(WorldTileContents.Wall))
        {
            return Color.blue;
        }
        return Color.red;
    }

    void DrawPathfindingForChunk(WorldChunk toDraw)
    {
        int length = toDraw.PathfindingNodes.GetLength(0);
        int neighbours = 0;
        PathfindingNode nodeToDraw = null;
        for (int x = 0; x < length; x++) { 
            for(int y=0;y< length; y++)
            {
                nodeToDraw = toDraw.PathfindingNodes[x, y];
                neighbours = nodeToDraw.neighbours.Count;
                if (nodeToDraw.IsPassable == false)
                {
                    continue;
                }
                for(int i = 0; i < neighbours; i++)
                {
                    if (nodeToDraw.neighbours[i].IsAccessable==false)
                    {
                        DrawLine(nodeToDraw.worldPos, nodeToDraw.neighbours[i].Node.worldPos,GetColourFromFeatures(toDraw,new Vector2Int(nodeToDraw.localX,nodeToDraw.localY)));
                    }
                    else
                    {
                        DrawLine(nodeToDraw.worldPos, nodeToDraw.neighbours[i].Node.worldPos, GetColourFromFeatures(toDraw, new Vector2Int(nodeToDraw.localX, nodeToDraw.localY)));

                    }
                }
                }
        }
    }

    Color GetColourForNode(PathfindingNode node)
    {
        if (node.IsPassable == false)
        {
            return Color.magenta;
        }
        else
        {
            return PathfindingNode.GetPathGroupColour(node.PathNodeGroupID);
        }
    }

    void DrawLine(Vector3 startPoint,Vector3 endPoint,Color color)
    {
        Debug.DrawLine(startPoint, endPoint, color);
    }
}
