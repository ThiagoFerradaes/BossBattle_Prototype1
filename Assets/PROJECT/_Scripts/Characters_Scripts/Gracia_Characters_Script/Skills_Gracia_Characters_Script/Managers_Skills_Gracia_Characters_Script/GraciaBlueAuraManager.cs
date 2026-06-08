using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class GraciaBlueAuraManager : SkillObjectManager
{
    #region Paramethers

    // Components
    GraciaBlueAuraSO _info;

    // int
    int _skillLevel;

    // Actions
    Action _onHit;

    // Coroutines
    Coroutine _skillDurationRoutine, _waitToSpawnHitRoutine;

    #endregion

    #region Initialize

    public override void UseSkill(SkillSO skill) {
        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine());
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as GraciaBlueAuraSO;
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

        _onHit = CallInstantiateHit;
        _skillLevel = GraciaPassiveManager.Instance.ReturnCurrentSkillArea(_info.TypeOfSkill);
    }


    #endregion

    #region Animation Methodes Override

    protected override void FirstFunc() {
        base.FirstFunc();

        cooldownManager.SetCooldownSingleCharge(slot, _info.Cooldown);
    }

    protected override void FourthFunc() {
        base.FourthFunc();

        GraciaAttackManager.OnAttackHit -= _onHit;
        GraciaAttackManager.OnAttackHit += _onHit;

        GraciaPassiveManager.Instance.ChangeBarValue(_info.AmountOfValueGainedWhenUsed, _info.TypeOfAura);

        UnblockInputs();

        _skillDurationRoutine ??= StartCoroutine(SkillDuration());
    }

    IEnumerator SkillDuration() {
        float timer = 0f;

        while (timer < _info.SkillDuration) {
            timer += Time.deltaTime;
            yield return null;
        }

        _skillDurationRoutine = null;

        if (_waitToSpawnHitRoutine != null) {
            StopCoroutine(_waitToSpawnHitRoutine);
            _waitToSpawnHitRoutine = null;
        }

        GraciaAttackManager.OnAttackHit -= _onHit;
        End();
    }

    #endregion

    #region Instantiate

    void CallInstantiateHit() {
        _waitToSpawnHitRoutine ??= StartCoroutine(WaitToInstantiateHit());
    }

    IEnumerator WaitToInstantiateHit() {
        float attackSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        float timeToSpawnHit = _info.CooldownToHit / attackSpeed;

        yield return new WaitForSeconds(timeToSpawnHit);

        _waitToSpawnHitRoutine = null;

        InstantiateHit();
    }

    void InstantiateHit() {

        // Buscando hitbox na pool
        GameObject prefab = PoolingManager.Instance.ReturnPrefabFromPool(_info.Prefabs[0][0].PreFab, TypeOfSkillPrefab.Hitbox);

        // Buscando o atributo de acordo com o n�vel
        DamageAtributes atributes = _info.AttackAtributesList[_skillLevel];

        // Settando o tamanho e a posi��o do ataque
        prefab.transform.localScale = atributes.Size;
        prefab.transform.SetParent(parent.transform, false);
        prefab.transform.SetLocalPositionAndRotation(_info.Prefabs[0][0].PreFabPosition, Quaternion.identity);
        prefab.transform.SetParent(null);

        // Calculando o dano do ataque
        DamageContext newContext = new(atributes, statusManager);

        // Ativando a hitbox
        InstantDamageHitBox hitbox = prefab.GetComponent<InstantDamageHitBox>();
        hitbox.Initialize(newContext);
    }

    #endregion

}
