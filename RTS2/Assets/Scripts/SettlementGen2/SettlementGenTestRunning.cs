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
            settlement.GenerateSettlementAreas(settings,64);
        }
        if (settlement != null&&settlement.areas!=null)
        {
            for(int q = 0; q < settlement.areas.GetLength(0); q++)
            {
                for(int  r = 0;r<settlement.areas.GetLength(1); r++)
                {
                   
                    for (int x = 0; x < settlement.areas[q, r].roads.Count; x++)
                    {
                        Debug.DrawLine(settlement.areas[q, r].roads[x].StartPos, settlement.areas[q, r].roads[x].EndPos, settlement.areas[q, r].DebugColour);
                    }


                    for (int x = 0; x < settlement.areas[q, r].avenues.Count; x++)
                    {
                        Debug.DrawLine(settlement.areas[q, r].avenues[x].StartPos, settlement.areas[q, r].avenues[x].EndPos, settlement.areas[q, r].DebugColour);
                    }

                    for (int x = 0; x < settlement.areas[q, r].highways.Count; x++)
                    {
                        Debug.DrawLine(settlement.areas[q, r].highways[x].StartPos, settlement.areas[q, r].highways[x].EndPos, settlement.areas[q, r].DebugColour);
                    }
                }
            }


        }
    }
}
