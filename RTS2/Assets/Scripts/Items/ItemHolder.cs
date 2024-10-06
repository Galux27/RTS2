using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public Transform HandsSlot;
    public ItemInWorld CurrentlyHolding;
    public Action<ItemInWorld> OnSetHolding;
    public void SetHolding(ItemInWorld toHold)
    {
        CurrentlyHolding= toHold;
        CurrentlyHolding.transform.parent = HandsSlot;
        CurrentlyHolding.transform.localPosition = Vector3.zero;
        CurrentlyHolding.transform.localRotation = Quaternion.identity;
        CurrentlyHolding.transform.localScale = Vector3.one;
        OnSetHolding?.Invoke(toHold);
    }
}
