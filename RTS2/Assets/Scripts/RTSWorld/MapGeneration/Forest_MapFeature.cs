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
        GenerateMK2(toGenerateIn);
        return;
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

                GenerateTree(center+ new Vector2Int(x, y), ValidObjectsForFeature[objectToCreate]);   
            }
        }
    }

    void GenerateMK2(WorldChunkBatch toGenerateIn)
    {
        int count = 0;
        WorldChunk chunk = null;
        int r = 0;
        tiles = new List<WorldTile>();
        int quantitiyToSpawn = Random.Range(MinQuantityToSpawn, MaxQuantityToSpawn);
        int tileCount = WorldChunkManager.ChunkBatchSize* WorldChunkManager.ChunkBatchSize;
        float odds = 1f / (float)tileCount;
        odds *= quantitiyToSpawn;//(float)quantitiyToSpawn/(float)tileCount;
        float currentOdds = odds;
        int attempts = 0;
       // while (count < quantitiyToSpawn)
        {
            for (int x = 0; x < toGenerateIn.Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < toGenerateIn.Chunks.GetLength(1); y++)
                {
                    chunk = toGenerateIn.Chunks[x, y];
                    for (int x1 = 0; x1 < chunk.ChunkTiles.GetLength(0); x1++)
                    {
                        for (int y1 = 0; y1 < chunk.ChunkTiles.GetLength(1); y1++)
                        {
                            if (chunk.ChunkTiles[x1, y1].CanGeneratePropOnTile())
                            {
                               attempts++;
                                
                               tiles.Add(chunk.ChunkTiles[x1, y1]);
                            }
                            
                        }
                    }
                }
            }
        }



        int index = 0;
        int placeAttempts = 0;
        while(quantitiyToSpawn > 0&&tiles.Count>0&&placeAttempts<tileCount)
        {
            index = Random.Range(0, tiles.Count);
            if (tiles[index].CanGeneratePropOnTile())
            {
                PlaceTree(tiles[index]);
                tiles.RemoveAt(index);
                count++;
                quantitiyToSpawn--;
            }
            placeAttempts++;
           
        }
        Debug.Log("Forest oods " + quantitiyToSpawn + "/" + tileCount + " odds " + odds + " final count " + count + "/" + attempts + " in " + toGenerateIn.coords);


    }

    void PlaceTree(WorldTile tile)
    {
        string objToCreate = ValidObjectsForFeature[Random.Range(0, ValidObjectsForFeature.Count)];
        obj = EnvironmentObjectManager.Instance.AllObjects[objToCreate];

        EnvironmentObjectInstance toAdd = new EnvironmentObjectInstance(Mathf.RoundToInt( tile.WorldPos().x),Mathf.RoundToInt( tile.WorldPos().y), objToCreate);
        WorldChunkManager.Instance.ChunkBatches[tile.Batch].Chunks[tile.Chunk.x,tile.Chunk.y].AddEnvironmentObject(toAdd);
        WorldController.Instance.SetTilesAroundEnvrionmentObjectTraversable(toAdd, !obj.BlocksTile);
       
    }


    Vector2Int batch, chunk, tile;
    List<WorldTile> tiles = new List<WorldTile>();
    EnvironmentObject obj = null;
    bool valid = true;

    public void GenerateTree(Vector2Int coords,string objectToCreate)
    {
        tiles.Clear();
        obj = EnvironmentObjectManager.Instance.AllObjects[objectToCreate];
        valid = true;
        int minX = coords.x;
        int maxX = coords.x + obj.Width;
        int miny = coords.y; 
        int maxY = coords.y+obj.Height;
        for (int x=coords.x-1;x<coords.x+obj.Width+1;x++)
        {
            for(int y = coords.y-1; y < coords.y + obj.Height+1; y++)
            {
                WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(x, y, out batch, out chunk, out tile);
                if (WorldChunkManager.Instance.DoesBatchExist(batch))
                {
                    if (x >= minX && x <= maxX && y >= miny && y <= maxY)
                    {

                        tiles.Add(WorldChunkManager.Instance.ChunkBatches[batch].Chunks[chunk.x, chunk.y].ChunkTiles[tile.x, tile.y]);
                    }
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
