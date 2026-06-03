using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Planter Behavior", menuName = "ScriptableObjects/ConstructableObjectBehaviours/Planter", order = 1)]
public class PlanterBehaviour :EnvironmentObjectBehaviourBase
{
    EnvironmentObjectInstance myObject;
    public float GrowDuration = 90f;
    bool DoneFirstUpdate = false;
    public bool HasBeenSeeded = false,Grown=false;
    float GrowStartTime = -1f;
    public override bool HasUpdate()
    {
        return true;
    }
    public bool IsDone()
    {
        return Grown;
    }

    public void SeedPlanter()
    {
        Debug.Log("Planter: seeded planter");
        HasBeenSeeded = true;
    }

    public void Harvest()
    {
        Grown = false;
        DoneFirstUpdate = false;
        GrowStartTime = -1;
    }

    public override void PassInEnvironmentObjectInstance(EnvironmentObjectInstance instance)
    {
        myObject= instance;
    }

    public override void PerformCheckForActionsFromObject(out List<PotentialBehaviourAssignment> retVal)
    {
        retVal = new List<PotentialBehaviourAssignment>();
        if (HasBeenSeeded == false)
        {
            retVal.Add(new PlantSeeds_PotentialBehaviour(myObject));
        }else if (Grown)
        {

        }
    }

    public override PotentialBehaviourAssignment GetPotentialBehaviour(int index)
    {
 
            return new PlantSeeds_PotentialBehaviour(myObject);
   
    }

    public override void OnUpdate()
    {
        if (HasBeenSeeded)
        {
            if (!DoneFirstUpdate)
            {
                GrowStartTime = GameTime.Instance.InGameTime;
                DoneFirstUpdate = true;
            }
            Debug.Log("Planter: Seed Growth Progress " + (GameTime.Instance.InGameTime - GrowStartTime));
            if(GameTime.Instance.InGameTime-GrowStartTime> GrowDuration)
            {
                Debug.Log("Planter: grown");

                Grown = true;
                HasBeenSeeded = false;
            }
            
        }
    }
}
