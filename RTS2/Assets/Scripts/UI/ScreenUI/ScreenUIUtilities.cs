using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class ScreenUIUtilities 
{

    public static bool IsCursorOverUI()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    private static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
            {

                if (IsUIElementHitActive(curRaysastResult.gameObject))
                {
                    return true;
                }
                
            }
        }
            return false;
    }

    static bool IsUIElementHitActive(GameObject g)
    {
        ActionSelectMenu actionSelect= g.GetComponentInChildren<ActionSelectMenu>();
        if (actionSelect != null)
        {
            if (actionSelect.GetComponent<CanvasGroup>().alpha == 0f)
            {
                return false;
            }
        }
        return true;
    }

   static string getFullPathToObject(GameObject g)
    {
        string retVal = g.name;
        while (g.transform.parent != null)
        {
            retVal += "/" + g.name;
            g = g.transform.parent.gameObject;
        }
        return retVal;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

}
