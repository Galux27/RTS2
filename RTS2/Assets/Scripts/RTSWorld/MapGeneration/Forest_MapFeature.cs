using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
[CreateAssetMenu(fileName = "Map Feature", menuName = "Map Features/Forest", order = 1)]
public class Forest_MapFeature :MapFeatureBase
{
    public List<string> ValidObjectsForFeature;
    public int MinQuantityToSpawn,MaxQuantityToSpawn;
    public override void GenerateFeature(WorldChunkBatch toGenerateIn)
    {
        int width = Random.Range(MinWidth, MaxWidth);
        int height = Random.Range(MinHeight, MaxHeight);
        Vector2Int center = toGenerateIn.coords + new Vector2Int(Mathf.Clamp( Random.Range(width, WorldChunkManager.ChunkBatchSize- width),0,WorldChunkManager.ChunkBatchSize), Mathf.Clamp(Random.Range(width, WorldChunkManager.ChunkBatchSize- width), 0, WorldChunkManager.ChunkBatchSize));
       
        int x = 0, y = 0;
        int quantitiyToSpawn = Random.Range(MinQuantityToSpawn, MaxQuantityToSpawn);
        int objectToCreate = 0;
        WorldTile tileChecking = null;
        WorldChunk tileChunk = null;
        Vector2Int chunkCoords = new Vector2Int(0, 0);
        EnvironmentObjectInstance toAdd = null, existing = null;
        for (int q = 0; q < quantitiyToSpawn; q++)
        {
            x = Random.Range(- width, width);
            y = Random.Range(- height,  height);
           
            {
                objectToCreate = Random.Range(0, ValidObjectsForFeature.Count);
                Debug.Log("Tree: trying to Creating tree at " + center +","+ new Vector2Int(x, y));

                GenerateTree(center+ new Vector2Int(x, y), ValidObjectsForFeature[objectToCreate]);   
            }
        }
    }
    Vector2Int batch, chunk, tile;
    public void GenerateTree(Vector2Int coords,string objectToCreate)
    {

        EnvironmentObject obj = EnvironmentObjectManager.Instance.AllObjects[objectToCreate];
        bool valid = true;
        List<WorldTile> tiles = new List<WorldTile>();
        for(int x=coords.x;x<coords.x+obj.Width;x++)
        {
            for(int y = coords.y; y < coords.y + obj.Height; y++)
            {
                WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(x, y, out batch, out chunk, out tile);
                if (WorldChunkManager.Instance.DoesBatchExist(batch))
                {
                    tiles.Add(WorldChunkManager.Instance.ChunkBatches[batch].Chunks[chunk.x, chunk.y].ChunkTiles[tile.x, tile.y]);

                    if (!WorldChunkManager.Instance.ChunkBatches[batch].Chunks[chunk.x, chunk.y].ChunkTiles[tile.x, tile.y].CanGeneratePropOnTile())
                    {
                        valid = false;
                        break;
                    }
                }
                else
                {
                    valid = false;
                    break;
                }
                }
                if (!valid)
            {
                break;
            }
        }

        if (valid)
        {
            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(coords.x, coords.y, out batch, out chunk, out tile);

            Debug.Log("Tree: Creating tree at " + coords);
            EnvironmentObjectInstance toAdd = new EnvironmentObjectInstance(coords.x,coords.y, objectToCreate);
            WorldChunkManager.Instance.ChunkBatches[batch].Chunks[chunk.x, chunk.y].AddEnvironmentObject(toAdd);
            WorldController.Instance.SetTilesAroundEnvrionmentObjectTraversable(toAdd, !obj.BlocksTile);
            for(int x = 0; x < tiles.Count; x++)
            {
                tiles[x].CanPutDecorationsOn = false;
            }
        }
    }
}
