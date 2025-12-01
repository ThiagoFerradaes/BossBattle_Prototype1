using NaughtyAttributes;
using System;
using UnityEngine;

[Serializable]
public class VFXAtributes
{
    public TypeOfCollider VFXType;

    public float VFXDuration;
    public LayerMask UnitsToHit;

    bool isProjectileOrBoomerang => VFXType == TypeOfCollider.Projectile || VFXType == TypeOfCollider.Boomerang;
    [ShowIf("VFXType", TypeOfCollider.Projectile), AllowNesting] public float VFXPosCollisionDuration;
    [ShowIf("VFXType", TypeOfCollider.Projectile), AllowNesting] public bool CrossEnemy;
    [ShowIf("VFXType", TypeOfCollider.Boomerang), AllowNesting] public float TimeStopped;
    [ShowIf("VFXType", TypeOfCollider.Boomerang), AllowNesting] public float MinDistanceBack = 0.1f;
    [ShowIf("isProjectileOrBoomerang"), AllowNesting] public float VFXSpeed;
    [ShowIf("isProjectileOrBoomerang"), AllowNesting] public float Distance;

    public VFXAtributes(VFXAtributes source)
    {
        VFXType = source.VFXType;
        VFXDuration = source.VFXDuration;
        UnitsToHit = source.UnitsToHit ;
        VFXPosCollisionDuration = source.VFXPosCollisionDuration;
        CrossEnemy = source.CrossEnemy;
        TimeStopped = source.TimeStopped;
        MinDistanceBack = source.MinDistanceBack;
        VFXSpeed = source.VFXSpeed;
        Distance = source.Distance;
    }
}

public class VFXPreFabStatic : MonoBehaviour
{
    public void Initialize(VFXAtributes atributes)
    {
        gameObject.SetActive(true);
        Invoke(nameof(TurnOff), atributes.VFXDuration);
    }
    public void Initialize(float duration)
    {
        gameObject.SetActive(true);
        Invoke(nameof(TurnOff), duration);
    }
    public void TurnOff()
    {
        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.VFX);
    }
}
