using UnityEngine;

public class LilianBaseAttackManager : SkillObjectManager
{
    // Components
    LilianBaseAttackSO _info;
    HealthManager _healthManager;

    // Atributes
    int _attackIndex = 1;
    float _attackSpeedMultiplier;

    #region Initialize
    public override void UseSkill(SkillSO skill)
    {
        base.UseSkill(skill);

        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(_attackIndex - 1));
    }

    private void Initialize(SkillSO skill)
    {
        if (_info == null) _info = skill as LilianBaseAttackSO;
        if (_healthManager == null) _healthManager = parent.GetComponent<HealthManager>();

        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }


    }

    #endregion

    #region Skill

    protected override void FirstFunc()
    {
        _attackSpeedMultiplier = GetAttackSpeedMultiplier();
        anim.SetFloat(_info.AttackSpeedAnimationParameter, _attackSpeedMultiplier);
        skillManager.SkillIsInAnimation(true);
    }

    protected override void ThirdFunc()
    {
        float healthToLoose = _healthManager.ReturnCurrentHealth() * _info.DamagePercentToDamageLilian / 100;
        _healthManager.TakeDamage(healthToLoose);
    }

    protected override void FourthFunc()
    {
        base.FourthFunc();

        // Cooldown
        cooldownManager.SetCooldownSingleCharge(slot, _info.Cooldown);

        // Resetando a velocidade da anima��o
        anim.SetFloat(_info.AttackSpeedAnimationParameter, 1);

        // Attack Index
        switch (_attackIndex)
        {
            case 1:
                _attackIndex = 2;
                break;
            case 2:
                _attackIndex = 1;
                break;
        }


        // Desbloqueando inputs
        UnblockInputs();
    }

    float GetAttackSpeedMultiplier()
    {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }

    public override void InstantiateHitBox(SkillAnimationEvent prefabInfo)
    {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        preFab.transform.localScale = _info.SkillDamageAtributes.Size;
        preFab.transform.SetPositionAndRotation(parent.transform.position + prefabInfo.PreFabPosition, parent.transform.rotation);

        DamageContext newContext = new(
            _info.SkillDamageAtributes,
            parent.GetComponent<StatusManager>()
            );

        ProjectileDamageHitBox hitbox = preFab.GetComponent<ProjectileDamageHitBox>();
        hitbox.Initialize(newContext);

        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
        };
    }
    #endregion
}
