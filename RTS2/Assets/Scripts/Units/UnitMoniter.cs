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

    public void AddUnit(Unit toAdd)
    {
        AllUnits.Add(toAdd);

    }

    public void RemoveUnit(Unit toRemove) 
    {
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
            Debug.Log("Checking pos " + pos);
            if(pos.x<=high.x && pos.x >= low.x)
            {
                if(pos.y<=high.y&& pos.y >= low.y)
                {
                    units.Add(AllUnits[x]);
                }
            }
        }
        Debug.Log("Getting selected between " + high + " & " + low+" found " + units.Count+"/"+AllUnits.Count);

        return units;
    }
}
