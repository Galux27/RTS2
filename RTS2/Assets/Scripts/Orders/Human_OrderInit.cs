using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_OrderInit : MonoBehaviour
{
    private void Awake()
    {
        UnitOrders orders = GetComponent<UnitOrders>();
        orders.AddOrder(new Order(OrderConstants.ORDER_DEFEND_SELF, false));
        orders.AddOrder(new Order(OrderConstants.ORDER_FLEE_DANGER, true));
        orders.AddOrder(new Order(OrderConstants.ORDER_PURSUE_ENEMIES, false));
        orders.AddOrder(new Order(OrderConstants.ORDER_ATTACK_NEARBY_ENEMIES, false));

    }
}
