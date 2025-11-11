using System.Collections;
using UnityEngine;

public class CyrusKanaboSkillManager : SkillObjectManager
{

    CyrusKanaboSkillSO _info;

    int _skillLevel;

    Coroutine _explosionCoroutine;

    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        Initialize(skill);

        StartCoroutine(AttackCoroutine(0, _info.AnimationParameter, _info.AnimationName, 0));
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as CyrusKanaboSkillSO;

        _skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    public override void FirstFunc() {
        base.FirstFunc();

        cooldownManager.SetCooldownSingleCharge(slot, _info.Cooldown);
        Debug.Log("old level = " + _skillLevel);
        if (_skillLevel < 3) CyrusPassiveManager.Instance.AddUseSkill(slot, _info.AmountOfUsesPerLevel[_skillLevel]);
        Debug.Log("new level = " + _skillLevel);
    }

    public override void FourthFunc() {
        base.FourthFunc();

        if (_skillLevel > 0) UnblockInputs();
        else EndWithUnblockSkills();
    }

    public override void InstantiateHitBox(SkillAnimationEvent prefab) {

        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);

        hitbox.transform.SetParent(parent.transform, false);
        hitbox.transform.SetLocalPositionAndRotation(prefab.PreFabPosition, Quaternion.identity);
        hitbox.transform.SetParent(null);

        Vector3 hitboxPos = hitbox.transform.position;

        hitbox.transform.localScale = _info.SkillDamageAtributes.Size;

        DamageContext newContext = new(_info.SkillDamageAtributes, statusManager);

        InstantDamageHitBox collider = hitbox.GetComponent<InstantDamageHitBox>();
        collider.Initialize(newContext);

        collider.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);

        };


        if (_skillLevel > 0) _explosionCoroutine ??= StartCoroutine(ExplosionCoroutine(hitboxPos));
        
    }

    IEnumerator ExplosionCoroutine(Vector3 position) {

        if (_skillLevel >= 3) InstantiateContinuosArea(position);

        yield return new WaitForSeconds(_info.TimeBetweenHitAndExplosion);

        float amountOfExplosions = _skillLevel switch {
            1 => _info.AmountOfExplosionLevelOne,
            2 => _info.AmountOfExplosionLevelTwo,
            3 => _info.AmountOfExplosionLevelThree,
            _ => 1
        };

        for (int i = 0; i < amountOfExplosions; i++) {

            InstantiateExplosion(position);

            if (i < amountOfExplosions - 1) yield return new WaitForSeconds(_info.TimeBetweenExplosions);
        }

        _explosionCoroutine = null;

        End();
    }

    void InstantiateContinuosArea(Vector3 position) {

        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(_info.ContinuosDamagePrefab, TypeOfSkillPrefab.Hitbox);

        Vector3 size = _skillLevel < 3 ? _info.ContinuosDamageAreaAtributes.Size : _info.ExplosionRadiusLevelThree * Vector3.one;
        hitbox.transform.localScale = size;
        hitbox.transform.SetPositionAndRotation(position, Quaternion.identity);

        DamageContext newContext = new(_info.ContinuosDamageAreaAtributes, statusManager);

        ContinuosDamageHitBox collider = hitbox.GetComponent<ContinuosDamageHitBox>();
        collider.Initialize(newContext);
    }

    void InstantiateExplosion(Vector3 position) {
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(_info.ExplosionPrefab, TypeOfSkillPrefab.Hitbox);

        Vector3 size = _skillLevel < 3 ? _info.ExplosionAtributes.Size : _info.ExplosionRadiusLevelThree * Vector3.one;
        hitbox.transform.localScale = size;
        hitbox.transform.SetPositionAndRotation(position, Quaternion.identity);

        DamageAtributes newAtributes = _info.ExplosionAtributes;

        if (_skillLevel > 1) 
            newAtributes.ExtraAtributes[ExtraDamageContextAtributes.CritRate] = _info.ExplosionCritRateLevelTwo;

        if (_skillLevel > 2)
            newAtributes.ExtraAtributes[ExtraDamageContextAtributes.CritDamage] = _info.ExplosionCritDamageLevelThree;

        DamageContext newContext = new(newAtributes, statusManager);

        InstantDamageHitBox collider = hitbox.GetComponent<InstantDamageHitBox>();
        collider.Initialize(newContext);
    }
}
