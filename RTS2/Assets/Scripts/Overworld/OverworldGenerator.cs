using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldGenerator : MonoBehaviour, ISerialize
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
    public Settlement[] Settlements;
    public void Generate()
    {
        EasyStopwatch.StartStopwatch();
        OverworldTiles=new OverworldTile[OverworldWidth,OverworldHeight];
        for(int x = 0; x < OverworldWidth; x++)
        {
            for(int y = 0; y < OverworldHeight; y++) 
            {
                OverworldTiles[x,y] = new OverworldTile(x,y);
            }
        }
        StartCoroutine(GenerateWorld());

    }

    int index = 0;
    IEnumerator GenerateWorld()
    {
        yield return new WaitForSeconds(.1f);
        FeatureGenerators[index].GenerateFeature(OverworldTiles);
        OverworldRenderer.Instance.RenderWorld();
        if(index < FeatureGenerators.Count - 1)
        {
            index++;
            StartCoroutine(GenerateWorld());
        }
        else
        {
            EasyStopwatch.StopStopwatch();
            Debug.Log("Generation took " + EasyStopwatch.GetStopwatchElapsedTime());
        }

    }

    public DataToSerialize GetDataToSerialize()
    {
        DataToSerialize retVal = new DataToSerialize();
        DataToSerialize[,] OverworldTileData = new DataToSerialize[this.OverworldWidth,this.OverworldHeight];
        for(int x = 0; x < OverworldWidth; x++)
        {
            for(int y=0;y < OverworldHeight; y++)
            {
                OverworldTileData[x, y] = OverworldTiles[x, y].GetDataToSerialize();
            }
        }
        retVal.AddDataToSerialize(DataKeys.Overworld, OverworldTileData);
        return retVal;
    }

    public SerializedData Serialize()
    {
        return new SerializedData(GetDataToSerialize());
    }

    public void Deserialize(SerializedData data)
    {
        throw new System.NotImplementedException();
    }

    public UID GetMyUID()
    {
        throw new System.NotImplementedException();
    }

    public void SetMyUID(ulong uid)
    {
        throw new System.NotImplementedException();
    }
}

public class OverworldTile: ISerialize
{
    public int X, Y;
    public float Elevation;
    public List<OverworldFeature> Features = new List<OverworldFeature>();
    public int Population = 0;
    public OverworldPathfindingNode Node;
    public string Settlement = "";

    public OverworldTile(int x,int y,float elevation=0)
    {
        X = x;
        Y = y;
        SetElevation( elevation);
    }
    
    public void SetElevation(float value)
    {
        Elevation = Mathf.Clamp( value,0,OverworldGenerator.Instance.MaxElevation);
    }

    public void AddFeatureToTile(OverworldFeature feature)
    {
        if (!Features.Contains(feature))
        {
            Features.Add(feature);
        }
    }

    public void SetPopulation(Settlement settlement,int pop)
    {
        this.Settlement = settlement.Id.ToString();
        this.Population = pop;
    }

    public void SetNode(OverworldPathfindingNode node)
    {
        Node = node;
    }

    public DataToSerialize GetDataToSerialize()
    {
        DataToSerialize retVal = new DataToSerialize();
        retVal.AddDataToSerialize(DataKeys.Coords, new Vector2Int(X, Y));
        retVal.AddDataToSerialize(DataKeys.OverElevation, Elevation);
        retVal.AddDataToSerialize(DataKeys.OverFeature, Features);
        retVal.AddDataToSerialize(DataKeys.OverPop, Population);
        retVal.AddDataToSerialize(DataKeys.OverSettlement, Settlement);
        return retVal;
    }

  
    public SerializedData Serialize()
    {
        return new SerializedData(GetDataToSerialize());
    }

    public void Deserialize(SerializedData data)
    {
        throw new System.NotImplementedException();
    }

    public UID GetMyUID()
    {
        throw new System.NotImplementedException();
    }

    public void SetMyUID(ulong uid)
    {
        throw new System.NotImplementedException();
    }

}

public enum OverworldFeature
{
    River,
    Settlement,
    MajorRoad,
    MinorRoad,
    Backroad,
    MiscFeature
}
