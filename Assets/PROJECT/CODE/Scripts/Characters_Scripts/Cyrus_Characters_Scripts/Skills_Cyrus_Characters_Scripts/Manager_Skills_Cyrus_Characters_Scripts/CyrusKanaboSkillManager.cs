using System.Collections;
using UnityEngine;

public class CyrusKanaboSkillManager : SkillObjectManager {

    CyrusKanaboSkillSO _info;

    int _skillLevel;

    Coroutine _explosionCoroutine;

    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        Initialize(skill);

        StartCoroutine(AttackCoroutine());
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as CyrusKanaboSkillSO;

        _skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    protected override void FirstFunc() {
        base.FirstFunc();

        cooldownManager.SetCooldownSingleCharge(slot, _info.Cooldown);
    }

    protected override void FourthFunc() {
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
            if (_skillLevel < 3) CyrusPassiveManager.Instance.AddUseSkill(slot, _info.AmountOfUsesPerLevel[_skillLevel], _info.ListOfSprites);
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
            for (int j = 0; j < _info.Prefabs[1].Count; j++) {

                if (_info.Prefabs[1][j].PrefabType == TypeOfSkillPrefab.Hitbox) {

                    InstantiateExplosion(position, _info.Prefabs[1][j].PreFab);

                }
                else if (_info.Prefabs[1][j].PrefabType == TypeOfSkillPrefab.VFX) {

                    InstantiateVFX(_info.Prefabs[1][j], position);

                }

                if (i < amountOfExplosions - 1) yield return new WaitForSeconds(_info.TimeBetweenExplosions);
            }

            _explosionCoroutine = null;

            End();
        }

        void InstantiateContinuosArea(Vector3 position) {

            if (_info.Prefabs[2].Count == 0) return; // Verificando se a lista aqui existe

            for (int i = 0; i < _info.Prefabs[2].Count; i++) {

                if (_info.Prefabs[2][i].PrefabType == TypeOfSkillPrefab.Hitbox) {

                    GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(_info.Prefabs[2][i].PreFab, TypeOfSkillPrefab.Hitbox);

                    Vector3 size = _skillLevel < 3 ? _info.ContinuosDamageAreaAtributes.Size : _info.ExplosionRadiusLevelThree * Vector3.one;
                    hitbox.transform.localScale = size;
                    hitbox.transform.SetPositionAndRotation(position, Quaternion.identity);

                    DamageContext newContext = new(_info.ContinuosDamageAreaAtributes, statusManager);

                    ContinuosDamageHitBox collider = hitbox.GetComponent<ContinuosDamageHitBox>();
                    collider.Initialize(newContext);
                }
                else if (_info.Prefabs[2][i].PrefabType == TypeOfSkillPrefab.VFX) {

                    InstantiateVFX(_info.Prefabs[2][i]);

                }
            }
        }

        void InstantiateExplosion(Vector3 position, GameObject explosionPrefab) {
            GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(explosionPrefab, TypeOfSkillPrefab.Hitbox);

            Vector3 size = _skillLevel < 3 ? _info.ExplosionAtributes.Size : _info.ExplosionRadiusLevelThree * Vector3.one;
            hitbox.transform.localScale = size;
            hitbox.transform.SetPositionAndRotation(position, Quaternion.identity);

            DamageAtributes newAtributes = new(_info.ExplosionAtributes);

            DamageContext newContext = new(newAtributes, statusManager);

            InstantDamageHitBox collider = hitbox.GetComponent<InstantDamageHitBox>();
            collider.Initialize(newContext);
        }
    }
}
