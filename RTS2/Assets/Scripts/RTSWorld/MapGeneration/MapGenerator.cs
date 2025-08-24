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
            }
            return instance; 
        } 
    }
    public List<MapFeatureBase> features = new List<MapFeatureBase>();

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
