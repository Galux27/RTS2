using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WaterData 
{
    public float WaterLevel;

    public WaterData(float level)
    {
        WaterLevel = level;
    }

    public void UpdateWaterLevel(float val)
    {
        Debug.Log("Updating water level by " + val);
        WaterLevel += val;
    }
}
