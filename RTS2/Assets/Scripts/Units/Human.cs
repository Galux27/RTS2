using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Unit
{
    protected void Awake()
    {
        this.GetComponent<BehaviourRunner>().SetDecisionMaker(new Human_BehaviourDecisionMaker());
        this.GetComponent<BehaviourRunner>().SetUnitPerforming(this);
        this.GetComponent<ItemHolder>().OnSetHolding += this.GetComponent<UnitAttackController>().OnNewItem;
        base.Awake();
    }
}
