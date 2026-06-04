using UnityEngine;

public class PotentialBehaviourAssignment
{
    protected Unit toPerform;
    public PotentialBehaviourAssignment()
    {
    }
  
    public virtual void SetUnit(Unit unit)
    {
        toPerform= unit; 
    }

    public virtual void AssignBehaviour()
    {

    }

    public virtual string PotentialBehaviourName()
    {
        return "";
    }
}

public class PlantSeeds_PotentialBehaviour : PotentialBehaviourAssignment
{
    EnvironmentObjectInstance toPlantSeedsIn;
    public PlantSeeds_PotentialBehaviour(EnvironmentObjectInstance obj) : base() {toPlantSeedsIn= obj;  }

    public override void AssignBehaviour()
    {
        PlantSeeds_Behaviour action = new PlantSeeds_Behaviour();
        action.InitBehaviour(toPerform, toPlantSeedsIn);
        toPerform.BehaviourRunner.SetBehaviour(action);
    }

    public override string PotentialBehaviourName()
    {
        return "Plant Seeds";
    }
}

public class HarvestPlanter_PotentialBehaviour : PotentialBehaviourAssignment
{
    EnvironmentObjectInstance toHarvest;
    public HarvestPlanter_PotentialBehaviour(EnvironmentObjectInstance obj) : base() { toHarvest = obj; }

    public override void AssignBehaviour()
    {
        HarvestPlanter_Behaviour action = new HarvestPlanter_Behaviour();
        action.InitBehaviour(toPerform, toHarvest);
        toPerform.BehaviourRunner.SetBehaviour(action);
    }

    public override string PotentialBehaviourName()
    {
        return "Harvest Planter";
    }
}
