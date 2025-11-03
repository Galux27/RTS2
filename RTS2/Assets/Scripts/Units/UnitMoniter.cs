using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
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
                if (instance != null)
                {
                    instance.Init();
                }
            }
            return instance; 
        }

    }

    void Init()
    {
        unitCounts.Add(UnitType.Engineer,new UserUnitTypeCount(UnitType.Engineer));
        unitCounts.Add(UnitType.Rifleman, new UserUnitTypeCount(UnitType.Rifleman));
        unitCounts.Add(UnitType.Civilian, new UserUnitTypeCount(UnitType.Civilian));

    }


    public List<Unit> AllUnits=new List<Unit>();
    Dictionary<UnitType, UserUnitTypeCount> unitCounts=new Dictionary<UnitType, UserUnitTypeCount>();

    public int GetUserUnitCount(string type)
    {
        if (type == "Rifleman")
        {
            if (unitCounts.ContainsKey(UnitType.Rifleman))
            {
                return unitCounts[UnitType.Rifleman].Count;
            }
            return 0;
            }
        else if (type == "Engineer")
        {
            if (unitCounts.ContainsKey(UnitType.Engineer))
            {
                return unitCounts[UnitType.Engineer].Count;
            }
            return 0;
        }
        else if (type == "Civilian")
        {

            if (unitCounts.ContainsKey(UnitType.Civilian))
            {
                return unitCounts[UnitType.Civilian].Count;
            }
            return 0;
        }
        return 99999999;
    }

    public void AddUnit(Unit toAdd)
    {
    
       if (toAdd.MyFaction.MyFactionID == FactionController.USER_FACTION)
       {
           IncreaseUnitCount(toAdd);
       }
        //else if (toAdd.MyFaction.MyFactionID == FactionController.ZOMBIE_FACTION)
        //{
        //    WorldChunkManager.Instance.GetWorldChunkBatchFromPosition(new Vector2Int(Mathf.RoundToInt(toAdd.transform.position.x), Mathf.RoundToInt(toAdd.transform.position.y))).AddUnitToBatch(toAdd);
        //    ZombieController.Instance.AddZombieToMoniter(toAdd);
        //}

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
        if (UnitCapacityUIElement.Instance == null)
        {
            return;
        }
        foreach(KeyValuePair<UnitType, UserUnitTypeCount > KeyValuePair in unitCounts)
        {
            UnitCapacityUIElement.Instance.UpdateDisplay(KeyValuePair.Value);
        }
    }

    public void RemoveUnit(Unit toRemove) 
    {
        if (toRemove.MyFaction.MyFactionID == FactionController.USER_FACTION&&AllUnits.Contains(toRemove))
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
