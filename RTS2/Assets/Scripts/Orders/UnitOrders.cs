using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Stores all the orders given to set unit
/// </summary>
public class UnitOrders : MonoBehaviour
{
    public Dictionary<string, Order> Orders=new Dictionary<string, Order>();

    public Order GetOrder(string key)
    {
        if(Orders.ContainsKey(key)) return Orders[key];
        return null;
    }

    public void AddOrder(Order order)
    {
        if(!Orders.ContainsKey(order.Key))
        {
            Orders.Add(order.Key, order);
        }
    }

    public void SetOrder(string key, bool val)
    {
        if (Orders.ContainsKey(key))
        {
            Orders[key].Value = val;
        }

    }
}
