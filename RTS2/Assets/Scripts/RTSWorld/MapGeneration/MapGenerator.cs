using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    static MapGenerator instance;
    public static MapGenerator Instance {  
        get 
        { 
            if(instance == null)
            {
                instance=FindObjectOfType<MapGenerator>(true);
                instance.Init();
            }
            return instance; 
        } 
    }

    public void Init()
    {
        OverworldConverters = new Dictionary<OverworldFeature, OverworldFeatureToWorldConverter>();
        OverworldConverters.Add(OverworldFeature.Backroad, new Road(OverworldFeature.Backroad, "BackRoad", 2));
        OverworldConverters.Add(OverworldFeature.MinorRoad, new Road(OverworldFeature.MinorRoad, "MinorRoad", 7));
        OverworldConverters.Add(OverworldFeature.MajorRoad, new Road(OverworldFeature.MajorRoad,"MajorRoad",31));
        OverworldConverters.Add(OverworldFeature.River, new FeatureRiver(OverworldFeature.River, "River", 5));

    }
    public List<FeatureMapGenerator> Features = new List<FeatureMapGenerator>();
    public FeatureMapGenerator DefaultFeatures;
    public Dictionary<OverworldFeature,OverworldFeatureToWorldConverter> OverworldConverters;
    public void GenerateMap(WorldChunkBatch toGenerateIn)
    {
        if (toGenerateIn.NeedsGeneration == false)
        {
            return;
        }
        OverworldTile overworldTile = OverworldGenerator.Instance.GetOverworldTile(toGenerateIn.OverworldCoords);
        Debug.Log("Generating: render call" + toGenerateIn.coords);
        for (int x = 0; x < overworldTile.Features.Count; x++)
        {

            if (OverworldConverters.ContainsKey(overworldTile.Features[x]))
            {
                OverworldConverters[overworldTile.Features[x]].GenerateFeature(toGenerateIn);
            }
        }
        if (!overworldTile.Features.Contains(OverworldFeature.LargeWaterBody))
        {
            

            bool generated = false;
            TryToGenerateByFeature(toGenerateIn, out generated);
            bool wasGenerated = generated;
            if (!generated)
            {
                DefaultFeatures.GenerateForFeature(toGenerateIn,DefaultFeatures.features);
            }

         
            if (!wasGenerated)
            {
                DefaultFeatures.GenerateForFeature(toGenerateIn, DefaultFeatures.UnnaturalFeatures,false);

            }
        }
        else
        {
            toGenerateIn.ApplyOverworldHeight(overworldTile.Elevation);
            toGenerateIn.GenerateWorldTileBlends();
            toGenerateIn.ApplyOverworldHeight(0);

        }
        toGenerateIn.GenerateRoadBlends(RoadType.Backroad);
        toGenerateIn.GenerateRoadBlends(RoadType.MinorRoad);
        toGenerateIn.GenerateRoadBlends(RoadType.MajorRoad);

        toGenerateIn.GenerateRoads(RoadType.Backroad);
        toGenerateIn.GenerateRoads(RoadType.MinorRoad);
        toGenerateIn.GenerateRoads(RoadType.MajorRoad);


        toGenerateIn.NeedsGeneration = false;
        toGenerateIn.SetChunksLoaded();
    }

    public void TryToGenerateByFeature(WorldChunkBatch toGenerateIn,out bool generated)
    {
        generated = false;
        OverworldTile overworldTile = OverworldGenerator.Instance.GetOverworldTile(toGenerateIn.OverworldCoords);

        for (int x = 0; x < Features.Count; x++)
        {
            if (overworldTile.Features.Contains(Features[x].toGenerateFor))
            {
                Features[x].GenerateForFeature(toGenerateIn, Features[x].features);
                generated = true;
            }
        }
    }

}
[System.Serializable]
public class FeatureMapGenerator
{
    public OverworldFeature toGenerateFor;
    public List<MapFeatureBase> features = new List<MapFeatureBase>(),UnnaturalFeatures=new List<MapFeatureBase>();
    public int FeaturesToGenerate;
    public FloorTileGenerator floorTileGenerator;
    public bool RefreshElevationOnGenerate = false;
    public void GenerateForFeature(WorldChunkBatch toGenerateIn,List<MapFeatureBase> features,bool useFloorGenerator=true)
    {



        OverworldTile overworldTile = OverworldGenerator.Instance.GetOverworldTile(toGenerateIn.OverworldCoords);
        toGenerateIn.ApplyOverworldHeight(overworldTile.Elevation);

        if (floorTileGenerator.Use && useFloorGenerator)
        {
            floorTileGenerator.GenerateTiles(toGenerateIn);
        }

        int featureGenerating = 0;
        if (OverworldGenerator.Instance.SeaLevel < overworldTile.Elevation)
        {
            for (int x = 0; x < FeaturesToGenerate; x++)
            {
                featureGenerating = Random.Range(0, features.Count);
                features[featureGenerating].GenerateFeature(toGenerateIn);
            }
        }
        if (RefreshElevationOnGenerate)
        {
            toGenerateIn.RefreshElevationTiles();

        }
        toGenerateIn.GenerateWorldTileBlends();

    }
}

[System.Serializable]
public class FloorTileGenerator
{
    public string TileToSet;
    public bool Use = true;
    public virtual void GenerateTiles(WorldChunkBatch toSetIn)
    {
        WorldChunk setTilesIn = null;
        uint id = WorldRenderer.Instance.WorldTilesManager.GetTileID(TileToSet);
        for (int x = 0; x < toSetIn.Chunks.GetLength(0); x++)
        {
            for(int y = 0; y < toSetIn.Chunks.GetLength(1); y++)
            {
                setTilesIn = toSetIn.Chunks[x, y];
                for(int x1=0;x1<setTilesIn.ChunkTiles.GetLength(0); x1++)
                {
                    for (int y1 = 0; y1 < setTilesIn.ChunkTiles.GetLength(1); y1++)
                    {
                        setTilesIn.UpdateTile(x1, y1, TileToSet,id);
                    }
                }
            }
        }
    }
}
