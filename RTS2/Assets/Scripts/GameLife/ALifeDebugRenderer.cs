using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ALifeDebugRenderer : MonoBehaviour
{
    static ALifeDebugRenderer instance;
    public static ALifeDebugRenderer Instance
    {
        get
        {
           
            return instance;
        }
    }
    private void Awake()
    {
        instance = FindObjectOfType<ALifeDebugRenderer>();

        tex = new Texture2D(WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize);

    }
    public RawImage disp;
    public List<Vector2Int> ChunksWithCombat = new List<Vector2Int>();
    public bool UpdateTex = false;
    public int ToUpdate = 0;

    public void AddChunkWithCombat(Vector2Int coords)
    {
        if (!ChunksWithCombat.Contains(coords))
        {
            ChunksWithCombat.Add(coords);
        }
        else
        {
            if (ToUpdate == ChunksWithCombat.IndexOf(coords))
            {
                UpdateTex = true;
            }
        }
    }
    public Vector2Int GetCurCoords()
    {
        if (ToUpdate > ChunksWithCombat.Count - 1)
        {
            return Vector2Int.zero;
        }
        return ChunksWithCombat[ToUpdate];
    }
    Texture2D tex;
    public float[,] HazardMap = null;
    public bool DrawHazard = false;

    public void SetHazardMap(float[,] hazardMap)
    {
        Debug.Log("Set hazard map");
        HazardMap=new float[WorldChunkManager.ChunkBatchSize,WorldChunkManager.ChunkBatchSize];
        for(int x = 0; x < WorldChunkManager.ChunkBatchSize; x++)
        {
            for(int y = 0; y < WorldChunkManager.ChunkBatchSize; y++)
            {
                HazardMap[x, y] = hazardMap[x, y];
            }
        }
        UpdateTex = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (UpdateTex)
        {
            Debug.Log("Has hazard map " + HazardMap == null);
            tex = new Texture2D(WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize);

            int i = 0,j=0;
            for (int x = 0; x < WorldChunkManager.ChunkBatchSize; x++)
            {
                for (int y = 0; y < WorldChunkManager.ChunkBatchSize; y++)
                {
                    tex.SetPixel(x, y, Color.white);
                    if (HazardMap != null&&DrawHazard)
                    {
                        if (HazardMap[x, y] > 0)
                        {
                            i++;
                            tex.SetPixel(x, y, Color.black);
                        }
                        else if (HazardMap[x, y] < 0)
                        {
                            j++;
                        }
                    }
                    
                }
            }

            Debug.Log("hazard map vals " + i + " ," + j);
            Color c = Color.red;
            foreach(KeyValuePair<string,ALifeFactionGroup> kvp in OverworldGenerator.Instance.OverworldTiles[GetCurCoords().x, GetCurCoords().y].ALifeChunk.UnitsInTile)
            {
                if (kvp.Key == FactionController.USER_FACTION) { c = Color.blue; }
                for(int x=0;x<kvp.Value.FactionEntities.Count;x++)
                {
                    tex.SetPixel(kvp.Value.FactionEntities[x].LocalCoords.x, kvp.Value.FactionEntities[x].LocalCoords.y, c);
                }

            }
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            disp.texture = tex;

            UpdateTex = false;
        }
       
    }
}
