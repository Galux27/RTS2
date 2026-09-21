using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SelectedOutlineManager : MonoBehaviour
{
    const string SelectionOutlinePool = "SelectionOutline";

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

    private void Awake()
    {
        UIEventManager.OnObjectDeselected += OnDeselected;
        UIEventManager.OnObjectSelected+= OnSelected;
    }

    Dictionary<Selectable, GameObject> Outlines = new Dictionary<Selectable, GameObject>();
    void OnSelected(Selectable selected)
    {
        GameObject g = selected.GetGameObject();
       
        if (g != null) {
            GameObject outline = GetFreeSelectionOutline();
            outline.gameObject.SetActive(true);
            outline.transform.parent = g.transform;
            outline.GetComponent<SelectedOutline>().ApplyToObject(g, selected.GetSize(), selected.GetCenterOffset());
            Outlines.Add(selected, outline);
        }
        else
        {
            Outlines.Add(selected, OnWallSelected(selected as WallSegment, selected.GetSize(), selected.GetCenterOffset()));
        }
    }

    void OnDeselected(Selectable selected)
    {

        if (Outlines.ContainsKey(selected))
        {
            Outlines[selected].SetActive(false);
            GameObjectPoolManager.Instance.ReturnObjectToPool(Outlines[selected], SelectionOutlinePool);
            Outlines.Remove(selected);
        }
    }

    List<GameObject> freeSelectionOutlinePrefabs = new List<GameObject>(), inUseSelectionOutlines = new List<GameObject>();

    GameObject GetFreeSelectionOutline()
    {
        return GameObjectPoolManager.Instance.GetObjectFromPool(SelectionOutlinePool);
    }

   

    public GameObject OnWallSelected(WallSegment wall, Vector3 size = default, Vector3 offset = default)
    {
        GameObject g = GetFreeSelectionOutline();
        g.GetComponent<SelectedOutline>().ApplyToWall(wall, size, offset);
        g.SetActive(true);
        inUseSelectionOutlines.Add(g);
        return g;
    }

    public void OnDeselectObject(GameObject deselected)
    {
        inUseSelectionOutlines.Remove(deselected);
        freeSelectionOutlinePrefabs.Add(deselected);
        deselected.SetActive(false);
    }
}
