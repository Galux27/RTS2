using UnityEngine;

public class Gargant : Unit
{

    protected void Awake()
    {
        base.Awake();
        this.GetComponent<BehaviourRunner>().SetDecisionMaker(new Gargant_BehaviourDecisionMaker());
        this.GetComponent<BehaviourRunner>().SetUnitPerforming(this);
    }

    public override float Speed()
    {
        return .5f;
    }
}
