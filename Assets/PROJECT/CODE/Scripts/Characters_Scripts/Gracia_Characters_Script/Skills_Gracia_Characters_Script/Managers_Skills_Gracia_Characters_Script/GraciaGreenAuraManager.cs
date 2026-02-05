using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;

public class GraciaGreenAuraManager : SkillObjectManager
{
    #region Paramethers

    // Components 
    GraciaGreenAuraSO _info;

    // int
    int _skillLevel;

    // Coroutines
    Coroutine _durationRoutine;

    // Actions
    Action _onHit;

    #endregion

    #region Initialize

    public override void UseSkill(SkillSO skill) {
        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.attackAnimationParameter, _info.attackAnimationName, 0));
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as GraciaGreenAuraSO;
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
        _onHit = CreateShield;
        _skillLevel = GraciaPassiveManager.Instance.ReturnCurrentSkillArea(_info.typeOfSkill);
    }

    #endregion

    #region Animation Methods Override

    public override void FirstFunc() {
        base.FirstFunc();

        cooldownManager.SetCooldownSingleCharge(slot, _info.Cooldown);
    }

    public override void FourthFunc() {
        base.FourthFunc();

        GraciaAttackManager.OnAttackHitAnOponnent -= _onHit;
        GraciaAttackManager.OnAttackHitAnOponnent += _onHit;

        GraciaPassiveManager.Instance.ChangeBarValue(_info.amountOfValueGainedWhenUsed, _info.typeOfSkill, _info.TypeOfAura);

        UnblockInputs();

        _durationRoutine ??= StartCoroutine(SkillDuration());
    }

    IEnumerator SkillDuration() {
        float timer = 0f;

        while (timer < _info.skillDuration) {
            timer += Time.deltaTime;
            yield return null;
        }

        _durationRoutine = null;
        GraciaAttackManager.OnAttackHitAnOponnent -= _onHit;
        End();
    }

    void CreateShield() {
        float amountOfShield = _info.shieldAmountPerLevel[_skillLevel];
        healthManager.RecieveShield(amountOfShield, _info.shieldDuration);
    }
    #endregion


}
