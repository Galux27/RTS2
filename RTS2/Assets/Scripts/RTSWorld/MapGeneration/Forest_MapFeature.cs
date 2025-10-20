using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Map Feature", menuName = "Map Features/Forest", order = 1)]
public class Forest_MapFeature :MapFeatureBase
{
    public List<string> ValidObjectsForFeature;
    public int MinQuantityToSpawn,MaxQuantityToSpawn;
    public override void GenerateFeature(WorldChunkBatch toGenerateIn)
    {
        Vector2Int center = toGenerateIn.coords + new Vector2Int(Random.Range(0, WorldChunkManager.ChunkBatchSize), Random.Range(0, WorldChunkManager.ChunkBatchSize));
        int width = Random.Range(MinWidth, MaxWidth);
        int height = Random.Range(MinHeight, MaxHeight);
        int x = 0, y = 0;
        int quantitiyToSpawn = Random.Range(MinQuantityToSpawn, MaxQuantityToSpawn);
        int objectToCreate = 0;
        WorldTile tileChecking = null;
        WorldChunk tileChunk = null;
        Vector2Int chunkCoords = new Vector2Int(0, 0);
        EnvironmentObjectInstance toAdd = null, existing = null;
        for (int q = 0; q < quantitiyToSpawn; q++)
        {
            x = Random.Range(center.x - width, center.x + width);
            y = Random.Range(center.y - height, center.y + height);
            tileChecking = WorldTileHelpers.GetTileFromCoords(x, y);
            if (tileChecking != null && tileChecking.traversable && tileChecking.WaterData.WaterLevel == 0f &&tileChecking.CanPutDecorationsOn)
            {
                objectToCreate = Random.Range(0, ValidObjectsForFeature.Count);
                chunkCoords = tileChecking.Chunk; //WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(new Vector3(x, y));
                tileChunk = toGenerateIn.Chunks[chunkCoords.x, chunkCoords.y];
                //tileChunk.DoesAnyObjectExistAtCoords(new Vector2Int(x, y), out existing) == false
                //if ()
                {
                    toAdd = new EnvironmentObjectInstance(x, y, ValidObjectsForFeature[objectToCreate]);
                    tileChunk.AddEnvironmentObject(toAdd);
                    WorldController.Instance.SetTilesAroundEnvrionmentObjectTraversable(toAdd, !EnvironmentObjectHelpers.GetEnvironmentObject(ValidObjectsForFeature[objectToCreate]).BlocksTile);
                    tileChecking.CanPutDecorationsOn = false;
                }
            }
        }

    }
}
