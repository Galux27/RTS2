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

    public Vector3 Size()
    {
        return new Vector3(Width, Height);
    }

    public int HalfWidth
    {
        get
        {
            return Mathf.Max(Width / 2, 1);
        }
    }

    public int HalfHeight
    {
        get
        {
            return Mathf.Max(Height / 2, 1);
        }
    }
}
