using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SelectionUtilities
{
    static List<Unit> toCheck=new List<Unit>();
    static List<WorldChunk> chunksToCheck = new List<WorldChunk>();
    public static Unit GetUnitWithinRangeOfPoint(Vector3 point,float maxDist,UnitType typeToGet)
    {
        point.z = 0;
        Unit retVal = null;
        chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(maxDist+(WorldChunkManager.ChunkSize), point);
        for (int x = 0; x < chunksToCheck.Count; x++)
        {
            
            toCheck.AddRange(chunksToCheck[x].UnitsInChunk);
        }

        float closest = 99999f;
        float curDist = -1f;
        Vector3 unitPosition = Vector3.zero;
        for (int x = 0; x < toCheck.Count;x++)
        {
            unitPosition = toCheck[x].transform.position;
            unitPosition.z= 0;
            if (toCheck[x].MyType == typeToGet)
            {
                curDist = Vector3.Distance(point, unitPosition);
                if (curDist < maxDist && curDist < closest)
                {
                    retVal = toCheck[x];
                    closest = curDist;
                }
            }
        }
        toCheck.Clear();
        chunksToCheck.Clear();
        return retVal;
    }
}
