using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
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

    public void OverrideCursor(Sprite toOverrideWith)
    {
        GetCursor();
        spriteRenderer.sprite = toOverrideWith;
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
            this.Cursor.AddComponent<SortingOrderController>();
            Cursor.SetActive(false);
        }
        return Cursor;
    }

    public void SetCursorPosition(Vector3 pos)
    {
        if (Cursor == null)
        {
            GetCursor();
        }
       // Vector2Int chunk = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(pos);
       // Cursor.transform.position = new Vector3(chunk.x, chunk.y);


        Vector3 cursorPos = pos;

        ConstructableObjectManager.Instance.GetCursor().SetActive(true);
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        float height = ConstructableObjectManager.Instance.selectedToConstruct.GetHeight;
        float width = ConstructableObjectManager.Instance.selectedToConstruct.GetWidth;


        Cursor.transform.position = new Vector3(coords.x + (width / 2f), coords.y + (height / 2f), 0f);


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


    public void CreateBuildableForObject(Vector2Int coords, Vector3 pos,Dictionary<string,List<FoundResourceData>> resourcesToConsume)
    {
        string toBuild = selectedToConstruct.Name;


        Action OnBuilt = GetActionForConstructableOnBuilt(coords, pos, toBuild);
        ConstructableObject buildingData = GetData(toBuild);

        Vector2Int chunk = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(pos);
        new BuildableStructure(coords.x, coords.y, buildingData.TimeToBuild, false, OnBuilt, buildingData.Size(),default,ConstructableType.Furniture,buildingData.Name);
       
        
        ResourceHelpers.ConsumeResources(selectedToConstruct.RequirementsToBuild);
    }
    
    public ConstructableObject GetData(string key)
    {
        return AllObjects[key];
    }
    
    public Action GetActionForConstructableOnBuilt(Vector2Int coords, Vector3 pos, string toBuild)
    {
        return () => { CreateObject(coords, pos, toBuild); };
    }

    public void CreateObject(Vector2Int coords, Vector3 pos, string toConstruct)
    {
        if (AllObjects.ContainsKey(toConstruct) == false)
        {
            return;
        }
        ConstructableObject selectedToConstruct = AllObjects[toConstruct];
        ConstructableObjectInstance instance = new ConstructableObjectInstance(coords.x, coords.y, selectedToConstruct.Name);
        WorldChunkManager.Instance.AddEnvironmentObjectInstanceToChunk(instance);
        WorldController.Instance.SetTilesAroundEnvrionmentObjectTraversable(instance, !AllObjects[toConstruct].BlocksTile);
    }
}
