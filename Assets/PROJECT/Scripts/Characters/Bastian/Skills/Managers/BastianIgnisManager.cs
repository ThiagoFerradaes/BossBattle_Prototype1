using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BastianIgnisManager : SkillObjectManager
{
    // Components
    BastianIgnisSO _info;

    public override void HandleInput(SkillSO skill, InputAction.CallbackContext ctx)
    {
        if (!BastianPassiveManager.Instance.CanShoot)
        {
            return;
        }

        if (ctx.phase == InputActionPhase.Started)
        {
            _preCasted = true;
            OnPreCast(skill);
        }
        if (ctx.phase == InputActionPhase.Canceled && _preCasted)
        {
            OnRelease(skill);
        }
    }

    public override void UseSkill(SkillSO skill)
    {


        if (_info == null) _info = skill as BastianIgnisSO;

        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

        animationCoroutine ??= StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        // Definindo Cooldown

        cooldownManager.SetCooldownWithCharges(slot, _info);

        float attackSpeedMultiplier = GetAttackSpeedMultiplier();

        skillManager.SkillIsInAnimation(true);

        // Animation
        anim.SetFloat(_info.AttackSpeedAnimationParameter, attackSpeedMultiplier);
        anim.SetTrigger(_info.AnimationParameter);
        AnimatorStateInfo stateInfo;

        yield return null;

        int layer = 0; // Encontrando a layer da animação
        for (int i = 0; i < anim.layerCount; i++)
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(i);
            AnimatorStateInfo nextState = anim.GetNextAnimatorStateInfo(i);

            if (state.IsName(_info.AnimationName) || nextState.IsName(_info.AnimationName))
            {
                layer = i;
                break;
            }
        }

        do
        { // Esperando entrar na animação
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(layer);
        } while (!stateInfo.IsName(_info.AnimationName));

        int attackStateHash = stateInfo.fullPathHash;

        // Pegando a lista de prefabs e ordenando pelo tempo de spawn delas
        var prefabList = _info.Prefabs[0];
        prefabList.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

        // Instanciando prefabs
        for (int i = 0; i < prefabList.Count; i++)
        {
            SkillAnimationEvent prefabInfo = prefabList[i];
            float targetNormalizedTime = prefabInfo.TimeToSpawnPreFab;

            do
            { // Esperando o tempo para instanciar hit box
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(layer);
            } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);


            if (prefabInfo.PrefabType == TypeOfSkillPrefab.Hitbox)
            {

                GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

                preFab.transform.SetPositionAndRotation(parent.transform.position + prefabInfo.PreFabPosition, parent.transform.rotation);

                float pen = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.SuperHeatArea) ? _info.PenetrationOnSuperHeat : 0;
                float critChance = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.OverHeatArea) ? _info.CritChanceOverHeat : 0;
                float additionalCriDmg = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.LastOverHeatArea) ? _info.LastOverHeatCritDamage : 0;
                float critDamage = statusManager.ReturnStatusValue(StatusType.CritDamage) + additionalCriDmg;

                DamageContext newContext = new(
                    _info.AttackMinDamage,
                    _info.AttackMaxDamage,
                    prefabInfo.PrefabDuration,
                    _info.HitShield,
                    _info.DamageType,
                    _info.EnemyTag,
                    parent.GetComponent<StatusManager>(),
                    new() {
                        {ExtraDamageContextAtributes.Speed, _info.ProjectileSpeed },
                        {ExtraDamageContextAtributes.Distance, _info.AttackDistance },
                        {ExtraDamageContextAtributes.Penetration, pen},
                        {ExtraDamageContextAtributes.CritChance, critChance},
                        {ExtraDamageContextAtributes.CritDamage, critDamage},
                        {ExtraDamageContextAtributes.CrossEnemy, _info.CrossTarget},

                    }
                    );

                ProjectileDamageHitBox hitbox = preFab.GetComponent<ProjectileDamageHitBox>();
                hitbox.Initialize(newContext);

                hitbox.OnHit += () => {
                    energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
                };

                if (BastianPassiveManager.Instance.ReturnMaxHeat(HeatArea.SuperHeatArea))
                    BastianPassiveManager.Instance.GainHeat(_info.HeatGain);
                else BastianPassiveManager.Instance.GainHeat(1);

            }
            else
            {
                GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
                preFab.transform.SetParent(parent.transform, false);
                preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

                preFab.GetComponent<VFXPreFab>().Initialize(prefabInfo.PrefabDuration);
            }
        }

        // Esperando a animação terminar
        while (anim.GetCurrentAnimatorStateInfo(layer).fullPathHash == attackStateHash)
        {
            yield return null;
        }


        // Resetando a velocidade da animação
        anim.SetFloat(_info.AttackSpeedAnimationParameter, 1);

        // Corrotina
        animationCoroutine = null;

        skillManager.SkillIsInAnimation(false);

        End();
    }
    float GetAttackSpeedMultiplier()
    {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }

}
