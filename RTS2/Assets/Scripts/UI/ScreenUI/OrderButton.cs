using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class OrderButton : MonoBehaviour
{
    public TextMeshProUGUI NameText;
    public Button Button;
    private void Awake()
    {
        Button.onClick.AddListener(OnButtonClick);
    }

    string key = "";
    bool value = false;
   public void InitButton(string key,bool initValue)
    {
        this.key=  key;
        value = initValue;
        RefreshUIText();
        
    }

    void RefreshUIText()
    {
        NameText.text = key + ":" + value;
    }


    void OnButtonClick()
    {
        value = !value;
        SelectableManager.Instance.SetOrderValue(key,value);
        RefreshUIText();
        OrderUISelect.Instance.RefreshUI();
    }
}
