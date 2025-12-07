using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectedUnit_UIElement : BaseUIElement
{
    public TextMeshProUGUI Type, Health, BehavourType;
    public Button SelectUnitButton, ZoomToUnit;
    Selectable myObject;
    ObjectHealth HealthReferenced;
    BehaviourRunner BehaviourReferenced;
    public void SetUnit(Unit u)
    {
        myObject= u;
        u.MyHealth.OnHealthUpdate += OnHealthChange;
        Type.text = u.MyType.ToString();
        u.BehaviourRunner.OnBehaviourChange += OnBehaviourChange;
        HealthReferenced = u.MyHealth;
        SelectUnitButton.onClick.AddListener(SelectUnit);
        ZoomToUnit.onClick.AddListener(AutoMoveToUnit);
        BehaviourReferenced = u.BehaviourRunner;
    }

    public void SetConstructableObject(ConstructableObjectInstance env)
    {
        myObject = env;
        env.MyHealth.OnHealthUpdate += OnHealthChange;
        Type.text = env.Name();
        SelectUnitButton.onClick.AddListener(SelectUnit);
        ZoomToUnit.onClick.AddListener(AutoMoveToUnit);
        HealthReferenced = env.MyHealth;
        BehaviourReferenced = null;
    }

    public void SetWallSegment(WallSegment env)
    {
        myObject = env;
        env.MyHealth.OnHealthUpdate += OnHealthChange;
        Type.text = env.Name();
        SelectUnitButton.onClick.AddListener(SelectUnit);
        ZoomToUnit.onClick.AddListener(AutoMoveToUnit);
        HealthReferenced = env.MyHealth;
        BehaviourReferenced = null;
    }
    public void Cleanup()
    {
        if (HealthReferenced != null)
        {
            HealthReferenced.OnHealthUpdate -= OnHealthChange;
            HealthReferenced = null;
        }
        if (BehaviourReferenced != null)
        {
            BehaviourReferenced.OnBehaviourChange -= OnBehaviourChange;
            BehaviourReferenced = null;
        }
        SelectUnitButton.onClick.RemoveAllListeners();
        ZoomToUnit.onClick.RemoveAllListeners();
    }

    void SelectUnit()
    {
        SelectableManager.Instance.SetToOnlySelected(myObject);
    }

    void OnBehaviourChange(BehaviourBase b)
    {
        BehavourType.text = BehaviourReferenced.GetBehaviourDisplayText();
    }

    Vector3 GetPositionOfSelecteable()
    {
        if(myObject as Unit != null)
        {
            return (myObject as Unit).Position();
        }
        else if(myObject as ConstructableObjectInstance != null)
        {
            return (myObject as ConstructableObjectInstance).Position();
        }
        return CameraController.Instance.transform.position ;
    }


    void OnHealthChange(float newHealth)
    {
        Health.text = newHealth.ToString()+"/"+HealthReferenced.MaxHealth.ToString();
    }
    void AutoMoveToUnit()
    {
        CameraController.Instance.SetToAutoMove(GetPositionOfSelecteable());
        SelectionController.Instance.blockInputTimer = InputController.BlockInputLength;

    }
}
