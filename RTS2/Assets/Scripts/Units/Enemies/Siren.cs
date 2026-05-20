using UnityEngine;

public class Siren : Unit
{

    protected void Awake()
    {
        base.Awake();
        this.GetComponent<BehaviourRunner>().SetDecisionMaker(new Siren_BehaviourDecisionMaker());
        this.GetComponent<BehaviourRunner>().SetUnitPerforming(this);
    }

    public override float Speed()
    {
        return 2f;
    }
}
