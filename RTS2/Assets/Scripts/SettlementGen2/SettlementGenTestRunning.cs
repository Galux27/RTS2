using UnityEngine;

public class SettlementGenTestRunning : MonoBehaviour
{
    public Settlement_Settings settings;
   public GeneratedSettlement settlement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            settlement=SettlementGenerator.GenerateSettlement(settings);
        }
        if (settlement != null)
        {
            for(int x=0;x<settlement.highways.Count;x++)
            {
                Debug.DrawLine(settlement.highways[x].StartPos,settlement.highways[x].EndPos, settlement.highways[x].debugColor);  
            }
        }
    }
}
