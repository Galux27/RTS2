using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;

public class PauseMenuUIElement : BaseUIElement
{

    static PauseMenuUIElement instance;
    public static PauseMenuUIElement Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<PauseMenuUIElement>(true);
            }
            return instance;
        }
    }
    public ButtonManager Resume, Options, Save,Load, Quit;

    private void Start()
    {
        Resume.gameObject.GetComponent<Button>().onClick.AddListener(HideUI);
        Save.gameObject.GetComponent<Button>().onClick.AddListener(OpenSaveMenu);
        Load.gameObject.GetComponent<Button>().onClick.AddListener(OpenLoadMenu);
        Quit.gameObject.GetComponent<Button>().onClick.AddListener(QuitButtonClick);
    }
    public static float SpeedGameAtWhenOpened = 0f;
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

    void QuitButtonClick()
    {
        Application.Quit();
    }
    void OpenSaveMenu()
    {
        SavingMenu.Instance.DrawUI();
       // SerializationHelpers.SaveGame("TestWorld");
    }
    void OpenLoadMenu()
    {
        LoadingMenu.Instance.DrawUI();
    }
}
