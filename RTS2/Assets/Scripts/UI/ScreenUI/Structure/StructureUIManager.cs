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


    private void Awake()
    {
        CreateButton("None").onClick.AddListener(()=>StructureSelectionMode.Mode=StructureSelectionType.None);
        CreateButton("Walls").onClick.AddListener(() => StructureSelectionMode.Mode = StructureSelectionType.Walls);
        CreateButton("Doors").onClick.AddListener(() => StructureSelectionMode.Mode = StructureSelectionType.Door);

    }

    public Button CreateButton(string text)
    {
        GameObject button = Instantiate(ButtonPrefab, ButtonParent.transform);
        button.GetComponent<StructureButton>().InitButton(text);
        return button.GetComponent<Button>();
    }
}
