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

        animationCoroutine ??= StartCoroutine(AttackCoroutine());
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

    protected override void FirstFunc() {
        base.FirstFunc();

        cooldownManager.SetCooldownSingleCharge(slot, _info.Cooldown);
    }

    protected override void ThirdFunc() {
        IncreaseAttackSpeed();
        _passiveManager.ChangeBarValue(_info.AmountOfValueGainedWhenUsed, _info.TypeOfAura);
    }

    void IncreaseAttackSpeed() {
        int skillLevel = _passiveManager.ReturnCurrentSkillArea(_info.TypeOfSkill);
        _attackSpeedMultiplier = _info.AttackSpeedBuffList[skillLevel].Value;
        statusManager.ChangeStatusMultiplier(StatusType.AttackSpeed, _attackSpeedMultiplier, true);

        _skillDurationRoutine ??= StartCoroutine(SkillDuration());
    }

    IEnumerator SkillDuration() {
        float timer = 0;

        while (timer < _info.SkillDuration) {
            timer += Time.deltaTime;
            yield return null;
        }

        _skillDurationRoutine = null;

        ResetAttackSpeed();
        End();
    }

    void ResetAttackSpeed() {
        statusManager.ChangeStatusMultiplier(StatusType.AttackSpeed, _attackSpeedMultiplier, false);
    }

    protected override void FourthFunc() {
        base.FourthFunc();

        UnblockInputs();
    }
    #endregion

    #region Instantiate



    #endregion


}


