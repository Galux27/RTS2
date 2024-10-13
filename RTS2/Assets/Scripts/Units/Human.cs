using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Unit
{
    protected void Awake()
    {
        this.GetComponent<BehaviourRunner>().SetDecisionMaker(new BehaviourDecisionMaker());
        this.GetComponent<BehaviourRunner>().SetUnitPerforming(this);
        base.Awake();
    }
}
