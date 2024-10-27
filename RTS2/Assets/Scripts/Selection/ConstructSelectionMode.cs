using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructSelectionMode : SelectionMode
{
    public override void OnHover()
    {

    }

    public override void OnLeftMouseUp()
    {
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        WorldController.Instance.WallManager.AddSingleWall(coords.x, coords.y, WorldController.Instance.BuildingTilemap, WorldController.Instance.WallTest);
    }

    public override void OnRightMouseUp()
    {
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        WorldController.Instance.WallManager.RemoveSingleWall(coords.x, coords.y, WorldController.Instance.BuildingTilemap, WorldController.Instance.WallTest);
    }
}
