using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Unit
{

    protected void Awake()
    {
    
        base.Awake();
        this.GetComponent<BehaviourRunner>().SetDecisionMaker(new Zombie_BehaviourDecisionMaker());
        this.GetComponent<BehaviourRunner>().SetUnitPerforming(this);
    }

    public override float Speed()
    {
        return 1f;
    }
}
