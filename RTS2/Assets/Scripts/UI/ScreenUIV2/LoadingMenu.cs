using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using TMPro;
public class LoadingMenu : BaseUIElement
{
    public Transform SaveGamesParent;
    public GameObject SaveGamesPrefab;
    public ButtonManagerBasic Back;
    List<LoadingUIElement> currentSaveGames;
    public override void DrawUI()
    {
        base.DrawUI();
        RefreshSavesFound();
    }

    void RefreshSavesFound()
    {

    }
}
