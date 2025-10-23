using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentObjectBehaviourBase:ScriptableObject
{
    public Vector3 myPosition;

    public virtual bool HasUpdate()
    {
        return false;
    }

    public virtual void OnUpdate()
    {

    }


    public virtual void PassInVector(Vector3 data,string use)
    {
        if (use == "POS")
        {
            myPosition = data;
        }
    }
}
