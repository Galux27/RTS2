using UnityEngine;
using UnityEngine.UI;
public class MiscControlsPanel : BaseUIElement
{
    public Button Play, Settings, WorldMap;
    public Image PlayIcon, SettingsIcon, MapIcon;
    private void Awake()
    {
        Play.onClick.AddListener(OnPlayClick);
        PlayIcon.sprite = IconManager.Instance.GetIcon("Play");
        SettingsIcon.sprite = IconManager.Instance.GetIcon("Settings");
        MapIcon.sprite = IconManager.Instance.GetIcon("Map");
        WorldMap.onClick.AddListener(OnWorldMapClick);
        Settings.onClick.AddListener(OnSettingsClick);
    }

    void OnPlayClick()
    {
        if (DeltaTimeWrapper.GameplayDeltaMultiplier > 0)
        {
            DeltaTimeWrapper.GameplayDeltaMultiplier = 0;
            PlayIcon.sprite = IconManager.Instance.GetIcon("Pause");

        }
        else
        {
            DeltaTimeWrapper.GameplayDeltaMultiplier = 1;
            PlayIcon.sprite = IconManager.Instance.GetIcon("Play");

        }
    }

    void OnSettingsClick()
    {
        if (PauseMenuUIElement.Instance.IsDrawn())
        {
            PauseMenuUIElement.Instance.HideUI();
        }
        else
        {
            PauseMenuUIElement.Instance.DrawUI();
        }
    }


    void OnWorldMapClick()
    {
        if (!MapScreen_UIElement.Instance.IsDrawn())
        {
            MapScreen_UIElement.Instance.DrawUI();
        }
        else
        {
            MapScreen_UIElement.Instance.HideUI();
        }
    }
}
