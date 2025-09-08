using System.Collections;
using UnityEngine;

public class BastianReleaseManager : SkillObjectManager
{
    BastianReleaseSO _info;

    Coroutine _attackCoroutine;
    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        if(_info == null) _info = skill as BastianReleaseSO;

        gameObject.SetActive(true);

        _attackCoroutine ??= StartCoroutine(Attack());
    }

    IEnumerator Attack() {
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
                GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
                preFab.transform.SetParent(parent.transform, false);
                preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

                preFab.GetComponent<VFXPreFab>().Initialize(prefabInfo.PrefabDuration);
            }
        }

        BastianPassiveManager.Instance.LooseHeat(_info.HeatLost);
        statusManager.ChangeStatus(StatusType.AttackSpeed, _info.AttackSpeedGain, true, _info.AttackSpeedDuration);

        // Esperando a animação terminar
        while (anim.GetCurrentAnimatorStateInfo(layer).fullPathHash == attackStateHash) {
            yield return null;
        }

        // Definindo Cooldown
        cooldownManager.SetCooldown(slot, _info.Cooldown);

        // Corrotina
        _attackCoroutine = null;

        // Desbloqueando inputs
        UnblockInputs();

        End();
    }

    void End() {
        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Manager);
    }

}
