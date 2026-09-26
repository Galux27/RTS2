using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UnitCapacityUI : MonoBehaviour
{
    public TextMeshProUGUI CapacityDisplay;
    public Image Icon;
    string UnitKey;

    public void InitForTotal()
    {
        Icon.sprite = IconManager.Instance.GetIcon("Unit");
        UIEventManager.OnTotalUnitCapacityUpdated += OnTotalCapacityChange;
        UIEventManager.OnUnitCountUpdated += OnTotalUnitCountIncrease;
        RefreshUI();
    }

    public void Init(string key)
    {
        Icon.sprite = IconManager.Instance.GetIcon("Unit");
        UnitKey = key;
        UIEventManager.OnUnitCapacityUpdated += OnCapacityChange;
        UIEventManager.OnUnitCountUpdated += OnQuantityChange;
        RefreshUI();
    }

    void OnTotalCapacityChange(int val)
    {
        RefreshTotalUnits();
    }

    void OnTotalUnitCountIncrease(string key,int val)
    {
        RefreshTotalUnits();
    }

    void RefreshTotalUnits()
    {
        CapacityDisplay.text = UnitMoniter.Instance.GetTotalUnitCount() + "/" + UnitCapacityManager.TotalCapacity;

    }

    void OnCapacityChange(string key, int val)
    {
        if (UnitKey == key)
        {
            RefreshUI();
        }
    }
    void OnQuantityChange(string key, int val)
    {
        if (UnitKey == key)
        {
            RefreshUI();
        }
    }
    public void RefreshUI()
    {
        CapacityDisplay.text=UnitMoniter.Instance.GetUserUnitCount(UnitKey)+"/" + UnitCapacityManager.GetMaxCapacityForUnitType(UnitKey).ToString();
    }
}
