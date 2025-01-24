using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that all game actions are passed to so they can be stored & drawn in one place
/// </summary>
public class GameActionController : MonoBehaviour
{
    static GameActionController instance;
    public static GameActionController Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<GameActionController>(true);
            }
            return instance;
        }
    }

  

    public List<GameAction> currentValidGameActions=new List<GameAction>();

    private void Update()
    {
        if (currentValidGameActions != null && ActionSelectMenu.Instance.IsDisplaying() == false)
        {
            if (currentValidGameActions.Count > 1)
            {
                ActionSelectMenu.Instance.CreateButtonsForActions(currentValidGameActions);
                ActionSelectMenu.Instance.ShowUI();
            }
            else if (currentValidGameActions.Count == 1)
            {
                currentValidGameActions[0].PerformAction?.Invoke();
                OnActionPerformed();
            }
        }
    }
    public void OnActionPerformed()
    {
        currentValidGameActions.Clear();
        ActionSelectMenu.Instance.CloseMenu();
        SelectionController.Instance.blockInputTimer = .2f;

    }

    public void AddAction(GameAction action)
    {
        currentValidGameActions.Add(action);
    }
}
