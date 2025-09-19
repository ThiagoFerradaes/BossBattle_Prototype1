using System.Collections;
using UnityEngine;

public class LilianWingsOfHorrorManager : SkillObjectManager
{
    // Components
    LilianWingsOfHorrorSO _info;
    Animator _wingsOfHorrorAnim;
    GameObject _wingsOfHorror;

    // Atributes
    bool _lilianIsInAnimation;

    // Corroutines
    Coroutine _wingsOfHorrorCoroutine, _wingsOfHorrorCooldownCoroutine;

    public override void UseSkill(SkillSO skill)
    {
        base.UseSkill(skill);

        if (_info == null) _info = skill as LilianWingsOfHorrorSO;
        if (_wingsOfHorrorAnim == null) _wingsOfHorrorAnim = GetComponentInChildren<Animator>(true);
        if (_wingsOfHorror == null) _wingsOfHorror = transform.GetChild(0).gameObject;

        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);

            animationCoroutine ??= StartCoroutine(LilianAnimation());
        }
        else
        {
            EndWithUnblockSkills();
        }
    }

    IEnumerator LilianAnimation()
    {
        // Definindo Cooldown
        cooldownManager.SetCooldownWithCharges(slot, _info);

        skillManager.SkillIsInAnimation(true);
        _lilianIsInAnimation = true;

        // Animation
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


            if (prefabInfo.PrefabType == TypeOfSkillPrefab.VFX)
            {
                GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
                preFab.transform.SetParent(parent.transform, false);
                preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

                preFab.GetComponent<VFXPreFab>().Initialize(prefabInfo.PrefabDuration);
            }
        }

        TurnWingsOfHorrorOn();

        // Esperando a animação terminar
        while (anim.GetCurrentAnimatorStateInfo(layer).fullPathHash == attackStateHash)
        {
            yield return null;
        }

        // Corrotina
        animationCoroutine = null;

        skillManager.SkillIsInAnimation(false);
        _lilianIsInAnimation = false;

        UnblockInputs();
    }

    void TurnWingsOfHorrorOn()
    {
        Vector3 foward = parent.transform.forward;
        foward.y = 0;   
        foward.Normalize();

        Vector3 spawnPoint = parent.transform.position + foward * _info.WingsOfHorrorDistance;
        spawnPoint.y = _info.WingsOfHorrorHeight;

        Vector3 dir = parent.transform.position - spawnPoint;
        dir.y = 0;
        dir.Normalize();

        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);

        transform.SetPositionAndRotation(spawnPoint, rotation);

        _wingsOfHorror.SetActive(true);

        _wingsOfHorrorCooldownCoroutine ??= StartCoroutine(WingsOfHorrorColldown(_info.WingsOfHorrorCooldown/2));
    }
    IEnumerator WingsOfHorrorAnimation()
    {
        // Animation
        _wingsOfHorrorAnim.SetTrigger(_info.WingsOfHorrorAnimationParameter);
        AnimatorStateInfo stateInfo;

        yield return null;

        int layer = 0; // Encontrando a layer da animação
        for (int i = 0; i < _wingsOfHorrorAnim.layerCount; i++)
        {
            AnimatorStateInfo state = _wingsOfHorrorAnim.GetCurrentAnimatorStateInfo(i);
            AnimatorStateInfo nextState = _wingsOfHorrorAnim.GetNextAnimatorStateInfo(i);

            if (state.IsName(_info.WingsOfHorrorAnimationName) || nextState.IsName(_info.WingsOfHorrorAnimationName))
            {
                layer = i;
                break;
            }
        }

        do
        { // Esperando entrar na animação
            yield return null;
            stateInfo = _wingsOfHorrorAnim.GetCurrentAnimatorStateInfo(layer);
        } while (!stateInfo.IsName(_info.WingsOfHorrorAnimationName));

        int attackStateHash = stateInfo.fullPathHash;

        var prefabList = _info.Prefabs[1];
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


            if (prefabInfo.PrefabType == TypeOfSkillPrefab.VFX)
            {
                GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
                preFab.transform.SetParent(parent.transform, false);
                preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

                preFab.GetComponent<VFXPreFab>().Initialize(prefabInfo.PrefabDuration);
            }
            else
            {
                GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);
                preFab.transform.SetParent(transform, false);
                preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);
                preFab.transform.localScale = Vector3.one * _info.WingsOfHorrorDamageSize;

                DamageContext newContext = new(
                    _info.MinDamage,
                    _info.MaxDamage,
                    prefabInfo.PrefabDuration,
                    _info.HitShield,
                    _info.DamageType,
                    _info.EnemyTag,
                    parent.GetComponent<StatusManager>()
                    );

                InstantDamageHitBox hitbox = preFab.GetComponent<InstantDamageHitBox>();
                hitbox.Initialize(newContext);

                hitbox.OnHit += () => {
                    energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
                };
            }
        }

        // Esperando a animação terminar
        while (_wingsOfHorrorAnim.GetCurrentAnimatorStateInfo(layer).fullPathHash == attackStateHash)
        {
            yield return null;
        }

        _wingsOfHorrorCoroutine = null;
        _wingsOfHorrorCooldownCoroutine ??= StartCoroutine(WingsOfHorrorColldown(_info.WingsOfHorrorCooldown));
    }

    IEnumerator WingsOfHorrorColldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        CheckTributes();
        _wingsOfHorrorCooldownCoroutine = null;
    }

    void CheckTributes()
    {
        if (LilianPassiveManager.Instance.ReturnAmountOfTributes() < _info.TributeCost) EndWithUnblockSkills();
        else
        {
            LilianPassiveManager.Instance.ChangeTributeAmount(-_info.TributeCost);
            _wingsOfHorrorCoroutine ??= StartCoroutine(WingsOfHorrorAnimation());
        }
    }

    public override void EndWithUnblockSkills()
    {
        if (_lilianIsInAnimation)
        {
            animationCoroutine = null;
            _lilianIsInAnimation = false;
            skillManager.SkillIsInAnimation(false);
            UnblockInputs();
        }

        cooldownManager.SetCooldownWithCharges(slot, _info);
        _wingsOfHorror.SetActive(false);

        _wingsOfHorrorCoroutine = null;
        _wingsOfHorrorCooldownCoroutine = null;

        base.EndWithUnblockSkills();
    }
}
