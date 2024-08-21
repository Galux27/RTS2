using System.Collections;
using System.Collections.Generic;
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

    public List<Selectable> CurrentlySelected=new List<Selectable>();

    public void AddSelectable(Selectable toAdd)
    {
        if (toAdd.GetIsSelected() == false)
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
    }

    public void RemoveSelectable(Selectable toRemove)
    {
        if (toRemove.GetIsSelected()==true)
        {
            CurrentlySelected.Remove(toRemove);
            toRemove.SetIsSelected(false);
        }
    
    }
}
