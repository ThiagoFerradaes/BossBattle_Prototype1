using System.Collections;
using UnityEngine;

public class LilianSacrificeManager : SkillObjectManager
{
    // Components
    LilianSacrificeSO _info;
    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        if (_info == null) _info = skill as LilianSacrificeSO;

        gameObject.SetActive(true);

        animationCoroutine ??= StartCoroutine(Attack());
    }

    IEnumerator Attack() {
        // Definindo Cooldown
        cooldownManager.SetCooldownWithCharges(slot, _info);

        skillManager.SkillIsInAnimation(true);

        // Animation
        anim.SetTrigger(_info.AnimationParameter);
        AnimatorStateInfo stateInfo;

        yield return null;

        int layer = 0; // Encontrando a layer da animação
        for (int i = 0; i < anim.layerCount; i++) {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(i);
            AnimatorStateInfo nextState = anim.GetNextAnimatorStateInfo(i);

            if (state.IsName(_info.AnimationName) || nextState.IsName(_info.AnimationName)) {
                layer = i;
                break;
            }
        }

        do { // Esperando entrar na animação
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(layer);
        } while (!stateInfo.IsName(_info.AnimationName));

        int attackStateHash = stateInfo.fullPathHash;

        var prefabList = _info.Prefabs[0];
        prefabList.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

        // Instanciando prefabs
        for (int i = 0; i < prefabList.Count; i++) {
            SkillAnimationEvent prefabInfo = prefabList[i];
            float targetNormalizedTime = prefabInfo.TimeToSpawnPreFab;

            do { // Esperando o tempo para instanciar hit box
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(layer);
            } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);


            if (prefabInfo.PrefabType == TypeOfSkillPrefab.VFX) {
                GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
                preFab.transform.SetParent(parent.transform, false);
                preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

                preFab.GetComponent<VFXPreFab>().Initialize(prefabInfo.PrefabDuration);
            }
        }

        GainTributesAndLooseHealth();

        // Esperando a animação terminar
        while (anim.GetCurrentAnimatorStateInfo(layer).fullPathHash == attackStateHash) {
            yield return null;
        }

        // Corrotina
        animationCoroutine = null;

        skillManager.SkillIsInAnimation(false);

        EndWithUnblockSkills();
    }

    void GainTributesAndLooseHealth() {
        float oldHealth = healthManager.ReturnCurrentHealth();
        float healthToLoose = oldHealth * _info.PercentOfCurrentHealthToLoose/100;
        healthManager.TakeDamage(healthToLoose, false);

        float tributes = healthToLoose * _info.AmountOfTributesGainPerHealthLost;

        LilianPassiveManager.Instance.ChangeTributeAmount(tributes);
    }
}
