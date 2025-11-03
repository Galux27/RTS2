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
        //Name.text = name;
        Quantitiy.text = quantity.ToString() + "/" + capacity;
        Icon.sprite = icon;
    }

    public void UpdateQuantity(int newVal,int capacity)
    {
        Quantitiy.text = newVal.ToString()+"/"+capacity;
    }
}
