using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Radio Tower Behavior", menuName = "ScriptableObjects/ConstructableObjectBehaviours/Radio Tower", order = 1)]
public class RadioTower_EnvironemtnObjectBehaviour : EnvironmentObjectBehaviourBase
{
    /// <summary>
    /// check for spawning civilians every x frames
    /// </summary>
    const int CheckRate = 1000;
    int checkCount = 0;
    public override bool HasUpdate()
    {
        return true;
    }
   
    public override void OnUpdate()
    {
        checkCount++;
        if (checkCount >= CheckRate)
        {
            Debug.Log("radio tower: civilian capacity = "+UnitCapacityManager.GetMaxCapacityForUnitType("Civilian"));

            if (UnitCapacityManager.GetRemainingCapacityForType("Civilian") > 0)
            {
                CreateCivilian();
            }
            checkCount = 0;
        }
    }

    void CreateCivilian()
    {
        int xCoord = 0;
        int yCoord = 0;
        if (Random.Range(0, 100) < 50)
        {
            xCoord = WorldController.Instance.WorldWidth - 1;
           
        }

        if (Random.Range(0, 100) < 50)
        {
            yCoord = WorldController.Instance.WorldHeight - 1;

        }

        UnitTypeSO civ= UnitTypesController.Instance.Units["Civilian"];
        GameObject g = Instantiate(civ.Prefab, new Vector3(xCoord, yCoord, 0),Quaternion.identity);

        MoveTo_Behaviour moveTo = new MoveTo_Behaviour();
        moveTo.InitBehaviour(g.GetComponent<Unit>(), myPosition);
        moveTo.IsUserInstruction = true;
        g.GetComponent<BehaviourRunner>().SetBehaviour(moveTo);
    }


 
}
