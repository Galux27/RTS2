using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class UnitOrdersUI : BaseUIElement
{
    public const string ButtonPoolKey = "OrderButton";
    public Transform OrderUIParent;
    List<GameObject> OrderButtons = new List<GameObject>();
    public void Cleanup()
    {
      
        for(int x=0;x< OrderButtons.Count; x++)
        {
            OrderButtons[x].transform.parent = null;
            OrderButtons[x].gameObject.SetActive(false);
            GameObjectPoolManager.Instance.ReturnObjectToPool(OrderButtons[x], ButtonPoolKey);

        }
        OrderButtons.Clear();
    }

    void DrawButton(OrderCounter following)
    {
        Debug.Log("Drawing button for order " + following.OrderKey);
        GameObject button = GameObjectPoolManager.Instance.GetObjectFromPool(ButtonPoolKey);
        button.gameObject.SetActive(true);
        UnitOrderButton orderBut = button.GetComponent<UnitOrderButton>();
        button.transform.parent = OrderUIParent;
        orderBut.SetOrderDetails(following);
        OrderButtons.Add(button);
    }

    Dictionary<string, OrderCounter> GetOrdersFromSelectedUnits(List<Unit> SelectedUnits)
    {
        Dictionary<string, OrderCounter> Orders = new Dictionary<string, OrderCounter>();
        for (int x = 0; x < SelectedUnits.Count; x++)
        {
            foreach (KeyValuePair<string, Order> kvp in SelectedUnits[x].MyOrders.Orders)
            {
                if (!Orders.ContainsKey(kvp.Value.Key))
                {
                    Orders.Add(kvp.Value.Key, new OrderCounter(kvp.Value.Key));
                }
                if (kvp.Value.Value)
                {
                    Orders[kvp.Value.Key].Following++;
                }
                else
                {
                    Orders[kvp.Value.Key].NotFollowing++;

                }
            }
        }
        return Orders;
    }

    public void UpdateOrderUI(List<Unit> SelectedUnits)
    {
        Cleanup();
       
        foreach(KeyValuePair<string,OrderCounter> kvp in GetOrdersFromSelectedUnits(SelectedUnits))
        {
            DrawButton(kvp.Value);
        }
    }
}

public class OrderCounter
{
    public string OrderKey;
    public int Following, NotFollowing;
    public OrderCounter(string key)
    {
        OrderKey = key;
    }
}
