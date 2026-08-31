using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Scriptable object that stores an environment object that can be spawned in the world
/// Think trees, furniture, rocks...
/// </summary>
[CreateAssetMenu(fileName = "EnvironmentObject", menuName = "ScriptableObjects/EnvironmentObject", order = 1)]
public class EnvironmentObject : ScriptableObject
{
    public string Name;
    public Sprite ForwardsSprite,SideSprite,BackwardsSprite,HarvestedSprite;
    public bool BlocksTile;
    public int Width, Height;
    public bool CanHarvest,IsDecoration,RequiresUpdate,DestroyOnHarvest=true;
    public HarvestableResourceData Resources;
    public ResourceCapacityData CapacityData;
    public float MaxHealth;

    public EnvironmentObjectPlacementCriteria PlacementCriteria;

    public Vector3 Size()
    {
        return new Vector3(Width, Height);
    }

    public int GetWidth
    {
        get
        {
            return Width;
        }
    }

    public int GetHeight
    {
        get
        {
            return Height;
        }
    }

}
