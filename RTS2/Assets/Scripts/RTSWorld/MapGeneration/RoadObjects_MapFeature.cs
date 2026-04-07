using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Feature", menuName = "Map Features/RoadObjects", order = 1)]
public class RoadObjects_MapFeature : MapFeatureBase
{

    public int MinToGenerate=0,MaxToGenerate=10;
    public List<string> ValidObjectsForFeature;
    List<RoadData> ValidRoads;

    void GetValidRoads(WorldChunkBatch toGenerateIn)
    {
        ValidRoads = new List<RoadData>();
        for(int x=0;x<toGenerateIn.Roads.Count;x++)
        {
            //if (toGenerateIn.Roads[x].IsBlend() == false)
            {
                ValidRoads.Add(toGenerateIn.Roads[x]);
            }
        }
    }

    public override void GenerateFeature(WorldChunkBatch toGenerateIn)
    {
        GetValidRoads(toGenerateIn);
        if (ValidRoads.Count == 0)
        {
            return;
        }
        int countToGenerate = Random.Range(0, MaxToGenerate);

        for (int i = 0; i < countToGenerate; i++)
        {
            RoadData road = GetRandomRoad(toGenerateIn);
            if (road.Type == RoadType.MajorRoad || road.Type == RoadType.MinorRoad)
            {
                //RoadSegment rs = GetRandomRoadSegment(road);
                float point = Random.Range(0.15f, .85f);
                Vector2 posForObj = Vector2.Lerp(road.StartPos, road.EndPos, point);
                string objID = GetRandomObjectToSpawn();
                EnvironmentObject obj = EnvironmentObjectManager.Instance.AllObjects[objID];
                WorldTile toCheck = null;
                Vector3 size = obj.Size();
                bool isValid = true;
                Vector2 offset = road.EndPos - road.StartPos;
                offset = offset.normalized * (road.Width/2f);
                offset *= Random.Range(0f, 1f);
                float val = offset.x;
                offset.x = offset.y;
                offset.y = val;

                //offset = Vector2.Perpendicular(offset) 
                posForObj += offset;
                WorldTile toPutOn = Pathfinding.GetTileFromPosition(new Vector3(posForObj.x, posForObj.y));

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
                    Debug.Log("Generating road objects, creating road ojbects "+ obj.name + " at "+ posForObj+" offset " + offset+"Road points "
                        + road .StartPos+ ","+ road.EndPos+ " chunk batch "+ toGenerateIn.coords+","+toPutOn.Coords()+","+toPutOn.Batch+","+road.IsGenerated+","+road.IsGenerated);

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

    RoadData GetRandomRoad(WorldChunkBatch batch)
    {
        return ValidRoads[Random.Range(0, ValidRoads.Count)];
    }

    RoadSegment GetRandomRoadSegment(BatchRoad br)
    {
        return br.Segments[Random.Range(0,br.Segments.Count)];
    }



}
