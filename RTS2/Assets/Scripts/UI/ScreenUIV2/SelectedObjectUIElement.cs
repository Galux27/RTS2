using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;
public class SelectedObjectUIElement : MonoBehaviour
{
    public Button SelectObject, GoToObject;
    public TextMeshProUGUI Name, Quantitiy;
    public Image icon;

    public void SetupButton(ObjectInfo selected, int quantity, Action onSelect, Sprite icon = null)
    {
        GetComponentInChildren<ButtonManagerBasic>().buttonText = selected.Name();

        this.icon.sprite = icon;
        Name.text = selected.Name();
        Quantitiy.text = quantity.ToString();
        SelectObject.onClick.AddListener(() => onSelect.Invoke());

        if (quantity > 1)
        {
            GoToObject.gameObject.SetActive(false);
        }else if (quantity == 1)
        {
            GoToObject.gameObject.SetActive(true);
            GoToObject.onClick.AddListener(() => AutoMoveToUnit(selected));
        }
    }

    public void SetupButton(string name, int quantity, Action onSelect, Sprite icon = null)
    {
        GetComponentInChildren<ButtonManagerBasic>().buttonText = name;

        this.icon.sprite = icon;
        Name.text = name;
        Quantitiy.text = quantity.ToString();
        SelectObject.onClick.AddListener(() => onSelect.Invoke());

        if (quantity > 1)
        {
            GoToObject.gameObject.SetActive(false);
        }
        else if (quantity == 1)
        {
            GoToObject.gameObject.SetActive(true);
           // GoToObject.onClick.AddListener(() => AutoMoveToUnit(selected));
        }
    }


    void AutoMoveToUnit(ObjectInfo selected)
    {
        CameraController.Instance.SetToAutoMove(selected.Position());
        SelectionController.Instance.blockInputTimer = InputController.BlockInputLength;

    }

}
