using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class TabButton : MonoBehaviour
{
    public TextMeshProUGUI ActiveText, InactiveText;
    public Button ActiveButton, InactiveButton;
    public bool IsActive = false;
    public Action OnSetActive, OnSetInactive;
    public int index = -1;

    TabButtonParent myParent;
    private void Awake()
    {
        AddAction(false, SetActive);
        AddAction(true, SetInactive);
    }

    public void SetParent(TabButtonParent myParent)
    {
        this.myParent=myParent; 
    }

    public void SetText(string text)
    {
        ActiveText.text=text; 
        InactiveText.text = text;
    }

    public void AddAction(bool forActive,Action onClick)
    {
        if (forActive)
        {
            ActiveButton.onClick.AddListener(onClick.Invoke);
        }
        else
        {
            InactiveButton.onClick.AddListener(onClick.Invoke);
        }
    }

    public void SetActive()
    {
        Debug.Log("Setting tab button active " + this.gameObject.name);
        ActiveButton.gameObject.SetActive(true);
        InactiveButton.gameObject.SetActive(false);
        OnSetActive?.Invoke();
        IsActive = true;
        myParent.OnTabSelected(index, true);

    }

    public void SetInactive()
    {
        ActiveButton.gameObject.SetActive(false);
        InactiveButton.gameObject.SetActive(true);
        OnSetInactive?.Invoke();
        IsActive = false;
    }

   

}
