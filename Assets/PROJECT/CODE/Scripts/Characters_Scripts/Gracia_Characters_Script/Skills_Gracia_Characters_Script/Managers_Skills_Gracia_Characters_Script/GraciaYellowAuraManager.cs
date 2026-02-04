using System.Collections;
using UnityEngine;

public class GraciaYellowAuraManager : SkillObjectManager {
    #region

    // Components
    GraciaYellowAuraSO _info;
    GraciaPassiveManager _passiveManager;

    // float
    float _attackSpeedMultiplier;

    // Coroutines
    Coroutine _skillDurationRoutine;
    #endregion

    #region Initialize

    public override void UseSkill(SkillSO skill) {
        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.attackAnimationParameter, _info.attackAnimationName, 0));
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as GraciaYellowAuraSO;
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
        if (_passiveManager == null) _passiveManager = GraciaPassiveManager.Instance;
        if (_skillDurationRoutine != null) {
            StopCoroutine(_skillDurationRoutine);
            _skillDurationRoutine = null;
            ResetAttackSpeed();
        }
    }

    #endregion

    #region Animation Methodes Override

    public override void FirstFunc() {
        base.FirstFunc();

        cooldownManager.SetCooldownSingleCharge(slot, _info.Cooldown);
    }

    public override void ThirdFunc() {
        IncreaseAttackSpeed();
        _passiveManager.ChangeBarValue(_info.amountOfValueGainedWhenUsed, _info.typeOfSkill, _info.typeOfAura);
    }

    void IncreaseAttackSpeed() {
        int skillLevel = _passiveManager.ReturnCurrentSkillArea(_info.typeOfSkill);
        _attackSpeedMultiplier = _info.attackSpeedBuffList[skillLevel].Value;
        statusManager.ChangeStatus(StatusType.AttackSpeed, _attackSpeedMultiplier, true);

        _skillDurationRoutine ??= StartCoroutine(SkillDuration());
    }

    IEnumerator SkillDuration() {
        float timer = 0;

        while (timer < _info.skillDuration) {
            timer += Time.deltaTime;
            yield return null;
        }

        _skillDurationRoutine = null;

        ResetAttackSpeed();
        End();
    }

    void ResetAttackSpeed() {
        statusManager.ChangeStatus(StatusType.AttackSpeed, _attackSpeedMultiplier, false);
    }

    public override void FourthFunc() {
        base.FourthFunc();

        UnblockInputs();
    }
    #endregion

    #region Instantiate



    #endregion
}


