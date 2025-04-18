using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "ScriptableObjects/Resource", order = 1)]
public class Resource : ScriptableObject
{
    public string Name;
    public Sprite Item;
    public List<string> ContainersICanBeStoredIn;
    public float WeightPerUnit;
}
