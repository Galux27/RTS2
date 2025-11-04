using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitVisualManager : MonoBehaviour
{
    static UnitVisualManager instance;
    public static UnitVisualManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UnitVisualManager>(true);
                instance.Init();
            }
            return instance;
        }
    }

    public Dictionary<UnitType, UnitVisual> AllVisuals;
    const string FilePath = "UnitVisuals";

    void Init()
    {
        AllVisuals = new Dictionary<UnitType, UnitVisual>();

        Object[] items = Resources.LoadAll(FilePath);
        for (int x = 0; x < items.Length; x++)
        {
            UnitVisual i = (UnitVisual)items[x];
            if (AllVisuals.ContainsKey(i.TypeFor) == false)
            {
                AllVisuals.Add(i.TypeFor, i);
            }
        }
    }
}
