using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Feature", menuName = "Map Features/RoadObjects", order = 1)]
public class RoadObjects_MapFeature : MapFeatureBase
{

    public int MinToGenerate=0,MaxToGenerate=10;
    public List<string> ValidObjectsForFeature;


    public override void GenerateFeature(WorldChunkBatch toGenerateIn)
    {
        Debug.Log("Generating road objects, road count " + toGenerateIn.Roads.Count);
        if (toGenerateIn.Roads.Count == 0)
        {
            return;
        }
        int countToGenerate = Random.Range(0, MaxToGenerate);

        for (int i = 0; i < countToGenerate; i++)
        {
            BatchRoad road = GetRandomRoad(toGenerateIn);
            if (road.type == RoadType.MajorRoad || road.type == RoadType.MinorRoad)
            {
                RoadSegment rs = GetRandomRoadSegment(road);
                float point = Random.Range(0f, 1f);
                Vector2 posForObj = Vector2.Lerp(rs.Start, rs.End, point);
                WorldTile toPutOn = Pathfinding.GetTileFromPosition(new Vector3(posForObj.x, posForObj.y));
                string objID = GetRandomObjectToSpawn();
                EnvironmentObject obj = EnvironmentObjectManager.Instance.AllObjects[objID];
                WorldTile toCheck = null;
                Vector3 size = obj.Size();
                bool isValid = true;
                for (float x = posForObj.x; x < posForObj.x + (size.x); x++)
                {
                    for (float y = posForObj.y; y < posForObj.y + (size.y); y++)
                    {
                        toCheck = Pathfinding.GetTileFromPosition(new Vector3(x, y, 0));
                        if (toCheck == null || toCheck.TileTraversable() == false)
                        {
                            isValid = false;
                            break;
                        }
                    }
                    if (!isValid)
                    {
                        break;
                    }
                }
                if (isValid)
                {
                    Debug.Log("Generating road objects, creating road ojbects "+ obj.name + " at "+ posForObj);

                    EnvironmentObjectInstance toAdd = new EnvironmentObjectInstance(toPutOn.x,toPutOn.y,objID);
                    WorldChunkManager.Instance.ChunkBatches[toPutOn.Batch].Chunks[toPutOn.Chunk.x, toPutOn.Chunk.y].AddEnvironmentObject(toAdd);
                    WorldController.Instance.SetTilesAroundEnvrionmentObjectTraversable(toAdd, obj.BlocksTile==false);
                    toPutOn.CanPutDecorationsOn = false;
                }

            }
        }
    }

        string GetRandomObjectToSpawn()
    {
        return ValidObjectsForFeature[Random.Range(0, ValidObjectsForFeature.Count)];
    }

    BatchRoad GetRandomRoad(WorldChunkBatch batch)
    {
        return batch.Roads[Random.Range(0, batch.Roads.Count)];
    }

    RoadSegment GetRandomRoadSegment(BatchRoad br)
    {
        return br.Segments[Random.Range(0,br.Segments.Count)];
    }



}
