using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Component that stores a reference to a constructable object instance in the world
/// </summary>
public class ConstructableObjectWorldReference : MonoBehaviour
{
    public ConstructableObjectInstance ObjectIReference;

    public void Init(ConstructableObjectInstance toRef)
    {
        ObjectIReference = toRef;
    }

}
