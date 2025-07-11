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
                if (!positions.Contains(currentPosition))
                {
                    positions.Add(currentPosition);
                }
            }
        }
        return positions;
    }


    public override void OnLeftMouseUp()
    {
        OnClick(PositionsCurrentlySelected);
        RefreshRooms();
    }


    public override void OnRightMouseUp()
    {
        RefreshRooms();

    }
    List<Vector2Int> PositionsCurrentlySelected=new List<Vector2Int>();
    public override void OnHover()
    {
        int countLastSelected = PositionsCurrentlySelected.Count;
        PositionsCurrentlySelected = GetCurrentPositions();
        if (PositionsCurrentlySelected.Count != countLastSelected)
        {
            RoomDrawrer.Instance.RenderPoints(RoomDrawrer.Instance.DrawingParent, PositionsCurrentlySelected);
        }

        if (RoomManager.Instance.GetRoom() != null)
        {
           RoomManager.Instance.GetRoom().RefreshRoom();
           
        }
    }

    void RefreshRooms()
    {
        RoomDrawrer.Instance.CleanupAllRooms();
        RoomDrawrer.Instance.RenderAllRooms();
    }

    void OnClick(List<Vector2Int> positions)
    {
        if (RoomManager.Instance.GetRoom() == null)
        {
            return;
        }
        Debug.Log("Room: on click " + positions.Count+" current mode "+  CurrentMode.ToString());
        switch (CurrentMode)
        {
            case RoomMode.Expand:
                positions = FilterTilesInRoom(positions);
                Debug.Log("Room: adding tiles to " + RoomManager.Instance.GetRoom().roomName);
                RoomManager.Instance.GetRoom().AddTiles(positions);
                break;
            case RoomMode.Remove:
                positions = FilterTilesNotInRoom(RoomManager.Instance.GetRoom(), positions);
                RoomManager.Instance.GetRoom().RemoveTiles(positions);
                break;
            default:
                break;
        }
        RoomDrawrer.Instance.RenderAllRooms();
        RoomDrawrer.Instance.CleanupRoom(RoomDrawrer.Instance.DrawingParent);
    }


    List<Vector2Int> FilterTilesInRoom(List<Vector2Int> positions)
    {
        List<Vector2Int> retVal = new List<Vector2Int>();
        for(int x = 0; x < positions.Count; x++)
        {
           if(RoomManager.Instance.DoesAnyRoomContainPosition(positions[x]) == false)
            {
                retVal.Add(positions[x]);
            }
        }
        return retVal;
    }

    List<Vector2Int> FilterTilesNotInRoom(Room toCheck,List<Vector2Int> positions)
    {
        List<Vector2Int> retVal = new List<Vector2Int>();
        for (int x = 0; x < positions.Count; x++)
        {
            if (toCheck.tilesInRoom.Contains(positions[x]))
            {
                retVal.Add(positions[x]);
            }
        }
        return retVal;
    }
}

public enum RoomMode
{
    None,
    Expand,
    Remove
}
