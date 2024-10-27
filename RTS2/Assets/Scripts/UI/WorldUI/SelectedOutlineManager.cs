using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedOutlineManager : MonoBehaviour
{
    public GameObject SelectionOutlinePrefab;


    static SelectedOutlineManager instance;
    public static SelectedOutlineManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<SelectedOutlineManager>();
            }
            return instance;
        }
    }

    List<GameObject> freeSelectionOutlinePrefabs = new List<GameObject>(), inUseSelectionOutlines = new List<GameObject>();

    GameObject GetFreeSelectionOutline()
    {
        if(freeSelectionOutlinePrefabs.Count == 0)
        {
            GameObject g = Instantiate(SelectionOutlinePrefab, Vector3.zero, Quaternion.identity);
            freeSelectionOutlinePrefabs.Add(g);
        }
        GameObject retVal = freeSelectionOutlinePrefabs[0];
        freeSelectionOutlinePrefabs.RemoveAt(0);
        retVal.SetActive(true);
        return retVal;
    }

    public void OnSelectObject(GameObject selected)
    {
        GameObject g = GetFreeSelectionOutline();
        g.GetComponent<SelectedOutline>().ApplyToObject(selected);
        inUseSelectionOutlines.Add(g);
    }


    public void OnDeselectObject(GameObject deselected)
    {
        inUseSelectionOutlines.Remove(deselected);
        freeSelectionOutlinePrefabs.Add(deselected);
        deselected.SetActive(false);
    }
}
