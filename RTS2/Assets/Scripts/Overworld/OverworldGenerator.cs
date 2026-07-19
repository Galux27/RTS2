using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                instance.Init();
            }
            return instance;
        }
    }
    void Init()
    {
        ALifeSystem = new ALife();
    }
    public ALife ALifeSystem;

    public OverworldTile GetOverworldTile(Vector2Int coords)
    {
        coords.x = Mathf.Clamp(coords.x, 0, OverworldWidth);
        coords.y=Mathf.Clamp(coords.y, 0, OverworldHeight);
        return OverworldTiles[coords.x, coords.y];
    }

    public bool DoTilesAllContainFeature(List<OverworldTile> tiles, OverworldFeature feature)
    {
        for (int x = 0; x < tiles.Count; x++)
        {
            if (!tiles[x].Features.Contains(feature))
            {
                return false;
            }
        }
        return true;
    }

     Vector2Int OverworldStartingCoords;
    bool hasSetOverworldStartingCoords = false;
   public int OverworldWidth,OverworldHeight;
    public float MaxElevation, SeaLevel;
   public OverworldTile[,] OverworldTiles;
    public List<OverworldFeatureGenerator> FeatureGenerators;
    public OverworldSettlement[] Settlements;

    public OverworldSettlement GetSettlementByID(int id)
    {
        for (int x = 0; x < Settlements.Length; x++)
        {
            if (Settlements[x].Id == id)
            {
                return Settlements[x];
            }
        }
        return null;
    }


    public OverworldSettlement GetSettlementByID(uint id)
    {
        for(int x = 0; x < Settlements.Length; x++)
        {
            if (Settlements[x].Id == id)
            {
                return Settlements[x];
            }
        }
        return null;
    }
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

    void OnGenerationDone()
    {
        EasyStopwatch.StartStopwatch();
        GameLifeManager.Instance.OnNewGameStarted();
        for (int x = 0; x < OverworldWidth; x++)
        {
            for (int y = 0; y < OverworldHeight; y++)
            {
                ALifeSystem.GenerateEntitiesForOverworldTile(OverworldTiles[x, y]);
             
            }
        }
        EasyStopwatch.StopStopwatch();
        Debug.Log("Z Generation took " + EasyStopwatch.GetStopwatchElapsedTime()+ " total zomz "+ALifeSystem.zombieCount);
    }

    private void Update()
    {
        ALifeSystem.Update();
    }

    public List<OverworldTile> GetNeighbours(Vector2Int coords,bool getDiagonal=false)
    {
        List<OverworldTile> retVal = new List<OverworldTile>();

        if (coords.x == 0) 
        {
            retVal.Add(OverworldTiles[coords.x+1, coords.y]);

            if (getDiagonal)
            {
                if (coords.y == 0)
                {
                    retVal.Add(OverworldTiles[coords.x + 1, coords.y + 1]);
                }
                else if (coords.y >= OverworldHeight - 1)
                {
                    retVal.Add(OverworldTiles[coords.x + 1, coords.y - 1]);
                }
                else
                {
                    retVal.Add(OverworldTiles[coords.x + 1, coords.y + 1]);
                    retVal.Add(OverworldTiles[coords.x + 1, coords.y - 1]);
                }
            }
        }
        else if (coords.x >= OverworldWidth - 1)
        {
            retVal.Add(OverworldTiles[coords.x-1, coords.y]);
            if (getDiagonal)
            {
                if (coords.y == 0)
                {
                    retVal.Add(OverworldTiles[coords.x - 1, coords.y + 1]);
                }
                else if (coords.y >= OverworldHeight - 1)
                {
                    retVal.Add(OverworldTiles[coords.x - 1, coords.y - 1]);
                }
                else
                {
                    retVal.Add(OverworldTiles[coords.x - 1, coords.y + 1]);
                    retVal.Add(OverworldTiles[coords.x - 1, coords.y - 1]);
                }
            }
        }
        else
        {
            retVal.Add(OverworldTiles[coords.x + 1, coords.y]);
            retVal.Add(OverworldTiles[coords.x - 1, coords.y]);
            if (getDiagonal)
            {
                if (coords.y == 0)
                {
                    retVal.Add(OverworldTiles[coords.x + 1, coords.y + 1]);
                    retVal.Add(OverworldTiles[coords.x - 1, coords.y + 1]);

                }
                else if (coords.y >= OverworldHeight - 1)
                {
                    retVal.Add(OverworldTiles[coords.x+1, coords.y - 1]);
                    retVal.Add(OverworldTiles[coords.x - 1, coords.y - 1]);

                }
                else
                {
                    retVal.Add(OverworldTiles[coords.x+1, coords.y + 1]);
                    retVal.Add(OverworldTiles[coords.x+1, coords.y - 1]);
                    retVal.Add(OverworldTiles[coords.x - 1, coords.y + 1]);
                    retVal.Add(OverworldTiles[coords.x - 1, coords.y - 1]);
                }
            }
        }

        if (coords.y == 0)
        {
            retVal.Add(OverworldTiles[coords.x , coords.y + 1]);
        }
        else if (coords.y >= OverworldHeight- 1)
        {
            retVal.Add(OverworldTiles[coords.x , coords.y - 1]);
        }
        else
        {
            retVal.Add(OverworldTiles[coords.x , coords.y + 1]);
            retVal.Add(OverworldTiles[coords.x , coords.y - 1]);
        }
        return retVal;
    }
    public void SetOverworldStartingCoords(Vector2Int coords)
    {
        hasSetOverworldStartingCoords = true;
        OverworldStartingCoords = coords;
    }

    public Vector2Int WorldCenter;
   
    public Vector2Int GetOverworldStartingCoords()
    {
        if (!hasSetOverworldStartingCoords)
        {
            OverworldStartingCoords = new Vector2Int(50, 50);

            List<OverworldTile> neighbours;
            Vector2Int coords=new Vector2Int();    
            for(int x=10;x< OverworldWidth-10; x++)
            {
                for (int y = 10; y < OverworldHeight - 10; y++)
                {
                    coords = new Vector2Int(x, y);

                   // if (OverworldTiles[x, y].Elevation< SeaLevel+25 && OverworldTiles[x, y].Elevation>SeaLevel)
                    {
                        int count = 0;
                        neighbours = GetNeighbours(coords);
                        for (int i = 0; i < neighbours.Count; i++)
                        {
                            if (neighbours[i].Features.Contains(OverworldFeature.Settlement) )
                            {
                                count++;
                                //hasSetOverworldStartingCoords = true;

                                //OverworldStartingCoords = new Vector2Int(x, y);
                                //Debug.Log("Set overworld start coords to " + OverworldStartingCoords);
                                //return OverworldStartingCoords;

                            }
                        }
                        if (count>2)
                        {
                            hasSetOverworldStartingCoords = true;

                            OverworldStartingCoords = new Vector2Int(x, y);
                            Debug.Log("Set overworld starting coords " + OverworldStartingCoords);
                            break;
                        }
                        else
                        {
                            Debug.Log("Set overworld starting coords false " + count+"/"+neighbours.Count);
                        }
                    }
                  
                    if (hasSetOverworldStartingCoords)
                    {
                        break;
                    }
                }
            }
            WorldCenter = OverworldStartingCoords * WorldChunkManager.ChunkBatchSize;
            // OverworldStartingCoords = new Vector2Int(Mathf.RoundToInt(Random.Range(OverworldWidth * .1f, OverworldWidth * .9f)), Mathf.RoundToInt(Random.Range(OverworldWidth * .1f, OverworldWidth * .9f)));
            //hasSetOverworldStartingCoords = true;
            CameraController.Instance.SetInitialPosition(WorldCenter);
        }
        return Vector2Int.zero ;
    }


    public void GenerateWithoutCoroutine()
    {
        EasyStopwatch.StartStopwatch();
        OverworldTiles = new OverworldTile[OverworldWidth, OverworldHeight];
        for (int x = 0; x < OverworldWidth; x++)
        {
            for (int y = 0; y < OverworldHeight; y++)
            {
                OverworldTiles[x, y] = new OverworldTile(x, y);
            }
        }
        for (int x=0;x < FeatureGenerators.Count; x++)
        {
            FeatureGenerators[x].GenerateFeature(OverworldTiles);

        }
        OverworldRenderer.Instance.RenderWorld();
        Debug.Log("Full Generation took " + EasyStopwatch.GetStopwatchElapsedTime());
        OnGenerationDone();
    }


    int index = 0;
    IEnumerator GenerateWorld()
    {
       
            yield return new WaitForSeconds(.1f);
        try
        {
            FeatureGenerators[index].GenerateFeature(OverworldTiles);
            OverworldRenderer.Instance.RenderWorld();
            if (index < FeatureGenerators.Count)
            {
                index++;
                StartCoroutine(GenerateWorld());
            }
            else
            {
                EasyStopwatch.StopStopwatch();
                Debug.Log("Generation took " + EasyStopwatch.GetStopwatchElapsedTime());
                OnGenerationDone();

            }
        }catch(System.Exception e)
        {
            Debug.LogError(e.ToString());
            Debug.Break();
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
[System.Serializable]
public class OverworldTile: ISerialize
{
    public int X, Y;
    Vector2Int coords;
    public float Elevation;
    public List<OverworldFeature> Features = new List<OverworldFeature>();
    
    public int Population = 0;
    public OverworldPathfindingNode Node;
    public string Settlement = "";
    public OverworldSettlementDetails SettlementDetails;

    public OverworldTile(int x,int y,float elevation=0)
    {
        X = x;
        Y = y;
        coords= new Vector2Int(x,y);
        ALifeChunk = new ALifeChunk(coords);
        SetElevation( elevation);
    }
    
    public void SetElevation(float value)
    {
        Elevation = Mathf.Clamp( value,0,OverworldGenerator.Instance.MaxElevation);
        if (Elevation < OverworldGenerator.Instance.SeaLevel)
        {
            if (Features.Contains(OverworldFeature.LargeWaterBody) == false)
            {
                AddFeatureToTile(OverworldFeature.LargeWaterBody);
            }
        }
        else
        {
            if (Features.Contains(OverworldFeature.LargeWaterBody) )
            {
                Features.Remove(OverworldFeature.LargeWaterBody);
            }

            if(Elevation> OverworldGenerator.Instance.MaxElevation * .8f)
            {
                Features.Add(OverworldFeature.Mountain);
            }
        }
    }

    public Vector2Int RiverPoint;

    public void SetRiverPoint(int index,int length)
    {
        RiverPoint = new Vector2Int(index, length);
    }
    public void SetSettlementID(int id)
    {
        SettlementDetails = new OverworldSettlementDetails(id);
    }

    public void AddFeatureToTile(OverworldFeature feature)
    {
        if (feature == OverworldFeature.LargeWaterBody)
        {
            if(Elevation>OverworldGenerator.Instance.SeaLevel)
            {
                SetElevation(OverworldGenerator.Instance.SeaLevel - 1);
            }
        }

        if (!Features.Contains(feature))
        {
            Features.Add(feature);
        }
    }

    public void SetPopulation(OverworldSettlement settlement,int pop)
    {
        SetSettlementID(settlement.Id);
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
        retVal.AddDataToSerialize(DataKeys.OverRiverCoords, RiverPoint);
        return retVal;
    }

    public int GetQuantitiyOfFeature(OverworldFeature feature)
    {
        int retVal = 0;
        for(int x = 0; x < Features.Count; x++)
        {
            if (Features[x] == feature)
            {
                retVal++;
            }
        }
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

    public ALifeChunk ALifeChunk;
    public Dictionary<string, ALifeFactionGroup> UnitsInTile
    {
        get
        {
            return ALifeChunk.UnitsInTile;
        }
    }

    public void AddALifeEntity(ALifeEntity entity,bool CheckForExisting=true)
    {
       ALifeChunk.AddALifeEntity(entity,CheckForExisting);
    }

    public void RemoveALifeEntity(ALifeEntity entity)
    {
        ALifeChunk.RemoveALifeEntity(entity);
    }


}


public class OverworldSettlementDetails
{
    static int BaseID = 1;
    public int ID;

    public OverworldSettlementDetails(int id)
    {
        this.ID= id;
    }
    
}
public enum OverworldFeature
{
    River,
    Settlement,
    MajorRoad,
    MinorRoad,
    Backroad,
    MiscFeature,
    LargeWaterBody,
    Mountain
}
