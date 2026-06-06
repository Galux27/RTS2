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

    public SpriteRenderer RectSpriteRenderer;
    public Color CursorColour;
   
    public void SetShouldRender(bool shouldRender)
    {
        if (shouldRender)
        {
            RectSpriteRenderer.color=CursorColour;
        }
        else
        {
            RectSpriteRenderer.color = Color.clear;
        }
    }

    private void Update()
    {
        
    }
    public Vector3 low, high;
    public void SetCorners(Vector3 pos1,Vector3 pos2)
    {
        this.transform.position = Vector3.Lerp(pos1, pos2, .5f);
        high = new Vector3();
        low = new Vector3();
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

    public void DrawTileAccessable(Vector2Int coords,int width,int height)
    {
     
        Vector3 pos = new Vector3();
        for (int x = coords.x; x < coords.x + width; x++)
        {
            for (int y = coords.y; y < coords.y + height; y++)
            {
                pos.x = x;
                pos.y = y;
                if (WorldController.Instance.IsTraversible(x, y) == false)
                {
                    Debug.DrawLine(pos, pos + (Vector3.one * .5f), Color.red);
                }
                else
                {
                    Debug.DrawLine(pos, pos + (Vector3.one * .5f), Color.green);

                }
            }

        }
    }
}
