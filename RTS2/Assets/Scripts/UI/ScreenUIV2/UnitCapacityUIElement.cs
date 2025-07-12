using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCapacityUIElement : BaseUIElement
{

    static UnitCapacityUIElement instace;
    public static UnitCapacityUIElement Instance
    {
        get
        {
            if (instace == null)
            {
                instace=FindObjectOfType<UnitCapacityUIElement>(true);
            }
            return instace;
        }
    }


    public GameObject UnitCapacityPrefab, UnitCapacityParent;
    UnitPopDisplay totalDisplay;
    private void Awake()
    {
        DrawUI();
    }

    public override void DrawUI()
    {
        Init();
        base.DrawUI();
    }


    Dictionary<UnitType, UnitPopDisplay> displayUI;
    bool init = false;
    void Init()
    {
        if (init)
        {
            return;
        }

        displayUI = new Dictionary<UnitType, UnitPopDisplay>();
        totalDisplay = CreateUnitPopDisplay(Color.black);
        totalDisplay.UpdateValues(0, 0);
        displayUI.Add(UnitType.Civilian, CreateUnitPopDisplay(Color.green));
        displayUI.Add(UnitType.Rifleman, CreateUnitPopDisplay(Color.red));
        displayUI.Add(UnitType.Engineer, CreateUnitPopDisplay(Color.Lerp(Color.yellow, Color.red, .5f)));

        init = true;
    }

    public void UpdateDisplay(UserUnitTypeCount toUpdate)
    {
        Debug.Log("Updating Capacity display " + toUpdate.Type+"/"+toUpdate.Count);
        totalDisplay.UpdateValues(UnitMoniter.Instance.GetTotalUnitCount(), UnitCapacityManager.TotalCapacity);
        displayUI[toUpdate.Type].UpdateValues(toUpdate.Count, UnitCapacityManager.GetMaxCapacityForUnitType(toUpdate));
    }

    UnitPopDisplay CreateUnitPopDisplay(Color c)
    {
        GameObject g = Instantiate(UnitCapacityPrefab, UnitCapacityParent.transform);
        g.GetComponent<UnitPopDisplay>().InitUI(c);
        return g.GetComponent<UnitPopDisplay>();
    }
}
