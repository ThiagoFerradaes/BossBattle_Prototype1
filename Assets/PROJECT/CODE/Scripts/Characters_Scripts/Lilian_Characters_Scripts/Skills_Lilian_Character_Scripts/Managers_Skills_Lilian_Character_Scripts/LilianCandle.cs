using System;
using System.Collections;
using UnityEngine;

public class LilianCandle : MonoBehaviour
{
    #region Parameters

    // Componentes
    LilianFlameOfPenitenceSO _info;
    GameObject _parent;
    StatusManager _lilianStatusManager;
    EnergyManager _energyManager;
    HealthManager _healthManager;
    ContinuosDamageHitBox _continuosHitBox;

    // Atributes
    bool _canGainTributeAndEnergy;
    
    // Corrotines
    Coroutine _energyAndTributeGainCooldownCoroutine;

    // Actions
    Action _onExplode;

    // Events
    public event Action<LilianCandle> OnDeath;

    #endregion

    #region Initialize

    public void TurnCandleOn(LilianFlameOfPenitenceSO skillInfo, GameObject parent)
    {
        Initialize(skillInfo, parent);

        gameObject.SetActive(true);

        TurnContinuosHitBoxOn();
    }

    private void Initialize(LilianFlameOfPenitenceSO skillInfo, GameObject parent)
    {
        if (_info == null) _info = skillInfo;
        if (_parent == null) _parent = parent;
        if (_energyManager == null) _energyManager = parent.GetComponent<EnergyManager>();
        if (_lilianStatusManager == null) _lilianStatusManager = parent.GetComponent<StatusManager>();
        if (_healthManager == null) _healthManager = GetComponent<HealthManager>();

        _onExplode = Explode;
        _healthManager.OnDeath += _onExplode;
        _canGainTributeAndEnergy = true;
    }

    void End()
    {
        StopAllCoroutines();
        _energyAndTributeGainCooldownCoroutine = null;

        _continuosHitBox.End();
        _continuosHitBox = null;

        OnDeath?.Invoke(this);
        OnDeath = null;

        _healthManager.OnDeath -= _onExplode;

        PoolingManager.Instance.ReturnObjectToPool(gameObject, TypeOfSkillPrefab.Hitbox);
    }

    #endregion

    #region ContinuosHitBox

    void TurnContinuosHitBoxOn()
    {
        GameObject hitBox = PoolingManager.Instance.ReturnPrefabFromPool(_info.ContinuosHitBoxName, _info.ContinuosHitBox, TypeOfSkillPrefab.Hitbox);
        hitBox.transform.SetParent(transform);
        hitBox.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        hitBox.transform.localScale = Vector3.one * _info.CandleContinuosDamageSize;

        float damage = CalculateDamageBasedOnCorruption();

        DamageContext contex = new(
            _info.SkillDamageAtributes,
            Mathf.Infinity,
            _lilianStatusManager,
            new() {
                { ExtraDamageContextAtributes.DamageCooldown, _info.CandleContinuosDamageCooldown}
                }
            );

        _continuosHitBox = hitBox.GetComponent<ContinuosDamageHitBox>();
        _continuosHitBox.Initialize(contex);
        _continuosHitBox.OnHit += () =>
        {
            if (!_canGainTributeAndEnergy) return;

            _energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
            LilianPassiveManager.Instance.ChangeTributeAmount(_info.CandleTributeGeneration);
            _energyAndTributeGainCooldownCoroutine ??= StartCoroutine(TributeAndEnergyCooldown());
        };
    }

    IEnumerator TributeAndEnergyCooldown()
    {
        _canGainTributeAndEnergy = false;
        yield return new WaitForSeconds(_info.CandleTributeGenerationCooldown);

        _energyAndTributeGainCooldownCoroutine = null;
        _canGainTributeAndEnergy = true;
    }

    public float CalculateDamageBasedOnCorruption()
    {
        int corruption = LilianPassiveManager.Instance.ReturnAmountOfCorruption();

        float damage = _info.CandleContinuosDamage;

        float multiplier = 1 + (_info.CandleContinuosDamageCorruptionMultiplier/100 * corruption);

        return multiplier * damage;
    }

    #endregion

    #region Explosion

    public void Explode()
    {
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(_info.ExplosionHitBoxName, _info.ExplosionHitBox, TypeOfSkillPrefab.Hitbox);
        hitbox.transform.localScale = Vector3.one * _info.CandleExplosionDamageSize;
        hitbox.transform.position = this.transform.position;

        float damage = CalculateExplosionDamage();

        DamageContext context = new(
            _info.SkillDamageAtributes, 
            0.1f, 
            _lilianStatusManager
        );

        hitbox.GetComponent<InstantDamageHitBox>().Initialize(context);

        End();
    }

    float CalculateExplosionDamage()
    {
        float percentOfHealth = _healthManager.ReturnCurrentHealth()/_healthManager.ReturnMaxHealth();
        float damageMultiplier = percentOfHealth * _info.CandleExplosionHealthMultiplier;

        return Mathf.Max(_info.CandleExplosionDamage, _info.CandleExplosionDamage * damageMultiplier);
    }

    #endregion
}
