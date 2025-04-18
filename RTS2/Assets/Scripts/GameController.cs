using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
  static GameController instance;
    public static GameController Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindAnyObjectByType<GameController>();
            }
                return instance; 
        }
    }

    public Action OnUpdate;
    private void Update()
    {
        OnUpdate?.Invoke();
    }
}
