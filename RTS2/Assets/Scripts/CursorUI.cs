using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorUI : MonoBehaviour
{
    static CursorUI instance;
    public static CursorUI Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<CursorUI>();
            }
            return instance;
        }
    }


    SpriteRenderer spriteRenderer;
    bool shouldRender = false;
    private void Awake()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
    public void SetShouldRender(bool shouldRender)
    {
        this.shouldRender = shouldRender;
    }

    private void Update()
    {
        if (shouldRender)
        {
            spriteRenderer.color= new Color(1,1,1,.2f); 
        }
        else
        {
            spriteRenderer.color = Color.clear;
            this.transform.localScale = Vector3.zero;
        }
    }

    public void SetCorners(Vector3 pos1,Vector3 pos2)
    {
        this.transform.position = Vector3.Lerp(pos1, pos2, .5f);
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
        Vector3 size = high - low;
        this.transform.localScale = size;
       // this.transform.GetChild(0).localScale = size;
    }
}
