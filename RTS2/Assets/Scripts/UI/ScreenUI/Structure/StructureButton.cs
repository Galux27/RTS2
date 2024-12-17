using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StructureButton : MonoBehaviour
{
    public void InitButton(string text)
    {
        this.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
}
