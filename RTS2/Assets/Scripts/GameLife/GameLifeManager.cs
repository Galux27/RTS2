using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLifeManager : MonoBehaviour
{
    static GameLifeManager instance;
    public static GameLifeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameLifeManager>();
                instance.InitGameLifeManager();
            }
            return instance;
        }
        
    }
    ZombieSpawner ZombieSpawner;

    void InitGameLifeManager()
    {
        ZombieSpawner=this.GetComponent<ZombieSpawner>();
    }


    public void OnChunkBatchGenerated(WorldChunkBatch toGenerate)
    {
        ZombieSpawner.OnWorldChunkBatchGenerated(toGenerate);
    }

   
}
