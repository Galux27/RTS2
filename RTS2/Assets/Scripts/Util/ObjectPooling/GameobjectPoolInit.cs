using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameobjectPoolInit : MonoBehaviour
{
    public List<PoolToInit> PoolsToInit;
    private void Awake()
    {
        for(int x = 0; x < PoolsToInit.Count; x++)
        {
            GameObjectPoolManager.Instance.AddPool(PoolsToInit[x].Key, PoolsToInit[x].Object, PoolsToInit[x].InitCount);
        }
    }
}

[System.Serializable]
public struct PoolToInit
{
    public string Key;
    public int InitCount;
    public GameObject Object;
}
