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

    private void Awake()
    {
        SelectableManager.OnSelectionChanged += OnSelectionChanged;
    }

    public List<GameAction> currentValidGameActions=new List<GameAction>();
    public bool ShouldDisplay = false;
    private void Update()
    {
        if (currentValidGameActions != null)
        {
            CheckForShortcut();  
        }
        else
        {
            if (ActionSelectMenu.Instance.IsDisplaying())
            {
                ActionSelectMenu.Instance.CloseMenu();
            }
        }
    }


    public void OnManualInput()
    {
        if (currentValidGameActions != null)
        {
            if (ActionSelectMenu.Instance.IsDisplaying() == false)
            {
                if (ShouldDisplay)
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
            else
            {
                if (!ShouldDisplay)
                {
                    if (ActionSelectMenu.Instance.IsDisplaying())
                    {
                        ActionSelectMenu.Instance.CloseMenu();
                    }
                }
            }
        }
        else
        {
            if (ActionSelectMenu.Instance.IsDisplaying())
            {
                ActionSelectMenu.Instance.CloseMenu();
            }
        }
    }

    void CheckForShortcut()
    {
        Debug.Log("Shortcut check: " + currentValidGameActions.Count + " " + InputController.Instance.IsHoldingShortcutButton());
        if (InputController.Instance.IsHoldingShortcutButton())
        {
            for(int x = 0; x < currentValidGameActions.Count; x++)
            {
                Debug.Log("Shortcut check: " + currentValidGameActions[x].ActionName + " " + currentValidGameActions[x].Shortcut.ToString());
                if (InputController.Instance.IsHoldingKey(currentValidGameActions[x].Shortcut))
                {
                    currentValidGameActions[0].PerformAction?.Invoke();
                    OnActionPerformed();
                    return;
                }
            }
        }
    }

    void OnSelectionChanged()
    {
        currentValidGameActions.Clear();
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
