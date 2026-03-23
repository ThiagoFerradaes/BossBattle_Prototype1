using UnityEngine;
using UnityEngine.UIElements;

public class LilianCrowManager : SkillObjectManager {
    LilianCrowSO _info;

    bool _hasExploded, _hasFinishedAnimation, _hasGainedEnergyInExplosion;
    float _skillDamage;

    #region Initialize
    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine());
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as LilianCrowSO;

        if (!gameObject.activeInHierarchy) {
            gameObject.SetActive(true);
        }
    }
    #endregion

    #region Overrides

    protected override void FirstFunc() {
        base.FirstFunc();

        DecideCooldown();

        _skillDamage = DecideExplosionDamage();

        float healthToLoose = healthManager.ReturnCurrentHealth() * _info.SkillPercentDamageToLillian / 100;
        healthManager.TakeDamage(healthToLoose);
    }

    protected override void FourthFunc() {
        base.FourthFunc();

        _hasFinishedAnimation = true;

        TryFinishSkill();
    }

    void TryFinishSkill() {
        if (!_hasExploded || !_hasFinishedAnimation)
            return;

        _hasExploded = false;
        _hasFinishedAnimation = false;
        _hasGainedEnergyInExplosion = false;
        _skillDamage = 0;

        EndWithUnblockSkills();
    }
    void DecideCooldown() {

        // Pegando a porcentagem de vida atual
        float currentHealth = healthManager.ReturnCurrentHealth();
        float maxHealth = healthManager.ReturnMaxHealth();

        float healthPercent = currentHealth / maxHealth;

        // Calculos

        float maxCD, minCD;
        maxCD = _info.Cooldown;
        minCD = _info.SkillMinCooldown;

        float minHealthPercent = _info.SkillMinCooldownHealthPercent / 100f;

        float t = Mathf.InverseLerp(minHealthPercent, 1f, healthPercent);

        float currentCooldown = Mathf.Lerp(minCD, maxCD, t);
        cooldownManager.SetCooldownSingleCharge(slot, currentCooldown);
    }

    #endregion

    #region Instantiate
    public override void InstantiateHitBox(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        preFab.transform.localScale = _info.SkillDamageAtributes.Size;
        preFab.transform.SetPositionAndRotation(parent.transform.position + prefabInfo.PreFabPosition, parent.transform.rotation);

        DamageContext newContext = new(
            _info.SkillDamageAtributes,
            parent.GetComponent<StatusManager>()
            );

        ProjectileDamageHitBox hitbox = preFab.GetComponent<ProjectileDamageHitBox>();
        hitbox.Initialize(newContext);

        hitbox.OnFinalDestination += Explode;

        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
        };

    }

    void Explode(Vector3 position) {
        for (int j = 0; j < _info.Prefabs[1].Count; j++) {

            if (_info.Prefabs[1][j].PrefabType == TypeOfSkillPrefab.Hitbox) {

                InstantiateExplosion(position, _info.Prefabs[1][j].PreFab);

            }
            else if (_info.Prefabs[1][j].PrefabType == TypeOfSkillPrefab.VFX) {

                InstantiateVFX(_info.Prefabs[1][j], position);

            }
        }
    }

    void InstantiateExplosion(Vector3 position, GameObject explosionHitbox) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(explosionHitbox, TypeOfSkillPrefab.Hitbox);

        preFab.transform.localScale = _info.ExplosionAtributes.Size;
        preFab.transform.SetPositionAndRotation(position, Quaternion.identity);

        DamageAtributes newAtributes = new(_info.ExplosionAtributes) {
            Damage = _skillDamage
        };

        DamageContext newContext = new(
            newAtributes,
            parent.GetComponent<StatusManager>()
        );

        InstantDamageHitBox hitbox = preFab.GetComponent<InstantDamageHitBox>();
        hitbox.Initialize(newContext);

        hitbox.OnHit += () =>
        {
            if (_hasGainedEnergyInExplosion) return;
            _hasGainedEnergyInExplosion = true;

            energyManager.GainEnergy(_info.ExplosionEnergy);
        };

        _hasExploded = true;

        TryFinishSkill();

    }

    float DecideExplosionDamage() {
        // Pegando a porcentagem de vida atual
        float currentHealth = healthManager.ReturnCurrentHealth();
        float maxHealth = healthManager.ReturnMaxHealth();

        float healthPercent = currentHealth / maxHealth;

        // Calculos

        float maxDMG, minDMG;
        maxDMG = _info.ExplosionMaxDamage;
        minDMG = _info.ExplosionAtributes.Damage;

        float minHealthPercent = _info.SkillMinCooldownHealthPercent / 100f;

        float t = Mathf.InverseLerp(minHealthPercent, 1f, healthPercent);

        t = 1 - t;

        float damage = Mathf.Lerp(minDMG, maxDMG, t);
        return damage;
    }
    #endregion

}
