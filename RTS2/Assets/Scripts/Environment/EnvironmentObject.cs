using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnvironmentObject", menuName = "ScriptableObjects/EnvironmentObject", order = 1)]
public class EnvironmentObject : ScriptableObject
{
    public string Name;
    public Sprite ForwardsSprite,SideSprite,BackwardsSprite;
    public bool BlocksTile;
    public int Width, Height;
}
