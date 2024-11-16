using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
