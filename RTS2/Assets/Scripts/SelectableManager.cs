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

    public List<Selectable> CurrentlySelected=new List<Selectable>();

    public void AddSelectable(Selectable toAdd)
    {
        Debug.Log("Added selectable is selected " + toAdd.GetIsSelected());
        if (toAdd.GetIsSelected() == false)
        {
            Debug.Log("Setting objects to selected ");
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
