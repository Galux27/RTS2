using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    public void SetOrdersFromFile(Dictionary<string,bool> orders)
    {
        
        foreach(KeyValuePair<string,bool> kvp in orders)
        {
            if (!orders.ContainsKey(kvp.Key))
            {

                AddOrder(new Order(kvp.Key, kvp.Value));
            }
            else
            {
                SetOrder(kvp.Key, kvp.Value);
            }
        }
        Destroy(this.GetComponent<Human_OrderInit>());
    }

    public string SerializeOrders()
    {
        StringBuilder stringBuilder = new StringBuilder();
        
        foreach(KeyValuePair<string,Order> kvp in Orders)
        {
            stringBuilder.Append(kvp.Value.Key);
            stringBuilder.Append(SerializeDataHelpers.LIST_ELEMENT_SPLIT);

            if (kvp.Value.Value)
            {
                stringBuilder.Append("1");

            }
            else
            {
                stringBuilder.Append("0");

            }
            stringBuilder.Append(SerializeDataHelpers.DATA_SPLIT);

        }

        return stringBuilder.ToString();
    }
    
}
