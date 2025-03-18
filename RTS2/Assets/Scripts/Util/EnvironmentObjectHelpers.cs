using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnvironmentObjectHelpers
{
    public static EnvironmentObject GetEnvironmentObject(string key)
    {
        if (EnvironmentObjectManager.Instance.AllObjects.ContainsKey(key))
        {
            return EnvironmentObjectManager.Instance.AllObjects[key];
        }else if (ConstructableObjectManager.Instance.AllObjects.ContainsKey(key))
        {
            return ConstructableObjectManager.Instance.AllObjects[key];
        }

        return null;
    }
}
