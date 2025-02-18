using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ResourceUI : MonoBehaviour
{
    public TextMeshProUGUI Name, Quantitiy;
    public Image Icon;
    public void Init(string name,int quantity,Sprite icon)
    {
        //Name.text = name;
        Quantitiy.text = quantity.ToString();
        Icon.sprite = icon;
    }

    public void UpdateQuantity(int newVal)
    {
        Quantitiy.text = newVal.ToString();
    }
}
