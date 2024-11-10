using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentObjectManager : MonoBehaviour
{
    static EnvironmentObjectManager instance;
    public static EnvironmentObjectManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindAnyObjectByType<EnvironmentObjectManager>();
            }
            return instance;
        }
    }
    const string FilePath = "EnvironmentObjects";

    private void Awake()
    {
        LoadItemsFromResources();
    }

    public Dictionary<string, EnvironmentObject> AllObjects;
    List<string> EnvironmentObjectKeys;
    void LoadItemsFromResources()
    {
        AllObjects = new Dictionary<string, EnvironmentObject>();
        EnvironmentObjectKeys=new List<string>();
        Object[] items = Resources.LoadAll(FilePath);
        for (int x = 0; x < items.Length; x++)
        {
            EnvironmentObject i = (EnvironmentObject)items[x];
            if (AllObjects.ContainsKey(i.Name) == false)
            {
                AllObjects.Add(i.Name, i);
                EnvironmentObjectKeys.Add(i.Name);
            }
        }
    }
    const int ObjectsToGenerate = 5000;

    public void GenerateEnvironmentObjects()
    {
        Vector3 posCache = Vector3.zero;
        Vector2Int chunk = Vector2Int.zero;
        string objectToCreate = "";
        for (int q=0; q < ObjectsToGenerate; q++)
        {
            int x = Random.Range(0, WorldController.Instance.WorldWidth-1);
            int y = Random.Range(0,WorldController.Instance.WorldHeight-1);

            if (WorldController.Instance.WorldTiles[x, y].traversable)
            {
                posCache.x = x;
                posCache.y = y;
                chunk = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(posCache);
                objectToCreate = EnvironmentObjectKeys[Random.Range(0, EnvironmentObjectKeys.Count-1)];
                WorldChunkManager.Instance.Chunks[chunk.x, chunk.y].AddEnvironmentObject(new EnvironmentObjectInstance(x, y,objectToCreate ));
                WorldController.Instance.SetTraversible(x, y, !AllObjects[objectToCreate].BlocksTile);
            }
        }
    }

    const float DrawEnvironmentObjectRadius = 20f;
    List<WorldChunk> UpdatedLastFrame = new List<WorldChunk>();
    private void Update()
    {
        Vector3 CameraPos = Camera.main.transform.position;
        Vector2Int pos = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(CameraPos);

        List<WorldChunk> Chunks = WorldChunkManager.Instance.GetChunksInRadius(DrawEnvironmentObjectRadius, CameraPos);

        for(int x = 0; x < Chunks.Count; x++)
        {
            if (Chunks[x].ShouldDrawEnvironmentObjects())
            {
                if (Chunks[x].DrawnEnvironmentObjects() == false)
                {
                    Chunks[x].RenderEnvironmentObjects() ;
                }
            }
        }

        for(int x = 0; x < UpdatedLastFrame.Count; x++)
        {
            if (Chunks.Contains(UpdatedLastFrame[x])==false) {
                UpdatedLastFrame[x].CleanupEnvironmentObjects();
            }
        }
        UpdatedLastFrame = Chunks;

    }

}
