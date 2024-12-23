using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomsSelectionMode : SelectionMode
{
    public static RoomMode CurrentMode;

    List<Vector2Int> GetCurrentPositions()
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        Vector3 start = CursorUI.Instance.low;
        Vector3 end = CursorUI.Instance.high;
        Vector2Int currentPosition = new Vector2Int();
        for (float x = start.x; x < end.x; x += 1f)
        {
            for (float y = start.y; y < end.y; y += 1f)
            {
                currentPosition.x = Mathf.FloorToInt(x + .5f);
                currentPosition.y = Mathf.FloorToInt(y + .5f);
                positions.Add(currentPosition);
            }
        }
        return positions;
    }


    public override void OnLeftMouseUp()
    {
        OnClick(PositionsCurrentlySelected);
    }


    public override void OnRightMouseUp()
    {
      
    }
    List<Vector2Int> PositionsCurrentlySelected;
    public override void OnHover()
    {
        PositionsCurrentlySelected = GetCurrentPositions();
        RoomDrawrer.Instance.SetCoords(PositionsCurrentlySelected);
        RoomDrawrer.Instance.RenderRoom();
    }

    void OnClick(List<Vector2Int> positions)
    {
        Debug.Log("Room: on click " + positions.Count);
        switch (CurrentMode)
        {
            case RoomMode.Expand:
                RoomManager.Instance.SelectedRoom.AddTiles(positions);
                break;
            case RoomMode.Remove:
                RoomManager.Instance.SelectedRoom.RemoveTiles(positions);
                break;
            default:
                break;
        }
    }
}

public enum RoomMode
{
    None,
    Expand,
    Remove
}
