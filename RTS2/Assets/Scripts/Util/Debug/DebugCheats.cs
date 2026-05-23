using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCheats : MonoBehaviour
{
    static DebugCheats instance;
    public static DebugCheats Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<DebugCheats>(true);
            }
            return instance;
        }
    }

    public bool InstantConstructVal = true;
    public bool InstantConstruct()
    {
#if UNITY_EDITOR
        return InstantConstructVal;
#endif
        return false;
    }

    public bool CareAboutResources = false;
    public bool CareAboutResourcesNeeded()
    {
#if UNITY_EDITOR
        return CareAboutResources;
#endif
        return true;
    }

    public bool DrawDebugPathfinding = false;
    public bool DoWeDrawDebugPathfinidng()
    {
        return DrawDebugPathfinding;
    }

    public bool SpawnEnemies = true;
    public bool DoWeSpawnEnemies()
    {
#if UNITY_EDITOR
        return SpawnEnemies;
#endif
        return false;
    }

    public bool LogBehaviourDetails = true;
    public bool DoWeLogBehaviourDetails()
    {
#if UNITY_EDITOR
        return LogBehaviourDetails;
#endif
        return false;
    }


    public bool DrawSelectableBounds = true;
    public bool DoWeDrawSelectableBounds()
    {
#if UNITY_EDITOR
        return DrawSelectableBounds;
#endif
        return false;
    }
}

