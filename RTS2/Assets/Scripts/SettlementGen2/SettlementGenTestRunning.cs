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
            settlement=SettlementGenerator.GenerateSettlement(settings);
        }
        if (settlement != null)
        {
            for (int x = 0; x < settlement.roads.Count; x++)
            {
                Debug.DrawLine(settlement.roads[x].StartPos, settlement.roads[x].EndPos, settlement.roads[x].debugColor);
            }


            for (int x = 0; x < settlement.avenues.Count; x++)
            {
                Debug.DrawLine(settlement.avenues[x].StartPos, settlement.avenues[x].EndPos,Color.magenta);
            }

            for (int x = 0; x < settlement.highways.Count; x++)
            {
                Debug.DrawLine(settlement.highways[x].StartPos, settlement.highways[x].EndPos, Color.cyan);
            }
        }
    }
}
