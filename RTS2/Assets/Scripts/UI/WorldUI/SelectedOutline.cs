using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelectedOutline : MonoBehaviour
{
    SpriteRenderer sr;
    GameObject parent;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void ApplyToObject(GameObject obj,Vector3 size=default,Vector3 offset=default)
    {
        parent = obj;

        Vector3 scale = Vector3.one;

        if (size == default)
        {
            if (obj.GetComponent<ObjectVisuals>() != null)
            {
                scale = obj.GetComponent<ObjectVisuals>().Size;
            }
            else
            {
                Bounds b = ObjectBoundsGetter.GetBoundsOfObject(obj);

                scale = new Vector3(b.extents.x, b.size.y, 1);
            }
        }
        else
        {
            scale = size;
        }
        this.transform.localScale = scale;

        this.transform.parent = obj.transform;
        this.transform.localPosition = Vector3.zero+offset;
      
    }

    public void OnDeselect()
    {
        this.transform.parent = null;
        parent = null;
       
        SelectedOutlineManager.Instance.OnDeselectObject(this.gameObject);
       
    }
}
