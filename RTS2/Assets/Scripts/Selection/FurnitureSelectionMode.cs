using System.Collections.Generic;
using UnityEngine;

public class FurnitureSelectionMode : SelectionMode
{
    Constructable ConstructableHoveringOver;

    public override void OnHover()
    {
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);

        //    CursorIcon.Instance.SetWallPlaceIcon();

        if (ConstructableObjectManager.Instance.selectedToConstruct != null)
        {
            CheckForRequiredResources();
            ConstructableObjectManager.Instance.GetCursor().SetActive(true);

            if (AreAllTilesWalkable(coords, ConstructableObjectManager.Instance.selectedToConstruct.HalfWidth,
                ConstructableObjectManager.Instance.selectedToConstruct.HalfHeight) 
                && DoBoundsIntersectExisting(coords, ConstructableObjectManager.Instance.selectedToConstruct.Size()) ==false && hasEnoughResources)
            {
                ConstructableObjectManager.Instance.SetCursorColour(new Color(0, 1, 0, .5f));
            }
            else
            {
                ConstructableObjectManager.Instance.SetCursorColour(new Color(1, 0, 0, .5f));

            }

            ConstructableObjectManager.Instance.GetCursor().transform.position = new Vector3(coords.x , coords.y , 0f);
        }
        else
        {
            ConstructableObjectManager.Instance.GetCursor().SetActive(false);

        }

        Vector2Int v = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(cursorPos + new Vector3(.5f, .5f));
        Constructable ConstructableHoveringOverThisFrame = WorldChunkManager.Instance.Chunks[v.x, v.y].GetConstructableAtPosition(coords.x, coords.y, ConstructableType.Furniture);

        

        if (ConstructableHoveringOver != ConstructableHoveringOverThisFrame)
        {
            if (ConstructableHoveringOver != null)
            {
                ConstructableHoveringOver.OnHoverExit();
            }
            ConstructableHoveringOver = ConstructableHoveringOverThisFrame;

            if (ConstructableHoveringOver != null)
            {
                ConstructableHoveringOver.OnHover();
            }
        }
    }

    public static bool AreAllTilesWalkable(Vector2Int coords,int halfWidth,int halfHeight)
    {
      
        for (int x = coords.x - halfWidth; x < coords.x + halfWidth; x++)
        {
            for (int y = coords.y - halfHeight; y < coords.y + halfHeight; y++)
            {
                if (WorldController.Instance.IsTraversible(x, y) == false)
                {
                    return false;
                }
            }

        }
        return true;
    }
    bool hasEnoughResources = false;
    Dictionary<string, List<FoundResourceData>> resourcesForConstruction=null;
    void CheckForRequiredResources()
    {
        ResourceHelpers.CanMeetResourceRequirements(ConstructableObjectManager.Instance.selectedToConstruct.RequirementsToBuild,
            CursorSelect.Instance.GetMousePosition(), 200f, out hasEnoughResources, ref resourcesForConstruction);

    }

    public override void OnLeftMouseUp()
    {
        if (ConstructableObjectManager.Instance.selectedToConstruct != null && hasEnoughResources)
        {
            Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();

            ConstructableObjectManager.Instance.GetCursor().SetActive(true);
            Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
            
         


            if (AreAllTilesWalkable(coords, ConstructableObjectManager.Instance.selectedToConstruct.HalfWidth, ConstructableObjectManager.Instance.selectedToConstruct.HalfHeight) 
                && DoBoundsIntersectExisting(coords, ConstructableObjectManager.Instance.selectedToConstruct.Size()) ==false
                )
            {
                ConstructableObjectManager.Instance.CreateBuildableForObject(coords, cursorPos,resourcesForConstruction);
            }
        }
    }

    public static bool DoBoundsIntersectExisting(Vector2Int coords,Vector3 size)
    {
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();

        Bounds toBuild = new Bounds(new Vector3(coords.x, coords.y),size*.9f);

        List<Constructable> selectables= SelectionUtilities.GetAllConstructablesInRangeOfObject(cursorPos, 20);
        Bounds comparison = new Bounds();

        for(int x = 0; x < selectables.Count; x++)
        {
            comparison = new Bounds(selectables[x].GetPosition(), selectables[x].Size());
            if (comparison.Intersects(toBuild))
            {
                return true;
            }
          
        }

        return false;
    }

    public static void DrawBounds(Bounds b,Color c )
    {
        
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

        Debug.DrawLine(p1, p2, c);
        Debug.DrawLine(p2, p3, c);
        Debug.DrawLine(p3, p4,c);
        Debug.DrawLine(p4, p1,c);

        // top
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

        Debug.DrawLine(p5, p6, c);
        Debug.DrawLine(p6, p7, c);
        Debug.DrawLine(p7, p8, c);
        Debug.DrawLine(p8, p5, c);

        // sides
        Debug.DrawLine(p1, p5, c);
        Debug.DrawLine(p2, p6, c);
        Debug.DrawLine(p3, p7, c);
        Debug.DrawLine(p4, p8,c);
    }

    public override void OnRightMouseUp()
    {
        if (ConstructableHoveringOver != null)
        {
            Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
            Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);

            Vector2Int v = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(cursorPos + new Vector3(.5f, .5f));
           WallHelpers.GetWallAtCoords(coords.x, coords.y).HasWallUnderConstruction = false;
            WorldChunkManager.Instance.Chunks[v.x, v.y].RemoveConstructable(ConstructableHoveringOver);
        }
    }
}
