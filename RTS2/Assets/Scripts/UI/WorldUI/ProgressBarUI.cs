using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarUI : MonoBehaviour
{
    public static ProgressBarUI CreateProgressBar()
    {
        return GameObjectPoolManager.Instance.GetObjectFromPool("ProgressBar").GetComponent<ProgressBarUI>();
    }

    

    const float MaxValWidth = 100f;
    [SerializeField] GameObject Bar;
    public float MaxValue,CurrentValue;
    public void InitProgressBar(float max,float cur,Vector3 position)
    {
        MaxValue= max;
        CurrentValue= cur;
        this.transform.position= position;
        this.gameObject.SetActive(true);
    }
    public void UpdateCurrent(float cur)
    {
        CurrentValue = cur;

        UpdateProgress();
    }

    public void UpdateProgress()
    {
        RectTransform rectTransform = Bar.GetComponent<RectTransform>();
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.InverseLerp(0, MaxValue, CurrentValue) * MaxValWidth);
    }

    public void ReturnProgressBar()
    {
        GameObjectPoolManager.Instance.ReturnObjectToPool(this.gameObject, "ProgressBar");
    }
}
