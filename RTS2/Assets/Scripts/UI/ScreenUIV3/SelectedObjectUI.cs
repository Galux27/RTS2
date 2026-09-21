using System.Collections.Generic;
using UnityEngine;

public class SelectedObjectUI : BaseUIElement
{
    public const string ObjectPoolKey="SelectObjectButton";
    public Transform ButtonParent;
    Dictionary<Selectable, GameObject> CurrentObjects = new Dictionary<Selectable, GameObject>();
    private void Awake()
    {
        UIEventManager.OnObjectDeselected += OnObjectDeselected;
        UIEventManager.OnObjectSelected+= OnObjectSelected; 
    }

    void OnObjectSelected(Selectable selected)
    {
        GameObject UIInstance = GameObjectPoolManager.Instance.GetObjectFromPool(ObjectPoolKey);
        UIInstance.transform.parent = ButtonParent;
        SelectableIconButto button = UIInstance.GetComponent<SelectableIconButto>();
        button.SetSelectable(selected);
        UIInstance.gameObject.SetActive(true);
        CurrentObjects.Add(selected, UIInstance);
    }

    void OnObjectDeselected(Selectable selected)
    {
        if (CurrentObjects.ContainsKey(selected))
        {
            CurrentObjects[selected].SetActive(false);
            GameObjectPoolManager.Instance.ReturnObjectToPool(CurrentObjects[selected], ObjectPoolKey);
            CurrentObjects.Remove(selected);
        }
    }
}
