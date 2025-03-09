using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConstructableObject", menuName = "ScriptableObjects/ConstructableObject", order = 1)]
public class ConstructableObject : EnvironmentObject
{
    public int Cost;
    public float TimeToBuild;
    public EnvironmentObjectBehaviourBase MyBehaviour;

    public virtual void OnObjectConstructed(GameObject obj)
    {
        Debug.Log("Constructable created "+obj.name);
    }
    
}
