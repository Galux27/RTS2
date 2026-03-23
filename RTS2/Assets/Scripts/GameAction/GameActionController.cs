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
    public bool ShouldDisplay = true;
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

    void LogOutAllActions()
    {
        if (currentValidGameActions != null)
        {
            string val = "";
            for(int x=0;x<currentValidGameActions.Count;x++)
            {
                val += currentValidGameActions[x].ActionName + ",";
            }
            Debug.Log("Actions: " + val);
        }
    }

    public bool DoesActionListContainMoveAction()
    {
        if (currentValidGameActions != null)
        {
            for (int x = 0; x < currentValidGameActions.Count; x++)
            {
                if (currentValidGameActions[x].ActionName == "Move")
                {
                    return true;
                }
            }
        }
        return false;
    }

    void ForceToDoNonMoveAction()
    {
        for(int x = 0; x < currentValidGameActions.Count; x++)
        {
            if (currentValidGameActions[x].ActionName != "Move")
            {
                currentValidGameActions[x].PerformAction?.Invoke();
                OnActionPerformed();
            }
        }
    }

    void ForceMoveAction()
    {
        for (int x = 0; x < currentValidGameActions.Count; x++)
        {
            if (currentValidGameActions[x].ActionName == "Move")
            {
                currentValidGameActions[x].PerformAction?.Invoke();
                OnActionPerformed();
            }
        }
    }

    public void OnManualInput()
    {
        if (currentValidGameActions != null)
        {
            if (ActionSelectMenu.Instance.IsDisplaying() == false)
            {
                {
                    if (currentValidGameActions.Count == 1)
                    {
                        currentValidGameActions[0].PerformAction?.Invoke();
                        OnActionPerformed();
                    }
                    else if (currentValidGameActions.Count == 2)
                    {
                        bool val = false;
                        if (InputController.Instance.IsPressingRightMouse(out val))
                        {
                            if (InputController.Instance.WasLastRightClickDoubleClick)
                            {
                                ForceMoveAction();
                            }
                            else
                            {
                                ForceToDoNonMoveAction();
                            }
                        }
                    }
                    else if (currentValidGameActions.Count > 2)
                    {
                        ActionSelectMenu.Instance.CreateButtonsForActions(currentValidGameActions);
                        ActionSelectMenu.Instance.ShowUI();
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
        if (InputController.Instance.IsHoldingShortcutButton())
        {
            for(int x = 0; x < currentValidGameActions.Count; x++)
            {
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
        SelectionController.Instance.blockInputTimer =InputController.BlockInputLength;

    }

    public void AddAction(GameAction action)
    {
        currentValidGameActions.Add(action);
    }
}
