using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to help with converting mouse clicks to world positions for use when selecting objects and giving orders
/// </summary>
public class CursorSelect : MonoBehaviour
{

    static CursorSelect instance;
    public static CursorSelect Instance
    {
        get
        {
            if(instance== null)
            {
                instance=FindObjectOfType<CursorSelect>();

            }
            return instance;
        }
    }
    public Camera Camera;
    public LayerMask CursorLayermask;
    public LayerMask UnitLayermask;
    private void Awake()
    {
        Camera = GetComponent<Camera>();
    }

    Vector3 GetMousePosition()
    {
        if (GotPositionThisFrame)
        {
            return cachedPosition;
        }
        Ray r = Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(r.origin, r.direction, 999f, CursorLayermask);
        if (hit.collider != null)
        {
            GotPositionThisFrame = true;
            cachedPosition = hit.point;
            return cachedPosition;
        }
        else
        {
            return Vector3.zero;
        }
    }


    public Vector2 startPoint,endPoint;
    Vector3 cachedPosition;
    bool mouseDown = false,GotPositionThisFrame=false;
    // Update is called once per frame
    public void UpdateSelectionPoints()
    {
        GotPositionThisFrame = false;
        if(Input.GetMouseButtonDown(0))
        {
            startPoint = GetMousePosition() ;
            mouseDown = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            endPoint = GetMousePosition();
            mouseDown = false;
        }
        if (Input.GetMouseButton(0) && mouseDown)
        {
           Vector3 pos = GetMousePosition();
            if (pos!=Vector3.zero)
            {
                endPoint = pos;
                CursorUI.Instance.SetCorners(startPoint, endPoint);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
        }
        CursorUI.Instance.SetShouldRender(mouseDown);
        CursorIcon.Instance.SetPosition(GetMousePosition());
        CursorIcon.Instance.SetVisible(!mouseDown);
    }
}
