using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine.UI;
public class LoadingMenu : BaseUIElement
{
    static LoadingMenu instance;
    public static LoadingMenu Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<LoadingMenu>(true);
            }
            return instance;
        }
    }
    private void Start()
    {
        Back.GetComponent<Button>().onClick.AddListener(HideUI);
        Back.GetComponent<Button>().onClick.AddListener(PauseMenuUIElement.Instance.DrawUI);
        SaveGamesPrefab.SetActive(false);
    }

    public Transform SaveGamesParent;
    public GameObject SaveGamesPrefab;
    public ButtonManagerBasic Back;
    List<LoadingUIElement> currentSaveGames;
    public override void DrawUI()
    {
        base.DrawUI();
        RefreshSavesFound();
    }

    void ClearSaves()
    {
        if (currentSaveGames == null)
        {
            return;
        }
        for(int x=0;x<currentSaveGames.Count;x++)
        {
            GameObject.Destroy(currentSaveGames[x].gameObject);
        }
        currentSaveGames.Clear();
    }

    void RefreshSavesFound()
    {
        ClearSaves();
        currentSaveGames = new List<LoadingUIElement>();
        List<FoundSave> saves = SaveLoadHelpers.GetAllSaves();
        for(int x = 0; x < saves.Count; x++)
        {
            CreateSaveUI(saves[x]);
        }
    
    }

    void CreateSaveUI(FoundSave save)
    {
        GameObject g = Instantiate(SaveGamesPrefab, SaveGamesParent);
        g.SetActive(true);
        LoadingUIElement ui = g.GetComponentInParent<LoadingUIElement>();
        ui.InitWithData(save);
        currentSaveGames.Add(ui);
    }

    public override void RefreshUI()
    {
        base.RefreshUI();
        RefreshSavesFound();
    }
}
