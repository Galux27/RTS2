using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Planter Behavior", menuName = "ScriptableObjects/ConstructableObjectBehaviours/Planter", order = 1)]
public class PlanterBehaviour :EnvironmentObjectBehaviourBase
{
    EnvironmentObjectInstance myObject;
    public float GrowDuration = 90f;
    bool DoneFirstUpdate = false;
    public bool HasBeenSeeded = false,Grown=false;
    float GrowStartTime = -1f;
    public List<Sprite> PlantGrowthSprites = new List<Sprite>();
    public List<HarvestableResourceData> HarvestData;
    
    public override bool HasUpdate()
    {
        return true;
    }
    public bool IsDone()
    {
        return Grown;
    }

    public void SeedPlanter()
    {
        Debug.Log("Planter: seeded planter");
        HasBeenSeeded = true;
        if (myObject.Drawn)
        {
            OnObjectRender();
        }
    }

    public void Harvest(Vector3 UnitHarvestingPos)
    {
        Grown = false;
        DoneFirstUpdate = false;
        GrowStartTime = -1;
        HasBeenSeeded = false;

        for (int x = 0; x < HarvestData.Count; x++)
        {
            HarvestData[x].GenerateResoruces(UnitHarvestingPos);
        }
        if (GrowingPlant != null)
        {
            GrowingPlant.GetComponent<SpriteRenderer>().sprite = GetGrowingStageSprite();
        }
    }

    public override void PassInEnvironmentObjectInstance(EnvironmentObjectInstance instance)
    {
        myObject= instance;
        myObject.OnRender += OnObjectRender;
        myObject.OnHidden += OnObjectHidden;
       
    }

    public override void PerformCheckForActionsFromObject(out List<PotentialBehaviourAssignment> retVal)
    {
        retVal = new List<PotentialBehaviourAssignment>();
        if (HasBeenSeeded == false)
        {
            retVal.Add(new PlantSeeds_PotentialBehaviour(myObject));
        }else if (Grown)
        {
            retVal.Add(new HarvestPlanter_PotentialBehaviour(myObject));
        }
    }
    GameObject GrowingPlant = null;
    public override PotentialBehaviourAssignment GetPotentialBehaviour(Type toDo)
    {
        if (HasBeenSeeded==false)
        {
            return (new PlantSeeds_PotentialBehaviour(myObject));
        }
        else if (Grown)
        {
            return (new HarvestPlanter_PotentialBehaviour(myObject));
        }
        return null;
    }
    bool init = false;
    public void OnObjectRender()
    {
        if (GrowingPlant == null)
        {
            GrowingPlant= GameObjectPoolManager.Instance.GetObjectFromPool("EnvironmentObject");
            GrowingPlant.gameObject.SetActive(true);
            GrowingPlant.transform.position = myObject.Object.transform.position;
        }
        SpriteRenderer sr = GrowingPlant.GetComponent<SpriteRenderer>();
        sr.sprite = GetGrowingStageSprite();
        sr.sortingOrder = myObject.Object.GetComponent<SpriteRenderer>().sortingOrder + 1;
        
    }

   

    Sprite GetGrowingStageSprite()
    {
        if (!Grown)
        {
            if (HasBeenSeeded == false)
            {
                return null;
            }
            else
            {
                
                return PlantGrowthSprites[growSpriteIndex];

            }
        }
        else
        {
            return PlantGrowthSprites[PlantGrowthSprites.Count - 1];
        }
       
    }
    int growSpriteIndex = 0;
   bool CheckToChangeSprite(float growTime)
    {
        float progress = Mathf.InverseLerp( 0.0f, GrowDuration, growTime);

        int newIndex = Mathf.RoundToInt(progress * PlantGrowthSprites.Count-2);

        if (newIndex != growSpriteIndex)
        {
            growSpriteIndex= newIndex;
            growSpriteIndex = Mathf.Clamp(growSpriteIndex,0, PlantGrowthSprites.Count - 2);
            return true;
        }
        return false;
    }

    public void OnObjectHidden()
    {
        GameObjectPoolManager.Instance.ReturnObjectToPool(GrowingPlant, "EnvironmentObject");
        GrowingPlant = null;
    }

    public override void OnUpdate()
    {
      
        if (HasBeenSeeded)
        {
            if (!DoneFirstUpdate)
            {
                GrowStartTime = GameTime.Instance.InGameTime;
                DoneFirstUpdate = true;
            }
            Debug.Log("Planter: Seed Growth Progress " + (GameTime.Instance.InGameTime - GrowStartTime));
            if(CheckToChangeSprite(GameTime.Instance.InGameTime - GrowStartTime))
            {
                if (GrowingPlant != null)
                {
                    GrowingPlant.GetComponent<SpriteRenderer>().sprite = GetGrowingStageSprite();
                }
                }
                if (GameTime.Instance.InGameTime-GrowStartTime> GrowDuration)
            {
                Debug.Log("Planter: grown");

                Grown = true;
            }
            
        }
    }
}
