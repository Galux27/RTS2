using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// Class that represents an action that a player could perform e.g. order unit to move
/// </summary>
public class GameAction
{
    public string ActionName;
    public Action PerformAction;
    public KeyCode Shortcut;
    public GameAction(string actionName, Action action,KeyCode shortcut)
    {
        ActionName = actionName;
        PerformAction = action;
        Shortcut = shortcut;
    }
}
