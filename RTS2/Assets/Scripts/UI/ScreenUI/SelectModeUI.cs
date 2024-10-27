using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.UI;
public class SelectModeUI : MonoBehaviour
{

    public Button None, Units, Buildings;

    private void Awake()
    {
        None.onClick.AddListener(() => { SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.None); });
        Units.onClick.AddListener(() => { SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Units); });
        Buildings.onClick.AddListener(() => { SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Buildings); });

    }
}
