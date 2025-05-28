using Michsky.UI.ModernUIPack;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavingMenu : BaseUIElement
{
    static SavingMenu instance;
    public static SavingMenu Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<SavingMenu>(true);
            }
            return instance;
        }
    }

    private void Awake()
    {
        BackButton.GetComponent<Button>().onClick.AddListener(HideUI);
        BackButton.GetComponent<Button>().onClick.AddListener(PauseMenuUIElement.Instance.DrawUI);
        SaveGameButton.GetComponent<Button>().onClick.AddListener(OnSave);
    }

    public CustomInputField NameInput;
    public ButtonManagerBasic SaveGameButton, BackButton;
    public TextMeshProUGUI NameValidityInfo;
    public override void DrawUI()
    {
        base.DrawUI();
    }

    public override void HideUI()
    {
        base.HideUI();
    }

    bool nameValid = false;
    string name;
    private void Update()
    {
       name = NameInput.inputText.text;
       nameValid = SaveLoadHelpers.IsSaveNameValid(name);
        if (!nameValid)
        {
            List<string> reasonsInvalid = SaveLoadHelpers.ReasonsForInvalid(name);
            string message = name + " invalid because:"+Environment.NewLine;
            for(int x = 0; x < reasonsInvalid.Count; x++)
            {
                message += reasonsInvalid[x] + Environment.NewLine;
            }
            NameValidityInfo.text = message;
        }
        else
        {
            NameValidityInfo.text = "Valid";
        }
    }


    void OnSave()
    {
        if (nameValid)
        {
            if (SaveLoadHelpers.FileWithNameAlreadyExists(name))
            {
                Action onConfirm = () => SaveLoadHelpers.DeleteSave(name);
                onConfirm+=()=> SerializationHelpers.SaveGame(name);
                ConfirmModal.Instance.DisplayConfirmModal("Do you want to overwrite " + name + "?", "Overwrite Save", onConfirm, null);

            }
            else
            {
                SerializationHelpers.SaveGame(name);
            }
        }
    }
}
