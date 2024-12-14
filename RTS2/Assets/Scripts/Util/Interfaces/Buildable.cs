using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Buildable 
{
    public Vector3 GetPosition();
    public float MaxDistToConstruct();
    public void OnConstructionComplete();
    public void Construct();
}
