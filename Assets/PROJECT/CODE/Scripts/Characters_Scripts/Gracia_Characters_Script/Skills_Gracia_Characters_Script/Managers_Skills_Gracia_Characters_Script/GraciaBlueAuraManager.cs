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

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.attackAnimationParameter, _info.attackAnimationName, 0));
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as GraciaBlueAuraSO;
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

        _onHit = CallInstantiateHit;
        _skillLevel = GraciaPassiveManager.Instance.ReturnCurrentSkillArea(_info.typeOfSkill);
    }


    #endregion

    #region Animation Methodes Override

    public override void FirstFunc() {
        base.FirstFunc();

        cooldownManager.SetCooldownSingleCharge(slot, _info.Cooldown);
    }

    public override void FourthFunc() {
        base.FourthFunc();

        GraciaAttackManager.OnAttackHitAnOponnent -= _onHit;
        GraciaAttackManager.OnAttackHitAnOponnent += _onHit;

        GraciaPassiveManager.Instance.ChangeBarValue(_info.amountOfValueGainedWhenUsed, _info.typeOfSkill, _info.typeOfAura);

        UnblockInputs();

        _skillDurationRoutine ??= StartCoroutine(SkillDuration());
    }

    IEnumerator SkillDuration() {
        float timer = 0f;

        while (timer < _info.skillDuration) {
            timer += Time.deltaTime;
            yield return null;
        }

        _skillDurationRoutine = null;

        if (_waitToSpawnHitRoutine != null) {
            StopCoroutine(_waitToSpawnHitRoutine);
            _waitToSpawnHitRoutine = null;
        }

        GraciaAttackManager.OnAttackHitAnOponnent -= _onHit;
        End();
    }

    #endregion

    #region Instantiate

    void CallInstantiateHit() {
        _waitToSpawnHitRoutine ??= StartCoroutine(WaitToInstantiateHit());
    }

    IEnumerator WaitToInstantiateHit() {
        float attackSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        float timeToSpawnHit = _info.cooldownToHit / attackSpeed;

        yield return new WaitForSeconds(timeToSpawnHit);

        _waitToSpawnHitRoutine = null;

        InstantiateHit();
    }

    void InstantiateHit() {

        // Buscando hitbox na pool
        GameObject prefab = PoolingManager.Instance.ReturnPrefabFromPool(_info.Prefabs[0][0].PreFab, TypeOfSkillPrefab.Hitbox);

        // Buscando o atributo de acordo com o nível
        DamageAtributes atributes = _info.attackAtributesList[_skillLevel];

        // Settando o tamanho e a posição do ataque
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
