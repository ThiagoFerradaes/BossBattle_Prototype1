using System;
using UnityEngine;

public class StageByMoveInPosition : TutorialClassBehaviour
{
    public override event Action<bool> OnCompleteTutorialEvent;

    [SerializeField]private LayerMask playerLayer;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == playerLayer) OnCompleteTutorialEvent?.Invoke(true);
    }
}
