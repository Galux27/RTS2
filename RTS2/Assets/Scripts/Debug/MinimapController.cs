using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{

    static MinimapController instance;
    public static MinimapController Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<MinimapController>(true) ;
            }
            return instance;
        }
    }

    public RawImage UnitsDisplay;
    UnitsMinimapRenderer Units;

    // Start is called before the first frame update
    void Start()
    {
        Units = new UnitsMinimapRenderer();
        StartCoroutine(UpdateMinimap());

    }

    // Update is called once per frame
    bool isUpdatingData = false, updatedTexture = false;
    void Update()
    {
        Debug.Log("Minimap: update " + Units.IsDataUpdateDone() + "," + isUpdatingData);

        if (Units.IsDataUpdateDone()==false)
        {
            isUpdatingData = true;
            StartCoroutine(RefreshUnitData());
           
        }
        else
        {
            if (!updatedTexture)
            {
                Units.RefreshTexture();
                UnitsDisplay.texture = Units.Texture;
                updatedTexture = true;
                StartCoroutine(UpdateMinimap());

            }
        }
       
    }
    const float TimeBetweenMinimapUpdates = 1f;

    IEnumerator UpdateMinimap()
    {
        yield return new WaitForSeconds(TimeBetweenMinimapUpdates);
        Units.StartRefresh();
        updatedTexture = false;
        Debug.Log("Minimap: starting refresh..");
    }

    IEnumerator RefreshUnitData()
    {
        yield return new WaitForEndOfFrame();
        Units.RefreshData();
        Debug.Log("Minimap: updating data..");

        isUpdatingData = false;
    }
}
