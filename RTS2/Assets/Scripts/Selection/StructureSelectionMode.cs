using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureSelectionMode: SelectionMode
{
    Constructable ConstructableHoveringOver;

    public static StructureSelectionType Mode;
    public override void OnHover()
    {

       switch (Mode) {

            case StructureSelectionType.None:
                break;
            case StructureSelectionType.Walls:
                Walls_OnHover();
                break;
            case StructureSelectionType.Door:
                break;
        }

       
    }

    public override void OnLeftMouseUp()
    {
        switch (Mode)
        {

            case StructureSelectionType.None:
                break;
            case StructureSelectionType.Walls:
                Walls_OnLeftClick();
                break;
            case StructureSelectionType.Door:
                break;
        }
    }

    public override void OnRightMouseUp()
    {
        switch (Mode)
        {

            case StructureSelectionType.None:
                break;
            case StructureSelectionType.Walls:
                Walls_OnRightClick();
                break;
            case StructureSelectionType.Door:
                break;
        }

    }


    void Walls_OnHover()
    {
        CursorIcon.Instance.SetWallPlaceIcon();
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        Sprite icon = WallHelpers.GetSpriteForWallType(WorldController.Instance.WallManager.WallsInWorld[coords.x, coords.y], WorldController.Instance.WallManager,
            WorldController.Instance.WallTest);
        CursorIcon.Instance.SetPosition(new Vector3(coords.x + .5f, coords.y + .5f, 0f));
        CursorIcon.Instance.SetCustomIcon(icon);

        Vector2Int v = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(cursorPos + new Vector3(.5f, .5f));

        Constructable ConstructableHoveringOverThisFrame = WorldChunkManager.Instance.Chunks[v.x, v.y].GetConstructableAtPosition(coords.x, coords.y, ConstructableType.Wall);



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

    void Walls_OnLeftClick()
    {
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        if (WallHelpers.CanIPlaceWallAtPosition(coords.x, coords.y))
        {
            WallHelpers.CreateWallBuildableStructure(coords.x, coords.y, WorldController.Instance.BuildingTilemap, WorldController.Instance.WallTest, cursorPos, new Vector3(.5f, .5f, 0f));
        }
    }

    void Walls_OnRightClick()
    {
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        if (WallHelpers.DoesConstructedWallExistAtPosition(coords.x, coords.y))
        {
            WorldController.Instance.WallManager.RemoveSingleWall(coords.x, coords.y, WorldController.Instance.BuildingTilemap, WorldController.Instance.WallTest);

        }

        if (WallHelpers.DoesUnderConstructionWallExistAtPosition(coords.x, coords.y) && ConstructableHoveringOver != null)
        {
            Vector2Int v = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(cursorPos + new Vector3(.5f, .5f));
            WorldController.Instance.WallManager.WallsInWorld[coords.x, coords.y].HasWallUnderConstruction = false;
            WorldChunkManager.Instance.Chunks[v.x, v.y].RemoveConstructable(ConstructableHoveringOver);
        }
    }
}

public enum StructureSelectionType 
{ 
    None,
    Walls,
    Door
}

