using System;
using System.Collections;
using UnityEditorInternal;
using UnityEngine;

public class GraciaRedAuraManager : SkillObjectManager
{
    #region Paramethers

    // Components
    GraciaRedAuraSO _info;

    // Int
    int _skillLevel;

    // Coroutines
    Coroutine _skillDurationRoutine;
    #endregion

    #region Initialize

    public override void UseSkill(SkillSO skill) {
        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AttackAnimationParameter, _info.AttackAnimationName, 0));
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as GraciaRedAuraSO;
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
        _skillLevel = GraciaPassiveManager.Instance.ReturnCurrentSkillArea(_info.TypeOfSkill);
    }


    #endregion

    #region Animation Methodes Override

    public override void FirstFunc() {
        base.FirstFunc();

        cooldownManager.SetCooldownSingleCharge(slot,_info.Cooldown);
    }

    public override void ThirdFunc() {
        GraciaPassiveManager.Instance.ChangeBarValue(_info.AmountOfValueGainedWhenUsed, _info.TypeOfSkill, _info.TypeOfAura);

        IncreaseCritValue();
    }

    void IncreaseCritValue() {
        CritRatePerAttackIndex newCrit = _info.AditionalCriRateList[_skillLevel];
        GraciaPassiveManager.Instance.SetCritRate(newCrit);
        float newDamage = _info.CritDamageIncreaseList[_skillLevel];
        GraciaPassiveManager.Instance.SetCritDamage(newDamage);
    }
    public override void FourthFunc() {
        base.FourthFunc();

        _skillDurationRoutine ??= StartCoroutine(SkillDuration());

        UnblockInputs();
    }
    void DecreaseCritValue() {
        CritRatePerAttackIndex newCrit = new(0, 0, 0);
        GraciaPassiveManager.Instance.SetCritRate(newCrit);
        GraciaPassiveManager.Instance.SetCritDamage(0);
    }
    IEnumerator SkillDuration() {
        float timer = 0;

        while (timer < _info.SkillDuration) {
            timer += Time.deltaTime;
            yield return null;
        }

        _skillDurationRoutine = null;
        DecreaseCritValue();
        End();
    }
    #endregion

    #region Instantiate



    #endregion
}
