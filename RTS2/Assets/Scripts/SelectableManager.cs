using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectableManager : MonoBehaviour
{
    static SelectableManager instance;
    public static SelectableManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<SelectableManager>();
            }
            return instance;
        }
    }



    public static System.Action OnSelectionChanged;
    public List<Selectable> CurrentlySelected=new List<Selectable>();
    public SelectableType CurrentSelectedType;
    public void AddSelectable(List<Selectable> selectables)
    {
        for(int x=0;x<selectables.Count;x++)
        {
            AddSelectable(selectables[x]);
        }
    }


    public void AddSelectable(Selectable toAdd)
    {
        if (toAdd.IsSelectable()&& toAdd.GetIsSelected() == false)
        {
            CurrentlySelected.Add(toAdd);
            toAdd.SetIsSelected(true);  
        }
    }

    public void ClearSelectables()
    {
        for(int x=0; x < CurrentlySelected.Count; x++)
        {
            CurrentlySelected[x].SetIsSelected(false);
        }
        CurrentlySelected.Clear();
        OnSelectionChanged?.Invoke();
    }

    public void RemoveSelectable(Selectable toRemove)
    {
        if (toRemove.GetIsSelected()==true)
        {
            CurrentlySelected.Remove(toRemove);
            toRemove.SetIsSelected(false);
        }
        OnSelectionChanged?.Invoke();
    }

    public void SetToOnlySelected(Selectable toSet)
    {
        if (toSet != null)
        {
            ClearSelectables();
            AddSelectable(toSet);
            OnSelectionChanged?.Invoke();

        }
    }

    public void SetToOnlyNameSelected(string key)
    {
        List<Selectable> newSelected = new List<Selectable>();

        ObjectInfo oi = null;
        for (int x = 0; x < CurrentlySelected.Count; x++)
        {
          oi = (ObjectInfo)CurrentlySelected[x];
            if (oi != null)
            {
                if (oi.Name() == key)
                {
                    newSelected.Add(CurrentlySelected[x]);
                }
            }
        }
        ClearSelectables();
        AddSelectable(newSelected);
        Debug.Log("Set to name selected " + key + "Found " +  newSelected.Count);
        OnSelectionChanged?.Invoke();

    }

    public void SetOnlyTypeSelected(UnitType toSelect)
    {
        Dictionary<UnitType, List<Unit>> curSelection = FilterUnitsByType();
        ClearSelectables();
        if(curSelection.ContainsKey(toSelect))
        {
            for(int x=0;x< curSelection[toSelect].Count; x++)
            {
                AddSelectable(curSelection[toSelect][x]);
            }
        }
    }

    public List<Selectable> GetAllSelectableOfType(SelectableType typeToGet)
    {
        List<Selectable> retVal = new List<Selectable>();

        for(int x = 0; x < CurrentlySelected.Count; x++)
        {
            if (CurrentlySelected[x].GetSelectableType() == typeToGet)
            {
                retVal.Add(CurrentlySelected[x]);
            }
        }

        return retVal;
    }

    public List<Unit> GetSelectedUnits()
    {
        List<Unit> ret = new List<Unit>();
        List<Selectable> units = GetAllSelectableOfType(SelectableType.Unit);
        Unit u = null;
        for(int x = 0; x < units.Count; x++)
        {
            u = units[x] as Unit;
            if (u == null)
            {
                continue;
            }
            ret.Add(u);
            
        }
        return ret;
    }

    public Dictionary<UnitType,List<Unit>> FilterUnitsByType()
    {
        Dictionary<UnitType, List<Unit>> retVal = new Dictionary<UnitType, List<Unit>>();
        List<Unit> units = GetSelectedUnits();

        for(int x = 0; x < units.Count; x++)
        {
            if (!retVal.ContainsKey(units[x].MyType))
            {
                retVal.Add(units[x].MyType, new List<Unit>());
            }
            retVal[units[x].MyType].Add(units[x]);
        }

        return retVal;
    }

    public void SetOrderValue(string key,bool value)
    {
        Unit toSet = null;
        for (int x = 0; x < CurrentlySelected.Count; x++)
        {
            if (CurrentlySelected[x].GetSelectableType() == SelectableType.Unit)
            {
                toSet = CurrentlySelected[x] as Unit;

                if (toSet != null && toSet.MyOrders!=null)
                {
                    toSet.MyOrders.SetOrder(key,value);
                }
            }
        }
    }

}
