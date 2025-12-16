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
   public ZombieSpawner ZombieSpawner;

    void InitGameLifeManager()
    {
        ZombieSpawner=this.GetComponent<ZombieSpawner>();
    }


    public void OnChunkBatchGenerated(WorldChunkBatch toGenerate)
    {
        ZombieSpawner.OnWorldChunkBatchGenerated(toGenerate);
    }

    public void OnChunkBatchUnloaded(WorldChunkBatch unloading)
    {
        for (int x=0;x< unloading.Chunks.GetLength(0);x++)
        {
            for (int y = 0; y < unloading.Chunks.GetLength(1); y++)
            {
                for(int i =0;i< unloading.Chunks[x, y].UnitsInChunk.Count; i++)
                {
                    ConvertUnitToALifeEntity(unloading.Chunks[x, y].UnitsInChunk[i]);
                }
            }
        }
    }
   
    void ConvertUnitToALifeEntity(Unit u)
    {

    }

}
