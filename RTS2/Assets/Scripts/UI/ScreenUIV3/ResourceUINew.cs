using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ResourceUINew : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public string Key;
    public Image icon;
    public void Init(string key)
    {
        Key = key;
        icon.sprite = IconManager.Instance.GetIcon(key);
        ResourceManager.OnResourceCapcityChange += OnResourceCapacityChange;
        ResourceManager.OnResourceQuanityChange += OnResourceAmountChange;
        Refresh();
    }

    void OnResourceAmountChange(string key)
    {
        if (key == Key)
        {
            Refresh();
        }
    }

    void OnResourceCapacityChange(string key)
    {
        if (key == Key)
        {
            Refresh();
        }
    }

    void Refresh()
    {
        Text.text = ResourceManager.Instance.GetUserResources(Key) +"/"+  ResourceManager.Instance.GetResourceCapacity(Key);
    }
}
