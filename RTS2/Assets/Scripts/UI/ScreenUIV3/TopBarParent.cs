using UnityEngine;
using System.Collections.Generic;
public class TopBarParent : BaseUIElement
{
    public const string Resource = "ResourceDisplay", Capacity = "CapacityDisplay", Break = "Break";
    private void Start()
    {
        Init();
    }
    public void Init()
    {
        GameObject icon = CreateResourceIcon(Capacity);

        foreach (KeyValuePair<string,ResourceData> kvp in ResourceManager.Instance.UserResources)
        {
            icon = CreateResourceIcon(Resource);
            icon.GetComponent<ResourceUINew>().Init(kvp.Key);
        }
        CreateResourceIcon(Break);
        icon = CreateResourceIcon(Capacity);
        icon.GetComponent<UnitCapacityUI>().InitForTotal();

        foreach (KeyValuePair<UnitType,UserUnitTypeCount> kvp in UnitMoniter.Instance.unitCounts)
        {
            icon = CreateResourceIcon(Capacity);
            icon.GetComponent<UnitCapacityUI>().Init(kvp.Key.ToString());
        }
    }

    GameObject CreateResourceIcon(string pool)
    {
        GameObject retVal = GameObjectPoolManager.Instance.GetObjectFromPool(pool);
        retVal.SetActive(true);
        retVal.transform.parent = this.transform;
        return retVal;
    }

   
}
