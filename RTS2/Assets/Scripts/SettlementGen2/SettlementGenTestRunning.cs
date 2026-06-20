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
                        Debug.DrawLine(settlement.areas[q, r].highways[x].StartPos, settlement.areas[q, r].highways[x].EndPos, Color.cyan);
                    }


                }
            }

            for(int x = 0; x < settlement.River.RiverSections.Count; x++)
            {
                Debug.DrawLine(settlement.River.RiverSections[x].StartPos, settlement.River.RiverSections[x].EndPos, Color.blue);
                Debug.DrawLine(settlement.River.RiverSections[x].PosSideStart, settlement.River.RiverSections[x].PosSideEnd, Color.blue);
                Debug.DrawLine(settlement.River.RiverSections[x].NegSideStart, settlement.River.RiverSections[x].NegSideEnd, Color.blue);

            }
        }
    }
}
