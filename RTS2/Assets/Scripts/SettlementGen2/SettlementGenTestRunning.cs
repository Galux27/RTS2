using UnityEngine;

public class SettlementGenTestRunning : MonoBehaviour
{
    public Settlement_Settings settings;
   public GeneratedSettlement settlement;
   
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            settlement = new GeneratedSettlement();
            settlement.GenerateSettlementAreas(settings, 64);

            SettlementGenerator.GenerateSettlement(settlement,settings);
            settlement.PopulateAreas(settings, 64);

        }

        if (settlement != null)
        {
            SettlementGenerator.DebugDrawSettlementRoads(settlement, Time.deltaTime);

        }

    }
}
