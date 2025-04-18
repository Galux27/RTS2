using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
        transform.localScale = Vector3.one;
        Vector3 startPos = Input.mousePosition / GetComponentInParent<Canvas>().scaleFactor;
        GetComponent<RectTransform>().anchoredPosition = startPos;
        this.gameObject.SetActive(true);

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
        g.GetComponentInChildren<TextMeshProUGUI>().text = action.ActionName;
    }

    public void CloseMenu()
    {
        Cleanup();
        cg.alpha = 0f;
        transform.localScale = Vector3.zero;
        this.gameObject.SetActive(false);
    }
}
