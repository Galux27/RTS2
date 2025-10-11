using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldChunkBatchPool
{
    public static List<WorldChunkBatch> AvailableBatches=new List<WorldChunkBatch>();
    static bool Init = false;
    const int NumberOfBatchesToInit = 20;

    public static void InitPool()
    {
        if (Init)
        {
            return;
        }
        for(int x=0;x<NumberOfBatchesToInit;x++)
        {
            AvailableBatches.Add(CreateWorldChunkBatch());
        }
        Init = true;
    }


    public static WorldChunkBatch CreateWorldChunkBatch()
    {
        GameObject g = new GameObject();
        WorldChunkBatch wcb = g.AddComponent<WorldChunkBatch>();
        wcb.OnBatchCreated();
        return wcb;
    }

    public static WorldChunkBatch GetChunkBatch()
    {
        if (!Init)
        {
            InitPool();
        }
        if (AvailableBatches.Count == 0)
        {
            AvailableBatches.Add(CreateWorldChunkBatch());
        }
        WorldChunkBatch retVal = AvailableBatches[0];
        AvailableBatches.RemoveAt(0);
        return retVal;
    }

    public static void ReturnChunkBatch(WorldChunkBatch toReturn)
    {
        AvailableBatches.Add(toReturn);
    }
}
