using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ObjectInfo 
{
    public string Name();
    public string Description();
    public int Quantitiy();
    public float Health();
    public float MaxHealth();

    public Vector3 Position();
}
