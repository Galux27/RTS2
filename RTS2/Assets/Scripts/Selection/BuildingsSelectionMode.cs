using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsSelectionMode : SelectionMode
{
    public override void OnHover()
    {

    //    CursorIcon.Instance.SetWallPlaceIcon();

        if (ConstructableObjectManager.Instance.selectedToConstruct != null)
        {
            Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();

            ConstructableObjectManager.Instance.GetCursor().SetActive(true);
            Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);

            if (IsValidToConstruct(coords))
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
    //    Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
    //    Sprite icon = WallHelpers.GetSpriteForWallType(WorldController.Instance.WallManager.WallsInWorld[coords.x, coords.y], WorldController.Instance.WallManager,
    //        WorldController.Instance.WallTest);
    //    CursorIcon.Instance.SetPosition(new Vector3(coords.x + .5f, coords.y + .5f, 0f));
    //    CursorIcon.Instance.SetCustomIcon(icon);
    }

    bool IsValidToConstruct(Vector2Int coords)
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
            
            if (IsValidToConstruct(coords))
            {
                ConstructableObjectManager.Instance.CreateBuildableForObject(coords, cursorPos);
               // ConstructableObjectManager.Instance.CreateObject(coords,cursorPos,constructable);
            }
        }
    }
    public override void OnRightMouseUp()
    {
        //Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        //Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        //WorldController.Instance.WallManager.RemoveSingleWall(coords.x, coords.y, WorldController.Instance.BuildingTilemap, WorldController.Instance.WallTest);
    }
}
