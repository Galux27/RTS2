using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPoolManager : MonoBehaviour
{
    static GameObjectPoolManager instance;
    public static GameObjectPoolManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<GameObjectPoolManager>();
            }
            return instance;
        }
    }

    public Dictionary<string,GameObjectPool> AllPools=new Dictionary<string,GameObjectPool>();


    public void AddPool(string name,GameObject prefab,int initCount)
    {
        AllPools.Add(name,new GameObjectPool(prefab,initCount));
    }

    public GameObject GetObjectFromPool(string name)
    {
        Debug.Log("Getting Object from pool " + name);
        return AllPools[name].GetObjectFromPool();
    }

    public void ReturnObjectToPool(GameObject inst,string poolName)
    {
        Debug.Log("Returning Object from to " + name);

        AllPools[poolName].ReturnObjectToPool(inst);
    }
}
