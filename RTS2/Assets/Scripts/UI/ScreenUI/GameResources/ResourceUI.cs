using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
    public TextMeshProUGUI Quantitiy;
    public Image Icon;
    public void Init(string name,int quantity,Sprite icon,int capacity)
    {
        Quantitiy.text = quantity.ToString() + "/" + capacity;
        Icon.sprite = icon;
    }

    public void UpdateQuantity(int newVal,int capacity)
    {
        Quantitiy.text = newVal.ToString()+"/"+capacity;
    }

    public void UpdateRequirement(int val)
    {
        Quantitiy.text = val.ToString();
    }

    public void SetHasEnoughOfResource(bool val)
    {
        if (!val)
        {
            GetComponent<Image>().color = Color.red;
        }
        else
        {
            GetComponent<Image>().color = new Color(85f/255f, 95f / 255f, 115f / 255f, 1f);

        }
    }
}
