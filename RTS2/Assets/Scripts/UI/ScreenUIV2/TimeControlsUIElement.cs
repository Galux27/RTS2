using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;
public class TimeControlsUIElement : BaseUIElement
{
    public ButtonManagerBasic Pause, One, Two, Four;
    private void Start()
    {
        Pause.gameObject.GetComponent<Button>().onClick.AddListener(() => SetTimeScale(0f));
        One.gameObject.GetComponent<Button>().onClick.AddListener(() => SetTimeScale(1f));
        Two.gameObject.GetComponent<Button>().onClick.AddListener(() => SetTimeScale(2f));
        Four.gameObject.GetComponent<Button>().onClick.AddListener(() => SetTimeScale(4f));

    }

    public void SetTimeScale(float val)
    {
        DeltaTimeWrapper.GameplayDeltaMultiplier = val;
    }
}
