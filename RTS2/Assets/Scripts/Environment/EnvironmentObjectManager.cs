using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using System;

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
        UnityEngine.Object[] items = Resources.LoadAll(FilePath);
        for (int x = 0; x < items.Length; x++)
        {
            if ((items[x] as EnvironmentObject) != null)
            {
                EnvironmentObject i = (EnvironmentObject)items[x];
                if (AllObjects.ContainsKey(i.Name) == false)
                {
                    AllObjects.Add(i.Name, i);
                    EnvironmentObjectKeys.Add(i.Name);
                }
            }
        }
    }

    public static Action<EnvironmentObjectInstance> OnEnvironmentObjectDestroyed,OnEnvironmentObjectCreated;

    public void OnDestroyEnvironmentObject(EnvironmentObjectInstance obj)
    {
        EnvironmentObject data = EnvironmentObjectHelpers.GetEnvironmentObject(obj.ObjectKey);

        Vector2Int coords = obj.coords;//WorldController.Instance.ConvertWorldToTileCoords(cursorPos);

        OnEnvironmentObjectDestroyed?.Invoke(obj);

        for (int x = coords.x; x < coords.x + data.GetWidth+1; x++)
        {
            for (int y = coords.y; y < coords.y + data.GetHeight+1; y++)
            {
                WorldController.Instance.SetTraversible(x, y, true,WorldTileContents.EnvObject);
            }
        }
    }
}
