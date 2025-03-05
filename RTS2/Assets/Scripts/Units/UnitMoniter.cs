using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoniter : MonoBehaviour
{
    static UnitMoniter instance;
    public static UnitMoniter Instance
    {
        get 
        { 
            if(instance == null)
            {
                instance = FindObjectOfType<UnitMoniter>();
            }
            return instance; 
        }

    }

    public List<Unit> AllUnits=new List<Unit>();
    Dictionary<UnitType, UserUnitTypeCount> unitCounts=new Dictionary<UnitType, UserUnitTypeCount>();

    public void AddUnit(Unit toAdd)
    {
        if (toAdd.MyFaction.MyFactionID == FactionController.USER_FACTION)
        {
            IncreaseUnitCount(toAdd);
        }
        AllUnits.Add(toAdd);

    }

    public int GetTotalUnitCount()
    {
        int retVal = 0;

        foreach(KeyValuePair<UnitType,UserUnitTypeCount> kvp in unitCounts)
        {
            retVal += kvp.Value.Count;
        }

        return retVal;
    }

    void IncreaseUnitCount(Unit toAdd)
    {
        if(!unitCounts.ContainsKey(toAdd.MyType))
        {
            unitCounts.Add(toAdd.MyType, new UserUnitTypeCount(toAdd.MyType));
        }
        unitCounts[toAdd.MyType].Count++;
        OnUnitCountsChanged();

    }

    void DecreaseUnitCount(Unit toRemove)
    {
        if (!unitCounts.ContainsKey(toRemove.MyType))
        {
            unitCounts.Add(toRemove.MyType, new UserUnitTypeCount(toRemove.MyType));
        }
        unitCounts[toRemove.MyType].Count--;
        OnUnitCountsChanged();
    }

    public void OnUnitCountsChanged()
    {
        foreach(KeyValuePair<UnitType, UserUnitTypeCount > KeyValuePair in unitCounts)
        {
            UnitPopulationUI.Instance.UpdateDisplay(KeyValuePair.Value);
        }
    }

    public void RemoveUnit(Unit toRemove) 
    {
        if (toRemove.MyFaction.MyFactionID == FactionController.USER_FACTION)
        {
            DecreaseUnitCount(toRemove);
        }
        AllUnits.Remove(toRemove);
    }

    public List<Unit> GetUnitsWithinBounds(Vector3 pos1,Vector3 pos2)
    {
        Vector3 high = new Vector3();
        Vector3 low = new Vector3();
        if (pos1.x > pos2.x)
        {
            high.x = pos1.x;
            low.x = pos2.x;
        }
        else
        {
            high.x = pos2.x;
            low.x = pos1.x;
        }
        if (pos1.y > pos2.y)
        {
            high.y = pos1.y;
            low.y = pos2.y;
        }
        else
        {
            high.y = pos2.y;
            low.y = pos1.y;
        }
        if (pos1.z > pos2.x)
        {
            high.z = pos1.z;
            low.z = pos2.z;
        }
        else
        {
            high.z = pos2.z;
            low.z = pos1.z;
        }

        List<Unit> units = new List<Unit>();
        Vector3 pos = new Vector3();
        for(int x = 0; x < AllUnits.Count; x++)
        {
            pos = AllUnits[x].transform.position;
            if(pos.x<=high.x && pos.x >= low.x)
            {
                if(pos.y<=high.y&& pos.y >= low.y)
                {
                    units.Add(AllUnits[x]);
                }
            }
        }

        return units;
    }
}

public class UserUnitTypeCount
{
    public UnitType Type;
    public int Count;

    public UserUnitTypeCount(UnitType type)
    {
        Type = type;
        Count = 0;
    }
}
