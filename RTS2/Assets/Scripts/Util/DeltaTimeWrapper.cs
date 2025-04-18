using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DeltaTimeWrapper
{
   public static float GameplayDeltaMultiplier = 1f;
   public static float GameplayDelta
   {
        get
        {
            return Time.deltaTime * GameplayDeltaMultiplier;
        }
    }
}
