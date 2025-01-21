using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConstructableContainerObject", menuName = "ScriptableObjects/ConstructableContainerObject", order = 1)]
public class ConstructableContainer : ConstructableObject
{
    public float WeightLimit;
    public ItemFilter Filter;

    public override void OnObjectConstructed(GameObject obj)
    {
        Inventory iv = obj.AddComponent<Inventory>();
        iv.InventoryCapacity = WeightLimit;
        iv.Filter= Filter;
    }
}
