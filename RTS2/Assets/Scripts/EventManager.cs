using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// Class for storing events that can be used in the game
/// </summary>
public class EventManager : MonoBehaviour
{
    static EventManager instance;
    public static EventManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EventManager>(true);
            }
            return instance;
        }
    }

    public Action<Vector2Int, ConstructableObjectInstance> OnConstructableObjectCreated;
}
