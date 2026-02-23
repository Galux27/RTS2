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
                None();
                break;
            case StructureSelectionType.Walls:
                Walls_OnHover();
                break;
            case StructureSelectionType.Door:
                Doors_OnHover();
                break;
        }

        CheckForRequiredResources();
    }

    public override void OnLeftMouseUp()
    {
        switch (Mode)
        {

            case StructureSelectionType.None:
                None();
                break;
            case StructureSelectionType.Walls:
                Walls_OnLeftClick();
                break;
            case StructureSelectionType.Door:
                Doors_OnLeftClick();
                break;
        }
    }

    public override void OnRightMouseUp()
    {
        switch (Mode)
        {

            case StructureSelectionType.None:
                None();
                break;
            case StructureSelectionType.Walls:
                Walls_OnRightClick();
                break;
            case StructureSelectionType.Door:
                Doors_OnRightClick();
                break;
        }

    }

    bool hasEnoughResources = false;
    Dictionary<string, List<FoundResourceData>> resourcesForConstruction = null;
    void CheckForRequiredResources()
    {
        ResourceHelpers.CanMeetResourceRequirements(WallTypeManager.Instance.SelectedWallTile.RequirementsToBuild,
            CursorSelect.Instance.GetMousePosition(), 200f, out hasEnoughResources, ref resourcesForConstruction);

    }


    void None()
    {
        CursorIcon.Instance.SetVisible(false);
    }

    void Walls_OnHover()
    {
        CursorIcon.Instance.SetVisible(true);

        CursorIcon.Instance.SetWallPlaceIcon();
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        Sprite icon = WallHelpers.GetSpriteForWallType(
            WallHelpers.GetWallAtCoords(coords), WorldController.Instance.WallManager,
        WallTypeManager.Instance.SelectedWallTile);
        CursorIcon.Instance.SetPosition(new Vector3(coords.x + .5f, coords.y + .5f, 0f));
        CursorIcon.Instance.SetCustomIcon(icon);

        Vector2Int v = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(cursorPos + new Vector3(.5f, .5f));

        Constructable ConstructableHoveringOverThisFrame = WorldChunkManager.Instance.GetWorldChunkFromPos(cursorPos).GetConstructableAtPosition(coords.x, coords.y, ConstructableType.Wall);


        if (ValidToPlaceStructure(v))
        {
            CursorIcon.Instance.SetColor(Color.white);
        }
        else
        {
            CursorIcon.Instance.SetColor(Color.red);
        }

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
        if (WallHelpers.CanIPlaceWallAtPosition(coords.x, coords.y) && hasEnoughResources)
        {
            WallHelpers.CreateWallBuildableStructure(coords.x, coords.y, WorldController.Instance.BuildingTilemap, WallTypeManager.Instance.SelectedWallTile, cursorPos, new Vector3(.5f, .5f, 0f));
            ResourceHelpers.ConsumeResources(WallTypeManager.Instance.SelectedWallTile.RequirementsToBuild);
        }
    }

    bool ValidToPlaceStructure(Vector2Int coords)
    {
        return WallHelpers.CanIPlaceWallAtPosition(coords.x, coords.y) && hasEnoughResources;
    }


    void Walls_OnRightClick()
    {
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        if (WallHelpers.DoesConstructedWallExistAtPosition(coords.x, coords.y))
        {
            WorldController.Instance.WallManager.RemoveSingleWall(coords.x, coords.y, WorldController.Instance.BuildingTilemap, WallTypeManager.Instance.SelectedWallTile);

        }

        if (WallHelpers.DoesUnderConstructionWallExistAtPosition(coords.x, coords.y) && ConstructableHoveringOver != null)
        {
            Vector2Int v = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(cursorPos + new Vector3(.5f, .5f));
           WallHelpers.GetWallAtCoords(coords).HasWallUnderConstruction = false;
            WorldChunkManager.Instance.RemoveConstructable(ConstructableHoveringOver);
        }
    }


    void Doors_OnHover()
    {
        CursorIcon.Instance.SetVisible(true);

        CursorIcon.Instance.SetWallPlaceIcon();
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        Sprite icon = WallTypeManager.Instance.DoorIcon;
        CursorIcon.Instance.SetPosition(new Vector3(coords.x + .5f, coords.y + .5f, 0f));
        CursorIcon.Instance.SetCustomIcon(icon);

        Vector2Int v = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(cursorPos + new Vector3(.5f, .5f));

        Constructable ConstructableHoveringOverThisFrame =
            WorldChunkManager.Instance.GetWorldChunkFromPos(cursorPos).GetConstructableAtPosition(coords.x, coords.y, ConstructableType.Wall);
        Debug.Log("Doors: hover " + coords);
        if (ValidToPlaceStructure(coords))
        {
            CursorIcon.Instance.SetColor(Color.white);
        }
        else
        {
            CursorIcon.Instance.SetColor(Color.red);
        }

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

    void Doors_OnLeftClick()
    {
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        if (WallHelpers.CanIPlaceDoorAtPosition(coords.x, coords.y) && hasEnoughResources&& ValidToPlaceStructure(coords))
        {
            WallHelpers.CreateDoorBuildableStructure(coords.x, coords.y, WorldController.Instance.BuildingTilemap, WallTypeManager.Instance.SelectedWallTile, cursorPos, new Vector3(.5f, .5f, 0f));
            ResourceHelpers.ConsumeResources(WallTypeManager.Instance.SelectedWallTile.RequirementsToBuild);

        }
    }

    void Doors_OnRightClick()
    {
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        if (WallHelpers.DoesConstructedDoorExistAtPosition(coords.x, coords.y))
        {
            WorldController.Instance.WallManager.RemoveSingleWall(coords.x, coords.y, WorldController.Instance.BuildingTilemap, WallTypeManager.Instance.SelectedWallTile);

        }

        if (WallHelpers.DoesUnderConstructionDoorExistAtPosition(coords.x, coords.y) && ConstructableHoveringOver != null)
        {
            Vector2Int v = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(cursorPos + new Vector3(.5f, .5f));
            WallHelpers.GetWallAtCoords(coords).HasWallUnderConstruction = false;
            WorldChunkManager.Instance.GetWorldChunkFromPos(cursorPos).RemoveConstructable(ConstructableHoveringOver);
        }
    }

}

public enum StructureSelectionType 
{ 
    None,
    Walls,
    Door
}

