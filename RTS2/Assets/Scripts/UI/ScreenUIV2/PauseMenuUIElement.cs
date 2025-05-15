using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;

public class PauseMenuUIElement : BaseUIElement
{
    public ButtonManager Resume, Options, SaveLoad, Quit;

    private void Start()
    {
        Resume.gameObject.GetComponent<Button>().onClick.AddListener(HideUI);
        SaveLoad.gameObject.GetComponent<Button>().onClick.AddListener(SaveTest);
    }
    float SpeedGameAtWhenOpened = 0f;
    public override void HideUI()
    {
        DeltaTimeWrapper.GameplayDeltaMultiplier =SpeedGameAtWhenOpened;
        base.HideUI();
    }
    public override void DrawUI()
    {
        SpeedGameAtWhenOpened=DeltaTimeWrapper.GameplayDeltaMultiplier;
        DeltaTimeWrapper.GameplayDeltaMultiplier = 0f;
        base.DrawUI();
    }

    void SaveTest()
    {
        SerializationHelpers.SaveGame("TestWorld");
      

    }
}
