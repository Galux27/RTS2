using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderUISelect : BaseUI
{

    static OrderUISelect instance;
    public static OrderUISelect Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<OrderUISelect>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        SelectableManager.OnSelectionChanged += RefreshUI;
    }


    public GameObject OrderButtonPrefab, OrderButtonParent;


    public override void RefreshUI()
    {
        CleanupExisting();
        List<Unit> units = SelectableManager.Instance.GetSelectedUnits();
        if (units.Count == 0)
        {
            return;
        }
        Dictionary<string, int> orderData = new Dictionary<string, int>();
        for(int x=0;x < units.Count;x++)
        {
            UnitOrders order = units[x].MyOrders;
            if(order != null)
            {
                foreach(KeyValuePair<string,Order> kvp in order.Orders)
                {
                    if (!orderData.ContainsKey(kvp.Key))
                    {
                        orderData.Add(kvp.Key, 0);
                    }
                    if (kvp.Value.Value)
                    {
                        orderData[kvp.Key]++;
                    }
                    else
                    {
                        orderData[kvp.Key]--;
                    }
                }
            }
        }
        

        foreach(KeyValuePair<string,int> kvp in orderData)
        {
            bool initVal = false;
            if (kvp.Value >= 0)
            {
                initVal = true;
            }
            GameObject button = Instantiate(OrderButtonPrefab, OrderButtonParent.transform);
            OrderButton ob  = button.GetComponent<OrderButton>();
            ob.InitButton(kvp.Key, initVal);
        }
    }

    void CleanupExisting()
    {
        for(int x=0;x<OrderButtonParent.transform.childCount;x++)
        {
            GameObject.Destroy(OrderButtonParent.transform.GetChild(x).gameObject);
        }
    }
}
