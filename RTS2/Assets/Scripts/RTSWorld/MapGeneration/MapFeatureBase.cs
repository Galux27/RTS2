using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFeatureBase:ScriptableObject
{
    public string FeatureName;
    public int MinWidth, MaxWidth, MinHeight, MaxHeight;
    public virtual void GenerateFeature()
    {

    }  

    public virtual void OnStartGenerate()
    {

    }
}
