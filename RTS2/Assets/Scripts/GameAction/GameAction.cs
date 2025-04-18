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

    public GameAction(string actionName, Action action)
    {
        ActionName = actionName;
        PerformAction = action;
    }
}
