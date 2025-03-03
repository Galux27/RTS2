using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPopulationUI : MonoBehaviour
{
    static UnitPopulationUI instance;
    public static UnitPopulationUI Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<UnitPopulationUI>(true);
                instance.InitUI();
            }
            return instance;
        }
    }

    Dictionary<UnitType, UnitPopDisplay> displayUI;
    public GameObject popDisplayPrefab;
    public Transform popDisplayParent;
    public void InitUI()
    {
        displayUI = new Dictionary<UnitType, UnitPopDisplay>();
        displayUI.Add(UnitType.Civilian, CreateUnitPopDisplay(Color.green));
        displayUI.Add(UnitType.Rifleman, CreateUnitPopDisplay(Color.red));
        displayUI.Add(UnitType.Engineer, CreateUnitPopDisplay(Color.Lerp(Color.yellow, Color.red, .5f)));
    }

    public void UpdateDisplay(UserUnitTypeCount toUpdate)
    {
        displayUI[toUpdate.Type].UpdateValues(toUpdate.Count, 0);
    }

    UnitPopDisplay CreateUnitPopDisplay(Color c)
    {
        GameObject g=Instantiate(popDisplayPrefab,popDisplayParent);
        g.GetComponent<UnitPopDisplay>().InitUI(c);
        return g.GetComponent<UnitPopDisplay>();
    }

    
}
