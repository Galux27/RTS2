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
}
