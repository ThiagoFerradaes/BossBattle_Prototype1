using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LilianJudgmentDayManager : SkillObjectManager
{
    // Components
    LilianJudgmentDaySO _info;
    ContinuosDamageHitBox _damageHitBox;

    Coroutine _durationRoutine, _damageRoutine;

    #region Initialize
    public override void UseSkill(SkillSO skill)
    {
        base.UseSkill(skill);

        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine());
    }

    void Initialize(SkillSO skill)
    {
        if (_info == null)
        {
            _info = skill as LilianJudgmentDaySO;
        }

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    #endregion

    #region OverrideRegion
    public override void FirstFunc()
    {
        base.FirstFunc();

        energyManager.LooseAllEnergy();

        energyManager.SetCanGainEnergy(false);
    }

    public override void FourthFunc()
    {
        base.FourthFunc();

        // Desbloqueando inputs
        UnblockInputs();
    }
    #endregion


    public override void InstantiateHitBox(SkillAnimationEvent prefab)
    {
        healthManager.Heal(_info.InitialHeal);

        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);
        hitbox.transform.SetParent(parent.transform);
        hitbox.transform.localScale = _info.Atributes.Size;
        hitbox.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        hitbox.transform.SetParent(null);

        DamageContext context = new(_info.Atributes, statusManager);

        ContinuosDamageHitBox damageHitbox = hitbox.GetComponent<ContinuosDamageHitBox>();
        _damageHitBox = damageHitbox;
        _damageHitBox.Initialize(context);

        if (healthManager.ReturnCurrentHealth() > _info.HealthLimit)
        {
            _durationRoutine ??= StartCoroutine(Duration());
            _damageRoutine ??= StartCoroutine(DamageToLilianRoutine());
        }
        else End();
    }

    IEnumerator Duration()
    {
        while (healthManager.ReturnCurrentHealth() > _info.HealthLimit)
        {
            yield return null;
        }

        End();
    }

    public override void End()
    {
        _damageHitBox.End();
        _damageHitBox = null;

        _durationRoutine = null;
        if (_damageRoutine != null)
        {
            StopCoroutine(_damageRoutine);
            _damageRoutine = null;
        }

        energyManager.SetCanGainEnergy(true);

        base.End();
    }

    IEnumerator DamageToLilianRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_info.DamageCooldownToLilian);

            float damageToLilian = _info.DamageToLilian;
            float currentHealth = healthManager.ReturnCurrentHealth();

            if (currentHealth - damageToLilian <= _info.HealthLimit)
            {
                float damage = currentHealth - _info.HealthLimit;
                healthManager.TakeDamage(damage);
            }
            else healthManager.TakeDamage(damageToLilian);
        }
    }
}
