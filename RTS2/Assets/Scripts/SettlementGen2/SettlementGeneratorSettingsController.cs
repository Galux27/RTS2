using UnityEngine;

public class SettlementGeneratorSettingsController : MonoBehaviour
{
    static SettlementGeneratorSettingsController instance;
    public static SettlementGeneratorSettingsController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SettlementGeneratorSettingsController>();
            }
            return instance;
        }
    }
    public Settlement_Settings BaseSettings;
}
