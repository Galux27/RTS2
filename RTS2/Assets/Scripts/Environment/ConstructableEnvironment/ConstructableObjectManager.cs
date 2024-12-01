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

    public Dictionary<string, ConstructableObject> AllObjects;
    List<string> EnvironmentObjectKeys;
    void LoadItemsFromResources()
    {
        AllObjects = new Dictionary<string, ConstructableObject>();
        EnvironmentObjectKeys = new List<string>();
        Object[] items = Resources.LoadAll(FilePath);
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
}
