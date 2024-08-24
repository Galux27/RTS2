using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedOutline : MonoBehaviour
{
    SpriteRenderer sr;
    GameObject parent;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void ApplyToObject(GameObject obj)
    {
        parent = obj;
        this.transform.parent = obj.transform;
        this.transform.localPosition = Vector3.zero;
        this.transform.localScale = obj.GetComponent<SpriteRenderer>().bounds.size;
      
    }

    public void OnDeselect()
    {
        this.transform.parent = null;
        parent = null;
        SelectedOutlineManager.Instance.OnDeselectObject(this.gameObject);
    }
}
