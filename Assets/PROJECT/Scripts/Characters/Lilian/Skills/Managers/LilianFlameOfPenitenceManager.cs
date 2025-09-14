using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilianFlameOfPenitenceManager : SkillObjectManager
{
    LilianFlameOfPenitenceSO _info;
    List<LilianCandle> _candles = new();
    
    public override void UseSkill(SkillSO skill)
    {
        base.UseSkill(skill);

        if (_info == null) _info = skill as LilianFlameOfPenitenceSO;

        gameObject.SetActive(true);

        animationCoroutine ??= StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        // Definindo Cooldown
        cooldownManager.SetCooldownWithCharges(slot, _info);

        skillManager.SkillIsInAnimation(true);

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

        HandleCandle();

        // Esperando a animação terminar
        while (anim.GetCurrentAnimatorStateInfo(layer).fullPathHash == attackStateHash)
        {
            yield return null;
        }

        // Corrotina
        animationCoroutine = null;

        skillManager.SkillIsInAnimation(false);

        End();
    }

    void HandleCandle()
    {
        if (CheckIfAmountOfCandlesIsInsideLimite(_candles.Count))
        {
            InstantiateCandle();
        }
        else
        {
            _candles[0].Explode();

            InstantiateCandle();
        }
    }

    void InstantiateCandle()
    {
        GameObject candle = PoolingManager.Instance.ReturnPrefabFromPool(_info.CandlePrefabName, _info.CandlePrefab, TypeOfSkillPrefab.Hitbox);

        Vector3 foward = parent.transform.forward;
        foward.y = 0;
        foward.Normalize();

        Vector3 spawnPoint = parent.transform.position + foward * _info.CandleFowardDistance;
        spawnPoint.y = _info.CandleHeight;

        candle.transform.position = spawnPoint;

        LilianCandle lilianCandle = candle.GetComponent<LilianCandle>();
        lilianCandle.TurnCandleOn(_info, parent);
        lilianCandle.OnDeath += RemoveCandleFromList;
        _candles.Add(lilianCandle);
    }

    void RemoveCandleFromList(LilianCandle candleToRemove)
    {
        if (!_candles.Contains(candleToRemove)) return;

        _candles.Remove(candleToRemove);
    }

    bool CheckIfAmountOfCandlesIsInsideLimite(int amount)
    {
        return amount < _info.CandleInitialLimit + LilianPassiveManager.Instance.ReturnAmountOfCorruption();
    }
}
