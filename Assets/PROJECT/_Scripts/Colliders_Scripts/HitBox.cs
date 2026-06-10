using System;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public static event Action<LayerMask> OnHitTarget;

    protected void CallOnHitTargetEvent(LayerMask layer) {
        OnHitTarget?.Invoke(layer);
    }
}
