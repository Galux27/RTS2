using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;


/// <summary>
/// Class to store references to all potential Environment Objects in the game and the drawing/cleaning up
/// of Environment Objects in the scene based on the camera position
/// </summary>
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
    public List<string> EnvironmentObjectKeys;
    void LoadItemsFromResources()
    {
        AllObjects = new Dictionary<string, EnvironmentObject>();
        EnvironmentObjectKeys=new List<string>();
        Object[] items = Resources.LoadAll(FilePath);
        for (int x = 0; x < items.Length; x++)
        {
            if ((items[x] as EnvironmentObject) != null)
            {
                Debug.Log("Loading env object " + items[x].name);
                EnvironmentObject i = (EnvironmentObject)items[x];
                if (AllObjects.ContainsKey(i.Name) == false)
                {
                    AllObjects.Add(i.Name, i);
                    EnvironmentObjectKeys.Add(i.Name);
                }
            }
        }
    }
    const int ObjectsToGenerate = 500;

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
                objectToCreate = EnvironmentObjectKeys[Random.Range(0, EnvironmentObjectKeys.Count)];
                EnvironmentObjectInstance instance = new EnvironmentObjectInstance(x, y, objectToCreate);
                WorldChunkManager.Instance.Chunks[chunk.x, chunk.y].AddEnvironmentObject(instance);
                WorldController.Instance.SetTilesAroundEnvrionmentObjectTraversable(instance, !EnvironmentObjectHelpers.GetEnvironmentObject(objectToCreate).BlocksTile);

            }
        }
    }

    public void OnDestroyEnvironmentObject(EnvironmentObjectInstance obj)
    {
        EnvironmentObject data = EnvironmentObjectHelpers.GetEnvironmentObject(obj.ObjectKey);

        Vector2Int coords = obj.coords;//WorldController.Instance.ConvertWorldToTileCoords(cursorPos);



        for (int x = coords.x - data.HalfWidth; x < coords.x + data.HalfWidth; x++)
        {
            for (int y = coords.y - data.HalfHeight; y < coords.y + data.HalfHeight; y++)
            {
                WorldController.Instance.SetTraversible(x, y, true);
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
