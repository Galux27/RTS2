using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSelect : MonoBehaviour
{

    Camera Camera;
    public LayerMask CursorLayermask;
    private void Awake()
    {
        Camera = GetComponent<Camera>();
    }


    public Vector2 startPoint,endPoint;
    bool mouseDown = false;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray r = Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(r.origin,r.direction,999f,CursorLayermask);
            if (hit.collider != null)
            {
               startPoint=hit.point;
                mouseDown = true;
            }
            else
            {
                
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Ray r = Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(r.origin, r.direction, 999f, CursorLayermask);
            if (hit.collider != null)
            {
                endPoint = hit.point;
            }
            else
            {

            }

            mouseDown = false;
        }
        if (Input.GetMouseButton(0) && mouseDown)
        {
            Ray r = Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(r.origin, r.direction, 999f, CursorLayermask);
            if (hit.collider != null)
            {
                endPoint = hit.point;
                CursorUI.Instance.SetCorners(startPoint, endPoint);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnSelection();
            mouseDown = false;
        }
        CursorUI.Instance.SetShouldRender(mouseDown);

    }


    void OnSelection()
    {
        SelectableManager.Instance.ClearSelectables();
        List<Unit> selected = UnitMoniter.Instance.GetUnitsWithinBounds(startPoint, endPoint);
        Debug.Log("Selected unit count " + selected.Count);
        for(int x=0;x<selected.Count; x++)
        {
            SelectableManager.Instance.AddSelectable(selected[x]);
        }
    }
}
