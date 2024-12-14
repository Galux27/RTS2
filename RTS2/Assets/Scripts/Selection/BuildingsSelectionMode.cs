using System.Collections.Generic;
using UnityEngine;

public class BuildingsSelectionMode : SelectionMode
{
    Constructable ConstructableHoveringOver;

    public override void OnHover()
    {
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);

        //    CursorIcon.Instance.SetWallPlaceIcon();

        if (ConstructableObjectManager.Instance.selectedToConstruct != null)
        {

            ConstructableObjectManager.Instance.GetCursor().SetActive(true);

            if (AreAllTilesWalkable(coords))
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

    bool AreAllTilesWalkable(Vector2Int coords)
    {
        for(int x=coords.x;x<coords.x+ ConstructableObjectManager.Instance.selectedToConstruct.Width; x++)
        {
            for (int y = coords.y;y < coords.y + ConstructableObjectManager.Instance.selectedToConstruct.Height; y++)
            {
                if(WorldController.Instance.IsTraversible(x, y) == false)
                {
                    return false;
                }
            }

        }
        return true;
    }

    public override void OnLeftMouseUp()
    {
        if (ConstructableObjectManager.Instance.selectedToConstruct != null)
        {
            Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();

            ConstructableObjectManager.Instance.GetCursor().SetActive(true);
            Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
            
            
            
            if (AreAllTilesWalkable(coords)&& DoBoundsIntersectExisting(coords)==false)
            {
                ConstructableObjectManager.Instance.CreateBuildableForObject(coords, cursorPos);
               // ConstructableObjectManager.Instance.CreateObject(coords,cursorPos,constructable);
            }
        }
    }

    bool DoBoundsIntersectExisting(Vector2Int coords)
    {
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();

        Bounds toBuild = new Bounds(new Vector3(coords.x, coords.y), ConstructableObjectManager.Instance.selectedToConstruct.Size());


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

    public override void OnRightMouseUp()
    {
        if (ConstructableHoveringOver != null)
        {
            Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
            Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);

            Vector2Int v = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(cursorPos + new Vector3(.5f, .5f));
            WorldController.Instance.WallManager.WallsInWorld[coords.x, coords.y].HasWallUnderConstruction = false;
            WorldChunkManager.Instance.Chunks[v.x, v.y].RemoveConstructable(ConstructableHoveringOver);
        }
    }
}
