using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructableObjectUI : MonoBehaviour
{
    public void InitUI(Vector3 size,Vector3 pos)
    {
        this.transform.localScale = size;
        this.transform.position = pos;
    }
}
