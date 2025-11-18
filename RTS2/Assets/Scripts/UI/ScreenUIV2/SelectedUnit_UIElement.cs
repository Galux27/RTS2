using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectedUnit_UIElement : BaseUIElement
{
    public TextMeshProUGUI Type, Health, BehavourType;
    public Button SelectUnitButton, ZoomToUnit;
    Unit myUnit;
    public void SetUnit(Unit u)
    {
        myUnit= u;
        u.MyHealth.OnHealthUpdate += OnHealthChange;
        Type.text = u.MyType.ToString();
        u.BehaviourRunner.OnBehaviourChange += OnBehaviourChange;

        SelectUnitButton.onClick.AddListener(SelectUnit);
        ZoomToUnit.onClick.AddListener(AutoMoveToUnit);
    }

    public void Cleanup()
    {
        myUnit.MyHealth.OnHealthUpdate -= OnHealthChange;
        myUnit.BehaviourRunner.OnBehaviourChange -= OnBehaviourChange;
        SelectUnitButton.onClick.RemoveAllListeners();
        ZoomToUnit.onClick.RemoveAllListeners();
    }

    void SelectUnit()
    {
        SelectableManager.Instance.SetToOnlySelected(myUnit);
    }

    void OnBehaviourChange(BehaviourBase b)
    {
        BehavourType.text = myUnit.BehaviourRunner.GetBehaviourDisplayText();
    }

    void OnHealthChange(float newHealth)
    {
        Health.text = newHealth.ToString()+"/"+myUnit.MaxHealth().ToString();
    }
    void AutoMoveToUnit()
    {
        CameraController.Instance.SetToAutoMove(myUnit.Position());
        SelectionController.Instance.blockInputTimer = .2f;

    }
}
