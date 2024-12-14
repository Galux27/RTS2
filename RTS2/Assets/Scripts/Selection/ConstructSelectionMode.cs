using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructSelectionMode : SelectionMode
{
    Constructable ConstructableHoveringOver;


    public override void OnHover()
    {

        CursorIcon.Instance.SetWallPlaceIcon();
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        Sprite icon = WallHelpers.GetSpriteForWallType(WorldController.Instance.WallManager.WallsInWorld[coords.x, coords.y], WorldController.Instance.WallManager,
            WorldController.Instance.WallTest);
        CursorIcon.Instance.SetPosition(new Vector3(coords.x + .5f, coords.y + .5f, 0f));
        CursorIcon.Instance.SetCustomIcon(icon);

        Vector2Int v = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(cursorPos + new Vector3(.5f, .5f));

        Constructable ConstructableHoveringOverThisFrame = WorldChunkManager.Instance.Chunks[v.x, v.y].GetConstructableAtPosition(coords.x,coords.y);
        
        
        
        if(ConstructableHoveringOver!= ConstructableHoveringOverThisFrame)
        {
            if (ConstructableHoveringOver != null) {
                ConstructableHoveringOver.OnHoverExit();
            }
            ConstructableHoveringOver = ConstructableHoveringOverThisFrame;

            if (ConstructableHoveringOver != null)
            {
                ConstructableHoveringOver.OnHover();
            }
        }
       
    }

    public override void OnLeftMouseUp()
    {
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        if (WallHelpers.CanIPlaceWallAtPosition(coords.x, coords.y))
        {
            WallHelpers.CreateWallBuildableStructure(coords.x, coords.y, WorldController.Instance.BuildingTilemap, WorldController.Instance.WallTest, cursorPos, new Vector3(.5f, .5f, 0f));
        }
    }

    public override void OnRightMouseUp()
    {
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        if (WallHelpers.DoesConstructedWallExistAtPosition(coords.x, coords.y))
        {
            WorldController.Instance.WallManager.RemoveSingleWall(coords.x, coords.y, WorldController.Instance.BuildingTilemap, WorldController.Instance.WallTest);

        }

        if (WallHelpers.DoesUnderConstructionWallExistAtPosition(coords.x, coords.y) && ConstructableHoveringOver!=null)
        {
            Vector2Int v= WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(cursorPos+new Vector3(.5f,.5f));
            WorldController.Instance.WallManager.WallsInWorld[coords.x, coords.y].HasWallUnderConstruction = false;
            WorldChunkManager.Instance.Chunks[v.x, v.y].RemoveConstructable(ConstructableHoveringOver);
        }

    }
}
