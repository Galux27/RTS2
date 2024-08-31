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
        Bounds b = ObjectBoundsGetter.GetBoundsOfObject(obj);

        this.transform.localScale = new Vector3(b.extents.x, b.size.y, 1);

        this.transform.parent = obj.transform;
        this.transform.localPosition = Vector3.zero;
      
    }

    public void OnDeselect()
    {
        this.transform.parent = null;
        parent = null;
        SelectedOutlineManager.Instance.OnDeselectObject(this.gameObject);
    }
}
