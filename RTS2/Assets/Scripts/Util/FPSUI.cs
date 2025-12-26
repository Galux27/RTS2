using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FPSUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    int fps = 0;
    void Update()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        text.text = fps.ToString("0.00") + "fps";
    }
}
