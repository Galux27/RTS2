using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedObjectUIElement : MonoBehaviour
{
    public Button SelectObject, GoToObject;
    public TextMeshProUGUI Name, Quantitiy;
    public Image icon;
    public void SetupButton(ObjectInfo selected, int quantity, Action onSelect, Sprite icon = null)
    {
        this.icon.sprite = icon;
        Name.text = selected.Name() ;
        Quantitiy.text = quantity.ToString();
        SelectObject.onClick.AddListener(() => onSelect.Invoke());
        if (quantity > 1)
        {
            GoToObject.gameObject.SetActive(false);
        }else if (quantity == 1)
        {
            GoToObject.gameObject.SetActive(true);
            //GoToObject.onClick.AddListener()
        }
    }
}
