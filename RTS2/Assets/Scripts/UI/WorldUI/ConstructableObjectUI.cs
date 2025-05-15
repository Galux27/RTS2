using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructableObjectUI : MonoBehaviour
{
    SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        Debug.Log("Construction UI enabled");

    }

    private void OnDisable()
    {
        Debug.Log("Construction UI disabled");
    }
    public void InitUI(Vector3 size,Vector3 pos)
    {
        this.transform.localScale = size;
        this.transform.position = pos;
    }

    public void SetSpriteRendererColour(Color c)
    {
        sr.color = c;
    }
}
