using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CyrusOrbsSkillManager : SkillObjectManager {
    CyrusOrbsSkillSO _info;

    int _skillLevel;

    int _currentAmountOfOrbs;

    Coroutine _durationRoutine, _orbRoutine;

    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AnimationParameter, _info.AnimationName, 0));
    }

    private void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as CyrusOrbsSkillSO;

        _skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    public override void FirstFunc() {
        base.FirstFunc();

        float cooldown = _skillLevel > 0 ? _info.OrbCooldownLevelOne : _info.Cooldown;
        cooldownManager.SetCooldownSingleCharge(slot, cooldown);
    }

    public override void FourthFunc() {
        base.FourthFunc();

        UnblockInputs();

        if (_skillLevel < 3) End();
        else HoldOrb();
    }

    void HoldOrb() {
        if (_currentAmountOfOrbs < _info.MaxAmountOfOrbs - 1) _currentAmountOfOrbs++;

        if (_durationRoutine != null) {
            StopCoroutine(_durationRoutine);
            _durationRoutine = StartCoroutine(Duration());
        }
        else {
            _durationRoutine = StartCoroutine(Duration());
        }
    }
    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.TimeHoldingOrb);
        End();
    }

    public override void End() {

        _currentAmountOfOrbs = 0;

        base.End();
    }
    public override void InstantiateHitBox(SkillAnimationEvent prefab) {

        _orbRoutine ??= StartCoroutine(InstantiateOrb(prefab));
    }

    IEnumerator InstantiateOrb(SkillAnimationEvent prefab) {
        bool hasHit = false;

        for (int i = 0; i <= _currentAmountOfOrbs; i++) {

            GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);

            // Tamanho
            hitbox.transform.localScale = _info.SkillDamageAtributes.Size;

            // Posição
            hitbox.transform.SetParent(parent.transform);
            hitbox.transform.SetLocalPositionAndRotation(prefab.PreFabPosition, Quaternion.identity);
            hitbox.transform.SetParent(null);

            // Ativando a hitbox
            DamageAtributes newAtributes = new(_info.SkillDamageAtributes);
            if (_skillLevel > 0) newAtributes.Speed = _info.OrbSpeedLevelOne;
            if (_skillLevel > 1) newAtributes.ExtraAtributes[ExtraDamageContextAtributes.CritRate] = _info.OrbCritRateLevelTwo;

            DamageContext newContext = new(newAtributes, statusManager);

            BoomerangDamageHitBox collider = hitbox.GetComponent<BoomerangDamageHitBox>();
            collider.Initialize(newContext);

            collider.OnHit += (GameObject obj) => {

                energyManager.GainEnergy(_info.FlatEnergyGainPerHit);

                if (!hasHit) {
                    hasHit = true;
                    if (_skillLevel < 3) CyrusPassiveManager.Instance.AddUseSkill(slot, _info.AmountOfUsesPerLevel[_skillLevel]);
                }
            };

            if (i < _currentAmountOfOrbs) yield return new WaitForSeconds(_info.TimeBetweenEachOrb);
        }

        _orbRoutine = null;
    }
}
