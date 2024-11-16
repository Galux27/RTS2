using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class to store values related to unit behaviour e.g. do we attack if attacked, do we fall back or stand ground etc...
/// </summary>
public class Order
{
    public string Key;
    public bool Value=false;

    public Order(string key,bool value)
    {
        this.Key = key;
        this.Value = value;
    }
}
