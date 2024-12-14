using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructableObjectManager : MonoBehaviour
{
    const string FilePath = "ConstructableObjects";

    static ConstructableObjectManager instance;
    public static ConstructableObjectManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance =FindObjectOfType<ConstructableObjectManager>();
            }
            return instance;
        }
    }

    
    private void Awake()
    {
        LoadItemsFromResources();
    }

    public void SetCursorObject(string key)
    {
        if(AllObjects.ContainsKey(key)) {

            if (selectedToConstruct == null || selectedToConstruct != AllObjects[key])
            {
                selectedToConstruct = AllObjects[key];
                GetCursor();
                spriteRenderer.sprite = selectedToConstruct.ForwardsSprite;
            }
        }
    }

    public GameObject GetCursor()
    {
        if (Cursor == null)
        {
            Cursor = new GameObject();
            Cursor.name = "Building Construction Cursor";
            spriteRenderer = Cursor.AddComponent<SpriteRenderer>();
            Cursor.SetActive(false);
        }
        return Cursor;
    }

    public void SetCursorColour(Color colour)
    {
        spriteRenderer.color = colour;
    }


    public ConstructableObject selectedToConstruct;
    GameObject Cursor;
    SpriteRenderer spriteRenderer;



    public Dictionary<string, ConstructableObject> AllObjects;
    List<string> EnvironmentObjectKeys;
    void LoadItemsFromResources()
    {
        AllObjects = new Dictionary<string, ConstructableObject>();
        EnvironmentObjectKeys = new List<string>();
        UnityEngine.Object[] items = Resources.LoadAll(FilePath);
        for (int x = 0; x < items.Length; x++)
        {
            ConstructableObject i = (ConstructableObject)items[x];
            if (AllObjects.ContainsKey(i.Name) == false)
            {
                AllObjects.Add(i.Name, i);
                EnvironmentObjectKeys.Add(i.Name);
            }
        }
    }


    public void CreateBuildableForObject(Vector2Int coords, Vector3 pos)
    {
        string toBuild = selectedToConstruct.Name;
        Action OnBuilt = () => {  CreateObject(coords, pos, toBuild); };
        ConstructableObject buildingData = AllObjects[toBuild];

        Vector2Int chunk = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(pos);
        WorldChunkManager.Instance.Chunks[chunk.x, chunk.y].AddConstructable(new BuildableStructure(coords.x, coords.y, buildingData.TimeToBuild, false, OnBuilt, buildingData.Size(),default,ConstructableType.Furniture));

    }

    public void CreateObject(Vector2Int coords, Vector3 pos, string toConstruct)
    {
        if (AllObjects.ContainsKey(toConstruct) == false)
        {
            return;
        }
        ConstructableObject selectedToConstruct = AllObjects[toConstruct];
        Vector2Int chunk = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(pos);
        WorldChunkManager.Instance.Chunks[chunk.x, chunk.y].AddEnvironmentObject(new ConstructableObjectInstance(coords.x, coords.y, selectedToConstruct.Name));
        for (int x = coords.x; x < coords.x + selectedToConstruct.Width; x++)
        {
            for (int y = coords.y; y < coords.y + selectedToConstruct.Height; y++)
            {
                WorldController.Instance.SetTraversible(x, y, !AllObjects[toConstruct].BlocksTile);
            }
        }


    }
}
