using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{
    public GameObjectPool(GameObject prefabOfBase,int countToInit)
    {
        prefab = prefabOfBase;
        for(int x=0; x < countToInit; x++)
        {
            GenerateNewInstance();
        }
    }

    GameObject prefab;
    public List<GameObject> Active=new List<GameObject>(), Inactive=new List<GameObject>();

    void GenerateNewInstance()
    {
        GameObject inst = GameObject.Instantiate(prefab);
        inst.SetActive(false);
        Inactive.Add(inst);
    }

    public void ReturnObjectToPool(GameObject g)
    {
        Active.Remove(g);
        g.SetActive(false);
        Inactive.Add(g);
    }

    public GameObject GetObjectFromPool()
    {
        if (Inactive.Count == 0)
        {
            GenerateNewInstance();
        }
        GameObject inst = Inactive[0];
        Inactive.RemoveAt(0);
        return inst;
    }
}
