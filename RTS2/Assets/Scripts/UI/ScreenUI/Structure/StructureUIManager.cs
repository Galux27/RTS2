using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StructureUIManager : MonoBehaviour
{
    static StructureUIManager instance;
    public static  StructureUIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<StructureUIManager>();
            }
            return instance;
        }
    }
    public GameObject ButtonPrefab, ButtonParent;


    private void Start()
    {
        CreateButton("None").onClick.AddListener(()=>StructureSelectionMode.Mode=StructureSelectionType.None);

        Button wallButton = CreateButton("Walls");
        StructureButton sb = wallButton.GetComponent<StructureButton>();
        foreach(KeyValuePair<string,WallTile> walls in WallTypeManager.Instance.AllObjects)
        {
            Action OnButtonClick = new Action(() => StructureSelectionMode.Mode = StructureSelectionType.Walls);
            OnButtonClick += () => WallTypeManager.Instance.SelectedWallTile = walls.Value;
            sb.AddType(walls.Key,OnButtonClick);
        }


        CreateButton("Doors").onClick.AddListener(() => StructureSelectionMode.Mode = StructureSelectionType.Door);

    }

  
    public Button CreateButton(string text)
    {
        GameObject button = Instantiate(ButtonPrefab, ButtonParent.transform);
        button.GetComponent<StructureButton>().InitButton(text);
        return button.GetComponent<Button>();
    }
}
