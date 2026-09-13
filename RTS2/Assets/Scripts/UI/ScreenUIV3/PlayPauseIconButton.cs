using UnityEngine;

public class PlayPauseIconButton : IconButton
{
    public IconButtonState PlayState, PauseState;


    private void Awake()
    {
        DefaultState = PlayState;
        base.Awake();
        OnClick += OnButtonClick;
    }

    void OnButtonClick()
    {
        if(CurrentState.StateName==PlayState.StateName)
        {
            SetPause();
        }
        else
        {
            SetPause();
        }
    }

    void SetPlay()
    {
        SetState(PlayState);
    }

    void SetPause()
    {
        SetState(PauseState);
    }
}
