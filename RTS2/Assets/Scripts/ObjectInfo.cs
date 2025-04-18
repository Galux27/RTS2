using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ObjectInfo :Health
{
    public string Name();
    public string Description();
    public int Quantitiy();
  
    public Vector3 Position();
}
