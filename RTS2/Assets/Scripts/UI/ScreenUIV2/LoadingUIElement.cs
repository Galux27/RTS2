using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using TMPro;
using System.ComponentModel;
using UnityEngine.UI;
using UnityEngine.Windows;
using System.IO;
using System;

public class LoadingUIElement : MonoBehaviour
{
    public ButtonManagerBasic Load, Delete;
    public TextMeshProUGUI SaveInfo, DateCreated;
    string saveName = "";
    FoundSave currentSave;
    private void Start()
    {
        Load.GetComponent<Button>().onClick.AddListener(OnLoad);
        Delete.GetComponent<Button>().onClick.AddListener(OnDelete);
    }
    public void InitWithData(FoundSave save)
    {
        currentSave = save;
        saveName= Path.GetFileName(save.path);
        SaveInfo.text = saveName; 
        DateCreated.text = save.CreatedAt.ToShortTimeString();
    }


    void OnLoad()
    {
        SaveLoadHelpers.LoadGame(saveName);
    }

    void OnDelete()
    {
        Action Delete = () => { SaveLoadHelpers.DeleteSave(saveName);LoadingMenu.Instance.RefreshUI(); };
        ConfirmModal.Instance.DisplayConfirmModal("Are you sure you want to delete the save at " + currentSave.path + "?", "Delete Save", Delete, null);
    }
}
