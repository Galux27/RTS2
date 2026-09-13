using UnityEngine;
using UnityEngine.UI;
public class ButtonBar : MonoBehaviour
{
    public Image BG, Display;

    public void SetBarValue(float cur,float max)
    {
        float width = Mathf.InverseLerp(0f, max, cur);
        Display.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * BG.rectTransform.rect.width);
    }


}
