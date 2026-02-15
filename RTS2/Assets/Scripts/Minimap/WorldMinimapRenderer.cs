using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMinimapRenderer : MinimapRenderer
{
    Dictionary<Vector2Int, Color[,]> BatchTextures=new Dictionary<Vector2Int, Color[,]>();
    Dictionary<Vector2Int, bool> BatchNeedsUpdating = new Dictionary<Vector2Int, bool>();
    public WorldChunkBatch BatchImIn;
    Vector3 bottomLeftCorner = Vector3.zero, topRightCorner = Vector3.zero;
    int xc = 0, yc = 0;
    Vector2Int coords, curCoords;
    bool updating = false;
    public bool IsDataUpdateDone()
    {
        return !updating;
    }

    public Color[,] GetCurChunkColours()
    {
        Vector3 cameraPos = CameraController.Instance.transform.position;
        BatchImIn = WorldChunkManager.Instance.GetWorldChunkBatchFromPosition(cameraPos);
        if (BatchImIn != null)
        {
            Debug.Log("Minimap: refreshing data from " + BatchImIn.coords+","+ BatchTextures.ContainsKey(BatchImIn.coords));
            if (BatchTextures.ContainsKey(BatchImIn.coords))
            {
                return BatchTextures[BatchImIn.coords];
            }
        }
        return new Color[WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize];
    }

    public void StartRefresh()
    {
        updating = true;
        ClearColours();
        Vector3 cameraPos = CameraController.Instance.transform.position;
        bottomLeftCorner = cameraPos - new Vector3(WorldChunkManager.ChunkBatchSize / 2, WorldChunkManager.ChunkBatchSize / 2, 0);
        topRightCorner = cameraPos + new Vector3(WorldChunkManager.ChunkBatchSize / 2, WorldChunkManager.ChunkBatchSize / 2, 0);
        coords = new Vector2Int(Mathf.CeilToInt(cameraPos.x), Mathf.CeilToInt(cameraPos.y));
        curCoords = coords;
        xc = coords.x - WorldChunkManager.ChunkBatchSize;
        yc = coords.y - WorldChunkManager.ChunkBatchSize;
    }


    int count = 0;
    public override void RefreshData()
    {
        curCoords.x = xc;
        curCoords.y = yc;
        
        BatchImIn = WorldChunkManager.Instance.GetWorldChunkBatchFromPosition(curCoords);
        //Debug.Log("Minimap: refreshing data from " + curCoords.ToString() + " batch exists " + (BatchImIn != null));
        Vector2Int colArCoords = Vector2Int.zero;
        Color cur = Color.black ;
        if (BatchImIn != null)
        {
            if (!BatchTextures.ContainsKey(BatchImIn.coords))
            {
                BatchTextures.Add(BatchImIn.coords, new Color[WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize]);
                BatchNeedsUpdating.Add(BatchImIn.coords, true);
            }
            if (BatchNeedsUpdating[BatchImIn.coords])
            {
                Debug.Log("Minimap: generating batch texture for "+ BatchImIn.coords);

                for (int x = 0; x < WorldChunkManager.ChunkSize; x++)
                {
                    for (int y = 0; y < WorldChunkManager.ChunkSize; y++)
                    {
                        for (int x1 = 0; x1 < WorldChunkManager.ChunkSize; x1++)
                        {
                            for (int y1 = 0; y1 < WorldChunkManager.ChunkSize; y1++)
                            {
                                cur = WorldRenderer.Instance.WorldTilesManager.GetTileMinimapColour(BatchImIn.Chunks[x, y].ChunkTiles[x1, y1].tileType);

                                colArCoords.x = x1 + (WorldChunkManager.ChunkSize * x);
                                colArCoords.y = y1 + (WorldChunkManager.ChunkSize * y);
                                BatchTextures[BatchImIn.coords][colArCoords.x, colArCoords.y] = cur;

                            }

                        }
                    }
                }
            }

                BatchNeedsUpdating[BatchImIn.coords] = false;
        }


        yc += WorldChunkManager.ChunkBatchSize;
        if (yc > coords.y + WorldChunkManager.ChunkBatchSize)
        {
            yc = coords.y - WorldChunkManager.ChunkBatchSize;
            xc += WorldChunkManager.ChunkBatchSize;

            if (xc > coords.x + WorldChunkManager.ChunkBatchSize)
            {

                xc = xc = coords.x - WorldChunkManager.ChunkBatchSize;
            }
        }
        count++;
        if (count >= 9)
        {
            updating = false;
            count = 0;
        }

    }



}
