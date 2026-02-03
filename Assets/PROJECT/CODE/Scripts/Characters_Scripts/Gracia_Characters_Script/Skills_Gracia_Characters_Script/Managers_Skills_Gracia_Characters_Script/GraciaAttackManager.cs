using System.Collections;
using UnityEngine;

public class GraciaAttackManager : SkillObjectManager
{
    #region Paramethers

    // Components
    GraciaAttackSO _info;

    // Ints e floats
    int _attackIndex = 1;
    float _attackSpeedMultiplier = 1;

    // Corrotinas
    Coroutine _timerBetweenAttacksCoroutine;

    #endregion

    #region Initialize

    public override void UseSkill(SkillSO skill) {
        
        Initialize(skill);

        StartAnimation();
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as GraciaAttackSO;

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

        if (_timerBetweenAttacksCoroutine != null) {
            StopCoroutine(_timerBetweenAttacksCoroutine);
            _timerBetweenAttacksCoroutine = null;
        }
    }

    void StartAnimation() {
        string animationParameter = _attackIndex switch {
            1 => _info.FirstAttackAnimationParameter,
            2 => _info.SecondAttackAnimationParameter,
            3 => _info.ThirdAttackAnimationParameter,
            _ => _info.FirstAttackAnimationParameter
        };

        string animationName = _attackIndex switch {
            1 => _info.FirstAttackAnimationName,
            2 => _info.SecondAttackAnimationName,
            3 => _info.ThirdAttackAnimationName,
            _ => _info.FirstAttackAnimationName
        };

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, animationParameter, animationName, 0));
    }

    #endregion

    #region Animation Methodes Override


    public override void FirstFunc() {
        base.FirstFunc();

        _attackSpeedMultiplier = GetAttackSpeedMultiplier();

        anim.SetFloat(_info.AttackSpeedAnimationParameter, _attackSpeedMultiplier);
    }

    float GetAttackSpeedMultiplier() {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }

    public override void FourthFunc() {
        base.FourthFunc();

        // DEFININDO COOLDOWN
        // Aqui o cooldown pode ser 2: 1 -> entre os ataques do combo | 2 -> cooldown do final do combo
        float cooldown = _attackIndex < 3? _info.CooldownBetweenAttacks : _info.Cooldown;
        float realCooldown = cooldown / _attackSpeedMultiplier; // Se a velocidade de ataque for maior então o cooldown diminui
        cooldownManager.SetCooldownSingleCharge(slot, realCooldown);

        // SETANDO O INDEX DO PROXIMO ATAQUE
        float oldIndex = _attackIndex;
        if (_attackIndex < 3) _attackIndex++; // Se for menor que 3 ele sobe em um
        else _attackIndex = 1; // Se chegou no 3 então volta pro 1

        // VOLTANDO OS INPUTS
        UnblockInputs();

        // COMEÇANDO O TEMPORIZADOR ENTRE ATAQUES
        if (oldIndex < 3) _timerBetweenAttacksCoroutine ??= StartCoroutine(CooldownBetweenAttacks());
        else End();
    }

    IEnumerator CooldownBetweenAttacks() {
        float timer = 0;

        while (timer <= _info.MaxTimeBetweenAttacks) {
            timer += Time.deltaTime;
            yield return null;
        }

        _attackIndex = 1;
        _timerBetweenAttacksCoroutine = null;
        End();
    }
    #endregion

    #region Instantiate




    #endregion
}
