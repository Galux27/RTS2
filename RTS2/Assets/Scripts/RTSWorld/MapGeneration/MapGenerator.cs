using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        OverworldConverters.Add(OverworldFeature.MinorRoad, new Road(OverworldFeature.MinorRoad, "MinorRoad", 5));
        OverworldConverters.Add(OverworldFeature.MajorRoad, new Road(OverworldFeature.MajorRoad,"MajorRoad",10));


    }

    public List<MapFeatureBase> features = new List<MapFeatureBase>();
    public Dictionary<OverworldFeature,OverworldFeatureToWorldConverter> OverworldConverters;
    public int FeaturesToGenerate;
    public void GenerateMap(WorldChunkBatch toGenerateIn)
    {
        if (toGenerateIn.NeedsGeneration == false)
        {
            return;
        }
        int featureGenerating = 0;


        OverworldTile overworldTile = OverworldGenerator.Instance.GetOverworldTile(toGenerateIn.OverworldCoords);
        toGenerateIn.ApplyOverworldHeight(overworldTile.Elevation);

        if (OverworldGenerator.Instance.SeaLevel < overworldTile.Elevation)
        {
            for(int x=0;x<overworldTile.Features.Count;x++)
            {

                if (OverworldConverters.ContainsKey(overworldTile.Features[x]))
                {
                    OverworldConverters[overworldTile.Features[x]].GenerateFeature(toGenerateIn);
                }
            }


            for (int x = 0; x < FeaturesToGenerate; x++)
            {
                featureGenerating = Random.Range(0, features.Count);
                features[featureGenerating].GenerateFeature(toGenerateIn);
            }
        }
        
        toGenerateIn.NeedsGeneration = false;
        toGenerateIn.SetChunksLoaded();
    }


}
