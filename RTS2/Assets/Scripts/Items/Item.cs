using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Scriptable object to store a potential item that could be held or carried by a unit
/// </summary>
[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class Item:ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Sprite;
    public ItemEquipSlot Slot;
    public List<ItemTags> Tags;
}

public enum ItemEquipSlot 
{ 
    Hands,
    Inventory
}

public enum ItemTags 
{ 
    Empty,
    Weapon,
    Storage
}

