using UnityEngine;
using UnityEngine.UI;
public class SettlementGenTestRunning : MonoBehaviour
{
    public Settlement_Settings settings;
   public GeneratedSettlement settlement;
    public SettlementTileArea area;
    public Texture2D Debug;
    public RawImage DebugDisplay;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            settlement = new GeneratedSettlement();
            settlement.GenerateSettlementAreas(settings, 64);

            SettlementGenerator.GenerateSettlement(settlement,settings);
            settlement.PopulateAreas(settings, 64);
            area = new SettlementTileArea(settlement, settings);
            settlement.AssignBuildingsToArea(area, 64, settings);
            Debug=area.GenerateDebugTexture();
            DebugDisplay.texture= Debug;
        }

        if (settlement != null)
        {
            SettlementGenerator.DebugDrawSettlementRoads(settlement, Time.deltaTime);

        }

    }
}
