using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ConstructableSelectButton : MonoBehaviour
{
    public Button MyButton;
    public TextMeshProUGUI Text;


    private void Awake()
    {
        MyButton.onClick.AddListener(OnClick);
    }

    public void InitButton(string id)
    {
        idOfConstructable = id;
        Text.text = id;
    }


    string idOfConstructable;

   void OnClick()
   {
        if (ConstructableObjectManager.Instance.AllObjects.ContainsKey(idOfConstructable))
        {
            ConstructableObjectManager.Instance.SetCursorObject( idOfConstructable);
        }
    }
}
