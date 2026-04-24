using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for controling what items a unit holds and any needed visuals for things like guns
/// </summary>
public class ItemHolder : MonoBehaviour
{
    public Transform HandsSlot;
    public ItemInWorld CurrentlyHolding;
    public Action<ItemInWorld> OnSetHolding;
    public void SetHolding(ItemInWorld toHold)
    {
        CurrentlyHolding= toHold;
        CurrentlyHolding.transform.parent = HandsSlot;
        CurrentlyHolding.transform.localPosition =new Vector3(0, 0);
        CurrentlyHolding.transform.localRotation = Quaternion.identity;
        CurrentlyHolding.transform.localScale = Vector3.one;
        CurrentlyHolding.GetComponent<SpriteRenderer>().sortingOrder = 18;
        OnSetHolding?.Invoke(toHold);
    }


    public bool IsHoldingItem()
    {
        return CurrentlyHolding != null;
    }

    public bool IsHoldingWeapon()
    {
        return IsHoldingItem() && CurrentlyHolding.MyItem.Tags.Contains(ItemTags.Weapon);
    }
}
