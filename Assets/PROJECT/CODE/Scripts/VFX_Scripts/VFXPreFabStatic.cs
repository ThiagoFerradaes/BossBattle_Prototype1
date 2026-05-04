using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

[Serializable]
public class VFXAtributes
{
    public TypeOfCollider VFXType;

    [ShowIf("VFXType", TypeOfCollider.Instant), AllowNesting] public float VFXDuration;

    bool isProjectileOrBoomerang => VFXType == TypeOfCollider.Projectile || VFXType == TypeOfCollider.Boomerang;
    [ShowIf("VFXType", TypeOfCollider.Projectile), AllowNesting] public float VFXPosCollisionDuration;
    [ShowIf("VFXType", TypeOfCollider.Projectile), AllowNesting] public bool CrossEnemy;
    [ShowIf("VFXType", TypeOfCollider.Boomerang), AllowNesting] public float TimeStopped;
    [ShowIf("VFXType", TypeOfCollider.Boomerang), AllowNesting] public float MinDistanceBack = 0.1f;
    [ShowIf("isProjectileOrBoomerang"), AllowNesting] public LayerMask UnitsToHit;
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

    private VisualEffect myVFX;
    private ParticleSystem myParticle;

    private bool isVFX;
    private bool isParticle;

    private Collider myCollider;

    public AK.Wwise.Event vfxSoundEvent;
    [SerializeField, ShowIf("ShowWarningDelay"), AllowNesting]
    private float warningResetDelay = 20f;
    private bool ShowWarningDelay => gameObject.name == "TentacleHit_AreaOfImpact_Red";
    private bool warningPlayed = false;

    void Awake()
    {
        isVFX = TryGetComponent<VisualEffect>(out myVFX);
        isParticle = TryGetComponent<ParticleSystem>(out myParticle);
    }

    public void Initialize(VFXAtributes atributes)
    {
        gameObject.SetActive(true);
        if (vfxSoundEvent != null)
        {
            vfxSoundEvent.Post(gameObject);
            if (vfxSoundEvent.Name == "Play_Warning_Siren" || vfxSoundEvent.Name == "Play_Stalactite_Warning_Siren")
            {
                warningPlayed = true;

                if (warningPlayed)
                {
                    StartCoroutine(ResetWarningAfterDelay(warningResetDelay));
                }
            }
        }
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

    private IEnumerator ResetWarningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        warningPlayed = false;
    }

}
