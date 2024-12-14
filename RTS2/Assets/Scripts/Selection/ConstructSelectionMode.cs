using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructSelectionMode : SelectionMode
{
    public override void OnHover()
    {
       
        CursorIcon.Instance.SetWallPlaceIcon();
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        Sprite icon = WallHelpers.GetSpriteForWallType(WorldController.Instance.WallManager.WallsInWorld[coords.x, coords.y], WorldController.Instance.WallManager,
            WorldController.Instance.WallTest);
        CursorIcon.Instance.SetPosition(new Vector3(coords.x+.5f,coords.y+.5f,0f));
        CursorIcon.Instance.SetCustomIcon(icon);
    }

    public override void OnLeftMouseUp()
    {
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        if (WallHelpers.CanIPlaceWallAtPosition(coords.x, coords.y, cursorPos))
        {
            WallHelpers.CreateWallBuildableStructure(coords.x, coords.y, WorldController.Instance.BuildingTilemap, WorldController.Instance.WallTest, cursorPos, new Vector3(.5f, .5f, 0f));
        }
    }

    public override void OnRightMouseUp()
    {
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        WorldController.Instance.WallManager.RemoveSingleWall(coords.x, coords.y, WorldController.Instance.BuildingTilemap, WorldController.Instance.WallTest);
    }
}
