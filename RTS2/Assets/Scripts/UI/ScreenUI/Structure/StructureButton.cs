using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class StructureButton : MonoBehaviour
{
    public GameObject TypeScrollView,ScrollViewContent;

    private void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(() => TypeScrollView.SetActive(true));
    }


    public void InitButton(string text)
    {
        this.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    public void AddType(string name,Action onClick)
    {
        GameObject button = GameObject.Instantiate(ScrollViewContent.transform.GetChild(0).gameObject,ScrollViewContent.transform);
        Button b = button.GetComponent<Button>();
        b.GetComponentInChildren<TextMeshProUGUI>().text = name;
        b.onClick.AddListener(onClick.Invoke);
        b.onClick.AddListener(() => TypeScrollView.SetActive(false));
        b.onClick.AddListener(()=>this.GetComponentInChildren<TextMeshProUGUI>().text=name);
        button.SetActive(true);
    }
}
