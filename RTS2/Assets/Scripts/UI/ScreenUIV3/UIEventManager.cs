using UnityEngine;
using System;
using System.Collections.Generic;
public static class UIEventManager
{
    public static Action<Selectable> OnObjectSelected,OnObjectDeselected;
    public static Action<List< Selectable>> OnObjectsSelected;

    public static Action<string, bool> SetUnitsToFollowOrder;

    public static Action<string> OnConstructableObjectSelected;

    public static Action<WallTile> OnWallTileSelected;

    public static Action<CurrentSelectionMode> OnSwitchSelectionMode;

    public static Action<Room> OnRoomEdited;


}
