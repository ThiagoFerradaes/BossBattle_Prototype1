using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilianLightGodManager : SkillObjectManager
{
    #region Variables
    LilianLightGodSO _info;

    [SerializeField] List<GameObject> listOfGodsObjects = new();
    int godIndex;

    Coroutine _selfDamageRoutine;
    Action<float> _onHeal;
    #endregion

    #region Initialize
    public override void UseSkill(SkillSO skill)
    {
        base.UseSkill(skill);

        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AnimationParameter, _info.AnimationName, 0));
    }

    void Initialize(SkillSO skill)
    {
        if (_info == null) _info =  skill as LilianLightGodSO;

        transform.SetParent(parent.transform, false);
        transform.SetLocalPositionAndRotation(_info.ManagerLocalPosition, Quaternion.identity);

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

        _onHeal = (float value) => { TurnGodOn(); };
    }
    #endregion

    #region Override
    public override void FirstFunc()
    {
        base.FirstFunc();

        energyManager.SetCanGainEnergy(false);
        energyManager.LooseAllEnergy();
    }

    public override void ThirdFunc()
    {
        healthManager.Heal(_info.HealthToHealBeforeUlt);

        TurnGodOn();

        _selfDamageRoutine ??= StartCoroutine(SelfDamageCooldownRoutine());

        healthManager.OnHeal += _onHeal;
    }

    public override void FourthFunc()
    {
        base.FourthFunc();

        UnblockInputs();
    }
    #endregion

    void TurnGodOn()
    {

        if (godIndex < listOfGodsObjects.Count - 1)
        {
            listOfGodsObjects[godIndex].SetActive(true);

            godIndex++;
        }
        else
        {
            Debug.Log("Atirar");
            End();
        }
    }
    
    IEnumerator SelfDamageCooldownRoutine()
    {
        float currentHealthPercent = healthManager.ReturnCurrentHealth() / healthManager.ReturnMaxHealth();

        while (currentHealthPercent > _info.PercentOfMinHealth/100)
        {
            healthManager.TakeDamage(_info.SelfDamageLostOverTime);
            currentHealthPercent = healthManager.ReturnCurrentHealth() / healthManager.ReturnMaxHealth();
            yield return new WaitForSeconds(_info.CooldownBetweenSelfDamage);
        }

    }

    public override void End()
    {
        healthManager.OnHeal -= _onHeal;

        godIndex = 0;

        if (_selfDamageRoutine != null)
        {
            StopCoroutine(_selfDamageRoutine);
            _selfDamageRoutine = null;
        }

        foreach(var god in listOfGodsObjects)
        {
            god.SetActive(false);
        }

        energyManager.SetCanGainEnergy(true);

        base.End();
    }
}
