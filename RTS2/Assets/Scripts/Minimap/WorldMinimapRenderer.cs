using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMinimapRenderer : MinimapRenderer
{
    Dictionary<Vector2Int, Color[,]> BatchTextures=new Dictionary<Vector2Int, Color[,]>();
    Dictionary<Vector2Int, bool> BatchNeedsUpdating = new Dictionary<Vector2Int, bool>();
    Color[,] workingCopy = new Color[WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize];
    public WorldChunkBatch BatchImIn;
    Vector3 bottomLeftCorner = Vector3.zero, topRightCorner = Vector3.zero;
    int xc = 0, yc = 0;
    Vector2Int coords, curCoords;
    bool updating = false;
    public bool IsDataUpdateDone()
    {
        return !updating;
    }
    readonly List<Vector3> Modifiers = new List<Vector3>() { new Vector3(WorldChunkManager.ChunkBatchSize/2, 0,0),
    new Vector3(-WorldChunkManager.ChunkBatchSize/2, 0,0) ,
    new Vector3(0, WorldChunkManager.ChunkBatchSize/2,0),
    new Vector3(0, -WorldChunkManager.ChunkBatchSize/2,0),
    new Vector3( WorldChunkManager.ChunkBatchSize/2, WorldChunkManager.ChunkBatchSize/2,0),
    new Vector3( WorldChunkManager.ChunkBatchSize/2, -WorldChunkManager.ChunkBatchSize/2,0),
    new Vector3(- WorldChunkManager.ChunkBatchSize/2, WorldChunkManager.ChunkBatchSize/2,0),
    new Vector3(- WorldChunkManager.ChunkBatchSize/2,- WorldChunkManager.ChunkBatchSize/2,0),
    };
    public Color[,] GetCurChunkColours()
    {
        Vector3 cameraPos = CameraController.Instance.transform.position;

        List<WorldChunkBatch> batchesForMinimap = new List<WorldChunkBatch>();

        for (int x = 0; x < Modifiers.Count; x++)
        {
            cameraPos = CameraController.Instance.transform.position + Modifiers[x];
            BatchImIn = WorldChunkManager.Instance.GetWorldChunkBatchFromPosition(cameraPos);
            if (!batchesForMinimap.Contains(BatchImIn) && BatchImIn != null)
            {
                batchesForMinimap.Add(BatchImIn);
            }
        }
        cameraPos = CameraController.Instance.transform.position;

        BatchImIn = WorldChunkManager.Instance.GetWorldChunkBatchFromPosition(cameraPos);
        Vector2Int batch = Vector2Int.zero, chunk = Vector2Int.zero, tile = Vector2Int.zero;
        Vector3 coords = CameraController.Instance.transform.position -new Vector3(WorldChunkManager.ChunkBatchSize / 2, WorldChunkManager.ChunkBatchSize / 2, 0);
        
        float xLerp = 0f, yLerp = 0f;
        Vector2 startingCoords = new Vector2();
        Vector2 endingCoords = new Vector2();
        Vector2Int TextureCoords = new Vector2Int();
        Vector2Int WorkingCoords = new Vector2Int();
        Vector2 startWorldCoords = new Vector2();
        Vector2 endWorldCoords = new Vector2();
        float increment = 1f / WorldChunkManager.ChunkBatchSize;
        Color[,] checking = null;
        for (int q = 0; q < batchesForMinimap.Count; q++) 
        {
            if (!BatchTextures.ContainsKey(batchesForMinimap[q].coords))
            {
                continue;
            }
            checking = BatchTextures[batchesForMinimap[q].coords];
            batch = batchesForMinimap[q].coords + new Vector2Int(WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize);
            startingCoords = new Vector2(Mathf.InverseLerp(batchesForMinimap[q].coords.x, batch.x, bottomLeftCorner.x)
                , Mathf.InverseLerp(batchesForMinimap[q].coords.y, batch.y, bottomLeftCorner.y));

            endingCoords = new Vector2(Mathf.InverseLerp(batchesForMinimap[q].coords.x, batch.x, topRightCorner.x)
               , Mathf.InverseLerp(batchesForMinimap[q].coords.y, batch.y, topRightCorner.y));


            startWorldCoords.x = Mathf.Lerp(batchesForMinimap[q].coords.x, batch.x, startingCoords.x);
            startWorldCoords.y = Mathf.Lerp(batchesForMinimap[q].coords.y, batch.y, startingCoords.y);

            endWorldCoords.x = Mathf.Lerp(batchesForMinimap[q].coords.x, batch.x, endingCoords.x);
            endWorldCoords.y = Mathf.Lerp(batchesForMinimap[q].coords.y, batch.y, endingCoords.y);

            for (float x=startingCoords.x; x < endingCoords.x; x += increment)
            {
                TextureCoords.x = Mathf.FloorToInt( Mathf.Lerp(0, WorldChunkManager.ChunkBatchSize, x));
                WorkingCoords.x = Mathf.FloorToInt(Mathf.InverseLerp(bottomLeftCorner.x, topRightCorner.x, Mathf.Lerp(batchesForMinimap[q].coords.x, batch.x, x)) * WorldChunkManager.ChunkBatchSize);
                for (float y = startingCoords.y; y < endingCoords.y; y += increment)
                {
                    TextureCoords.y = Mathf.FloorToInt(Mathf.Lerp(0, WorldChunkManager.ChunkBatchSize, y));
                    WorkingCoords.y = Mathf.FloorToInt(Mathf.InverseLerp(bottomLeftCorner.y, topRightCorner.y, Mathf.Lerp(batchesForMinimap[q].coords.y, batch.y, y)) * WorldChunkManager.ChunkBatchSize);
                    workingCopy[WorkingCoords.x, WorkingCoords.y] = checking[TextureCoords.x, TextureCoords.y];
                }
            }

        }
        
        return workingCopy;

        //if (BatchImIn != null)
        //{
        //    Debug.Log("Minimap: refreshing data from " + BatchImIn.coords+","+ BatchTextures.ContainsKey(BatchImIn.coords));
        //    if (BatchTextures.ContainsKey(BatchImIn.coords))
        //    {
        //        return BatchTextures[BatchImIn.coords];
        //    }
        //}
        //return new Color[WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize];
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

                for (int x = 0; x < WorldChunkManager.ChunksPerBatch; x++)
                {
                    for (int y = 0; y < WorldChunkManager.ChunksPerBatch; y++)
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
