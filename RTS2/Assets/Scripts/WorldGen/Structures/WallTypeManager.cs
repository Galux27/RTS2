using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTypeManager : MonoBehaviour
{
    static WallTypeManager instance;
    public static WallTypeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<WallTypeManager>();
            }
            return instance;
        }
    }
    const string FilePath = "Walls";
    public Sprite WallIcon;
    private void Awake()
    {
        LoadItemsFromResources();
    }

    public Dictionary<string, WallTile> AllObjects;
    public List<string> WallTileObjectKeys;
    public WallTile SelectedWallTile;
    void LoadItemsFromResources()
    {
        AllObjects = new Dictionary<string, WallTile>();
        WallTileObjectKeys = new List<string>();
        Object[] items = Resources.LoadAll(FilePath);
        for (int x = 0; x < items.Length; x++)
        {
            if ((items[x] as WallTile) != null)
            {
                
                Debug.Log("Loading env object " + items[x].name);
                WallTile i = (WallTile)items[x];
                if (AllObjects.ContainsKey(i.WallName) == false)
                {
                    AllObjects.Add(i.WallName, i);
                    WallTileObjectKeys.Add(i.WallName);
                    if (SelectedWallTile == null)
                    {
                        SelectedWallTile = i;
                    }
                }
            }
        }
    }
}
