using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ResourceCostUIDisplayElement : MonoBehaviour
{
    public Image Icon;
    public TextMeshProUGUI Display;
    string resourceKey;
    int quantityRequired;

    public void Init(string key, int quantityRequired)
    {
        this.quantityRequired = quantityRequired;
        resourceKey = key;
        Icon.sprite = IconManager.Instance.GetIcon(resourceKey);
        ResourceManager.OnResourceQuanityChange += OnResourceAmountChange;
        Refresh();
    }
    private void OnDisable()
    {
        ResourceManager.OnResourceQuanityChange -= OnResourceAmountChange;

    }
    void OnResourceAmountChange(string key)
    {
        Refresh();
    }


    void Refresh()
    {
        
        int count = ResourceManager.Instance.GetUserResources(resourceKey);
        Display.text=count.ToString()+"/"+quantityRequired.ToString();
        bool hasEnough = count >= quantityRequired;
        if (hasEnough)
        {
            Display.color = Color.green;
        }
        else
        {
            Display.color = Color.red;
        }
       
    }
}
