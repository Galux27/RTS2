using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for managing all items in the game and all instances of items in the world
/// </summary>
public class ItemController : MonoBehaviour
{
    const string ItemLocation = "Items";
    static ItemController instance;
    public static ItemController Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<ItemController>();
            }
            return instance;
        }
    }

    public Dictionary<string, Item> AllItems;

    private void Awake()
    {
        LoadItemsFromResources();
    }

    void LoadItemsFromResources()
    {
        AllItems = new Dictionary<string, Item>();
        Object[] items = Resources.LoadAll(ItemLocation);
        for(int x=0;x< items.Length;x++)
        {
            Item i = (Item)items[x];
            if (AllItems.ContainsKey(i.Name) == false)
            {
                AllItems.Add(i.Name, i);
            }
        }
    }

    public List<ItemInWorld> AllItemsInWorld = new List<ItemInWorld>();
}
