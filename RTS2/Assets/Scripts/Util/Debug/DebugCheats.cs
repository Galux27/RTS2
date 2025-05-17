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
}

