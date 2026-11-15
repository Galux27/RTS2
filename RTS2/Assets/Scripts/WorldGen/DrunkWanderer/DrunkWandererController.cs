using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class DrunkWandererController : MonoBehaviour
{
    
    static DrunkWandererController instance;
    public static DrunkWandererController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DrunkWandererController>(true);
            }
            return instance;
        }
    }

    private void Awake()
    {
        for(int x = 0; x < 20; x++)
        {
            Vector2Int test = new Vector2Int(Random.Range(-10000, 10000), Random.Range(-10000, 10000));
            Vector2Int converted = ConvertWorldCoordsToDrunkCoords(test);
            Debug.Log("Conversion: " + test + "->" + converted);
        }
    }

    public Vector2Int ConvertWorldCoordsToDrunkCoords(Vector2Int position)
    {
        Vector2Int retVal = new Vector2Int(RoundToMultiple(position.x, DrunkWanderer.Width()), RoundToMultiple(position.y, DrunkWanderer.Height()));
        if (retVal.x > position.x)
        {
            retVal.x -= DrunkWanderer.Width();
        }

        if (retVal.y > position.y)
        {
            retVal.y-=DrunkWanderer.Height();
        }

        return retVal;
    }
    int RoundToMultiple(int value, int roundTo)
    {
        return (value / roundTo) * roundTo;
    }
}
