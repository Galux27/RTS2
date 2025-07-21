using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldGenerator : MonoBehaviour
{
    static OverworldGenerator instance; 
    public static OverworldGenerator Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<OverworldGenerator>();
            }
            return instance;
        }
    }



   public int OverworldWidth,OverworldHeight;
    public float MaxElevation, SeaLevel;
   public OverworldTile[,] OverworldTiles;
    public List<OverworldFeatureGenerator> FeatureGenerators;
    public void Generate()
    {
        OverworldTiles=new OverworldTile[OverworldWidth,OverworldHeight];
        for(int x = 0; x < OverworldWidth; x++)
        {
            for(int y = 0; y < OverworldHeight; y++) {
                OverworldTiles[x,y] = new OverworldTile(x,y);
            }
        }
        for(int x = 0; x < FeatureGenerators.Count; x++)
        {
            FeatureGenerators[x].GenerateFeature(ref OverworldTiles);
        }
    }

}

public struct OverworldTile
{
    public int X, Y;
    public float Elevation;
    public OverworldTile(int x,int y,float elevation=0)
    {
        X = x;
        Y = y;
        Elevation = 0;
    }
    
    public void SetElevation(float value)
    {
        Elevation = value;
    }
}
