using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SelectedObjectButton : MonoBehaviour
{
    public Button SelectButton, ZoomToButton;
    public TextMeshProUGUI name, quantitiy;
    public void InitAsFilter(SelectedObjectCategory category)
    {
        name.text = category.Key;
        quantitiy.text = category.Quantity.ToString();
        ZoomToButton.gameObject.SetActive(false);
        SelectButton.onClick.AddListener(() =>
        {
            SelectableManager.Instance.SetToOnlyNameSelected(category.Key);
            SelectionController.Instance.blockInputTimer = .2f;
           
        }
        );
    }

    public void InitAsSelect(ObjectInfo objectInfo)
    {
        name.text = objectInfo.Name();
        ZoomToButton.gameObject.SetActive(true);
        quantitiy.text = "";
        SelectButton.onClick.AddListener(() => { SelectableManager.Instance.SetToOnlySelected(objectInfo as Selectable);
            SelectionController.Instance.blockInputTimer = .2f;

        });
    }
}
