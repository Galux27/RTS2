using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.ModernUIPack;

public class ActionSelectMenu : MonoBehaviour
{
    static ActionSelectMenu instance;
    public static ActionSelectMenu Instance
    {
        get
        {
            if(instance== null)
            {
                instance = FindObjectOfType<ActionSelectMenu>(true);
            }
            return instance;
        }
        
    }

    public GameObject ButtonPrefab;
    public Transform ButtonParent;
    CanvasGroup cg;
    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }
    void Cleanup()
    {
        for(int x=0;x<ButtonParent.transform.childCount;x++)
        {
            GameObject.Destroy(ButtonParent.transform.GetChild(x).gameObject);  
        }
    }
    public bool IsDisplaying()
    {
        return cg.alpha > 0;
    }

    private void Update()
    {
        if(SelectableManager.Instance.CurrentSelectedType!=SelectableType.Unit && IsDisplaying())
        {
            CloseMenu();
        }
    }

    public void ShowUI()
    {
        cg.alpha = 1f;
        cg.blocksRaycasts = true;
        cg.interactable = true;
        transform.localScale = Vector3.one;
        Vector3 startPos = Input.mousePosition / GetComponentInParent<Canvas>().scaleFactor;
        GetComponent<RectTransform>().anchoredPosition =  ClampPosToScreen( startPos);
        this.gameObject.SetActive(true);

    }

    Vector2 ClampPosToScreen(Vector2 pos)
    {
        if (pos.x < 200)
        {
            pos.x = 200;
        }else if(pos.x>Screen.width-200)
        {
            pos.x = Screen.width-200;
        }

        if (pos.y < 200)
        {
            pos.y = 200;
        }
        else if (pos.y > Screen.height - 200)
        {
            pos.y = Screen.height - 200;
        }

        return pos;
    }

    public void CreateButtonsForActions(List<GameAction> potentialActions)
    {
        Cleanup();
        for(int x=0;x<potentialActions.Count;x++)
        {
            CreateButton(potentialActions[x]);
        }
    }

    void CreateButton(GameAction action)
    {
        GameObject g = GameObject.Instantiate(ButtonPrefab, ButtonParent);
        Button b = g.GetComponent<Button>();
       
       b.onClick.AddListener(action.PerformAction.Invoke);
        b.onClick.AddListener(GameActionController.Instance.OnActionPerformed);
        b.onClick.AddListener(()=>Debug.Log("Action: button click " + action.ActionName));
        g.GetComponentInChildren<ButtonManagerBasic>().buttonText = action.ActionName;

    }

    public void CloseMenu()
    {
        Cleanup();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;

        transform.localScale = Vector3.zero;
        this.gameObject.SetActive(false);
    }
}
