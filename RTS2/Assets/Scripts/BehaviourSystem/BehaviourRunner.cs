using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourRunner : MonoBehaviour
{
    public BehaviourBase CurrentBehaviour;


    public void SetBehaviour(BehaviourBase toPerform)
    {
        CurrentBehaviour= toPerform; 
    }


    private void Update()
    {
        if(CurrentBehaviour != null)
        {
            if(CurrentBehaviour.CanPerformBehaviour())
            {

                CurrentBehaviour.PerformBehaviour();
            }

            if (CurrentBehaviour.IsBehaviourComplete())
            {
                OnBehaviourComplete();
            }
        }
    }

    void OnBehaviourComplete()
    {
        CurrentBehaviour.OnComplete?.Invoke();
        CurrentBehaviour = null;
    }
}
