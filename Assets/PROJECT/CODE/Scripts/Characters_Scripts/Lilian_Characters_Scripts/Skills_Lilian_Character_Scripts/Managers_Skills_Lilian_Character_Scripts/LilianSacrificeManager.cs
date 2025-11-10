using System.Collections;
using UnityEngine;

public class LilianSacrificeManager : SkillObjectManager {
    // Components
    LilianSacrificeSO _info;
    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        if (_info == null) _info = skill as LilianSacrificeSO;

        gameObject.SetActive(true);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AnimationParameter, _info.AnimationName, 0));
    }

    public override void FirstFunc() {
        base.FirstFunc();

        // Definindo Cooldown
        cooldownManager.SetCooldownWithCharges(slot, _info);
    }

    public override void ThirdFunc() {
        base.ThirdFunc();

        healthManager.TakeDamage(_info.PercentOfCurrentHealthToLoose / 100 * healthManager.ReturnCurrentHealth());

        float lerp = Mathf.InverseLerp(_info.HealthLimit, healthManager.ReturnMaxHealth(), healthManager.ReturnCurrentHealth());
        float t = 1 - lerp;
        float shield = Mathf.Lerp(_info.AmountOfShieldGainBasedOnHealth.x, _info.AmountOfShieldGainBasedOnHealth.y, t);
        
        healthManager.RecieveShield(shield, _info.ShieldDuration);
    }

    public override void FourthFunc() {
        base.FourthFunc();


        EndWithUnblockSkills();
    }

}
