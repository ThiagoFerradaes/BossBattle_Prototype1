using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class GraciaDanceUltimateManager : SkillObjectManager
{
    #region Paramethers

    // Components
    GraciaDanceUltimateSO _info;

    // Int
    int _skillLevel, _amountOfShieldsGained;

    // Bool
    bool _canEnd;

    // Hitbox
    ContinuosDamageHitBox _greenHitbox;

    // Coroutine
    Coroutine _yellowSkillDurationRoutine, _greenSkillDurationRoutine;

    // Actions
    Action _onBaseAttack, _onGainShield;
    #endregion

    #region Initialize

    public override void UseSkill(SkillSO skill)
    {
        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine());
    }

    void Initialize(SkillSO skill)
    {
        if (_info == null) _info = skill as GraciaDanceUltimateSO;
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    #endregion

    #region Animation Methods Override

    public override void FirstFunc()
    {
        base.FirstFunc();

        energyManager.SetCanGainEnergy(false);
    }
    public override void ThirdFunc()
    {
        DecideBehaviour();
    }
    public override void FourthFunc()
    {
        base.FourthFunc();

        UnblockInputs();
        if (_canEnd) { _canEnd = false; End(); }
    }

    #endregion

    #region Behaviours

    void DecideBehaviour()
    {
        GraciaAura currentAura = GraciaPassiveManager.Instance.ReturnCurrentAura();

        switch (currentAura)
        {
            case GraciaAura.Blue: BlueBehaviour(); break;
            case GraciaAura.Yellow: YellowBehaviour(); break;
            case GraciaAura.Red: RedBehaviour(); break;
            case GraciaAura.Green: GreenBehaviour();  break;
        }
    }

    #region Blue Region
    void BlueBehaviour()
    {
        // Verificando o nível da habilidade
        _skillLevel = GraciaPassiveManager.Instance.ReturnCurrentSkillArea(GraciaTypeOfSkill.Left);

        // Drenando a barra
        GraciaPassiveManager.Instance.ChangeBarValue(-_info.BlueAmountOfAuraConsumed, GraciaAura.Blue);

        // Instanciando Hitbox e VFX
        InstantiateBlueEffects();

        // Finalizando supremo
        energyManager.LooseAllEnergy();
        energyManager.SetCanGainEnergy(true);
        _canEnd = true;
    }

    void InstantiateBlueEffects() {
        // Verificando se existe algum prefab na lista
        if (_info.Prefabs[0].Count == 0) return;

        for (int i = 0; i < _info.Prefabs[0].Count; i++) {
            if (_info.Prefabs[0][i].PrefabType == TypeOfSkillPrefab.Hitbox) { // HITBOX 
                GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(_info.Prefabs[0][i].PreFab, TypeOfSkillPrefab.Hitbox);

                // Alterando os atributos pelo nível
                DamageAtributes newAtributes = new(_info.BlueAtributes);
                newAtributes.Size *= ( 1 + _info.BlueSizeIncreasePerLevel[_skillLevel].Value);
                newAtributes.DamageCooldown /= (1 + _info.BlueDamageCooldownDecreasePerLevel[_skillLevel].Value);

                // Definindo tamnho e posição 
                hitbox.transform.localScale = newAtributes.Size;
                hitbox.transform.SetParent(parent.transform);
                hitbox.transform.SetLocalPositionAndRotation(_info.Prefabs[0][i].PreFabPosition, Quaternion.identity);
                hitbox.transform.SetParent(null);

                // Atributos do dano
                DamageContext newContext = new(newAtributes, statusManager);

                // Ligando a hitbox
                ContinuosDamageHitBox collider = hitbox.GetComponent<ContinuosDamageHitBox>();
                collider.Initialize(newContext);
            }
            else if (_info.Prefabs[0][i].PrefabType == TypeOfSkillPrefab.VFX) { // VFX
                InstantiateVFX(_info.Prefabs[0][i]);
            }
        }
    }
    #endregion

    #region Yellow Region
    void YellowBehaviour()
    {
        // Verificando o nível da habilidade
        _skillLevel = GraciaPassiveManager.Instance.ReturnCurrentSkillArea(GraciaTypeOfSkill.Right);

        // Drenando a barra
        GraciaPassiveManager.Instance.ChangeBarValue(-_info.YellowAmountOfAuraConsumed, GraciaAura.Yellow);

        // Gastando energia
        energyManager.LooseAllEnergy();

        // Setando ação
        _onBaseAttack = InstantiateYellowHit;

        // Inscrevendo evento
        GraciaAttackManager.OnAttackHit -= _onBaseAttack;
        GraciaAttackManager.OnAttackHit += _onBaseAttack;

        // Começando a duração
        _yellowSkillDurationRoutine ??= StartCoroutine(YellowSkillDuration());
    }
    void InstantiateYellowHit() {
        // Verificando se existe algum prefab na lista
        if (_info.Prefabs[1].Count == 0) return;

        for (int i = 0; i < _info.Prefabs[1].Count; i++) {
            if (_info.Prefabs[1][i].PrefabType == TypeOfSkillPrefab.Hitbox) { // HITBOX 
                GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(_info.Prefabs[1][i].PreFab, TypeOfSkillPrefab.Hitbox);

                // Definindo tamnho e posição 
                hitbox.transform.localScale = _info.YellowAtributes.Size;
                hitbox.transform.SetParent(parent.transform);
                hitbox.transform.SetLocalPositionAndRotation(_info.Prefabs[1][i].PreFabPosition, Quaternion.identity);
                hitbox.transform.SetParent(null);

                // Atributos do dano
                DamageContext newContext = new(_info.YellowAtributes, statusManager);

                // Ligando a hitbox
                InstantDamageHitBox collider = hitbox.GetComponent<InstantDamageHitBox>();
                collider.Initialize(newContext);
            }
            else if (_info.Prefabs[1][i].PrefabType == TypeOfSkillPrefab.VFX) { // VFX
                InstantiateVFX(_info.Prefabs[1][i]);
            }
        }
    }
    IEnumerator YellowSkillDuration() {
        float timer = 0;

        while (timer < _info.YellowDurationPerLevel[_skillLevel]) {
            timer += Time.deltaTime;
            yield return null;
        }

        GraciaAttackManager.OnAttackHit -= _onBaseAttack;

        energyManager.SetCanGainEnergy(true);

        _yellowSkillDurationRoutine = null;

        End();
    }
    #endregion

    #region Red Region
    void RedBehaviour()
    {
        // Verificando o nível da habilidade
        _skillLevel = GraciaPassiveManager.Instance.ReturnCurrentSkillArea(GraciaTypeOfSkill.Left);

        // Drenando a barra
        GraciaPassiveManager.Instance.ChangeBarValue(-_info.RedAmountOfAuraConsumed, GraciaAura.Red);

        // Instanciando Hitbox e VFX
        InstantiateRedHit();

        // Finalizando habilidade
        energyManager.LooseAllEnergy();
        energyManager.SetCanGainEnergy(true);
        _canEnd = true;
    }

    void InstantiateRedHit() {
        // Verificando se existe algum prefab na lista
        if (_info.Prefabs[2].Count == 0) return;

        for (int i = 0; i < _info.Prefabs[2].Count; i++) {
            if (_info.Prefabs[2][i].PrefabType == TypeOfSkillPrefab.Hitbox) { // HITBOX 
                GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(_info.Prefabs[2][i].PreFab, TypeOfSkillPrefab.Hitbox);

                // Definindo tamnho e posição 
                hitbox.transform.localScale = _info.RedAtributes.Size;
                hitbox.transform.SetParent(parent.transform);
                hitbox.transform.SetLocalPositionAndRotation(_info.Prefabs[2][i].PreFabPosition, Quaternion.identity);
                hitbox.transform.SetParent(null);

                // Atributos do dano
                DamageAtributes newAtributes = new(_info.RedAtributes);
                newAtributes.ExtraAtributes[ExtraDamageContextAtributes.CritRate] = _info.RedCritRatePerLevel[_skillLevel];
                DamageContext newContext = new(newAtributes, statusManager);

                // Ligando a hitbox
                InstantDamageHitBox collider = hitbox.GetComponent<InstantDamageHitBox>();
                collider.Initialize(newContext);
            }
            else if (_info.Prefabs[2][i].PrefabType == TypeOfSkillPrefab.VFX) { // VFX
                InstantiateVFX(_info.Prefabs[2][i]);
            }
        }
    }
    #endregion

    #region Green Region

    void GreenBehaviour()
    {
        // Zerando a quantidade de vezes que ganhou escudo
        _amountOfShieldsGained = 0;

        // Verificando o nível da habilidade
        _skillLevel = GraciaPassiveManager.Instance.ReturnCurrentSkillArea(GraciaTypeOfSkill.Right);

        // Drenando a barra
        GraciaPassiveManager.Instance.ChangeBarValue(-_info.GreenAmountOfAuraConsumed, GraciaAura.Green);

        // Gastando energia
        energyManager.LooseAllEnergy();

        // Inscrevendo metodo quando jogador ganha escudo
        healthManager.OnGainSheild -= IncreaseDamageWhenGainedShield;
        healthManager.OnGainSheild += IncreaseDamageWhenGainedShield;

        // Verificando se existe algum prefab na lista
        if (_info.Prefabs[3].Count == 0) return;

        for (int i = 0; i < _info.Prefabs[3].Count; i++) {
            if (_info.Prefabs[3][i].PrefabType == TypeOfSkillPrefab.Hitbox) { // HITBOX 
                GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(_info.Prefabs[3][i].PreFab, TypeOfSkillPrefab.Hitbox);

                // Definindo tamnho e posição 
                hitbox.transform.localScale = _info.GreenAtributes.Size;
                hitbox.transform.SetParent(parent.transform);
                hitbox.transform.SetLocalPositionAndRotation(_info.Prefabs[3][i].PreFabPosition, Quaternion.identity);

                // Atributos do dano
                DamageAtributes newAtributes = new(_info.GreenAtributes);
                DamageContext newContext = new(newAtributes, statusManager);

                // Ligando a hitbox
                _greenHitbox = hitbox.GetComponent<ContinuosDamageHitBox>();
                _greenHitbox.Initialize(newContext);
            }
            else if (_info.Prefabs[3][i].PrefabType == TypeOfSkillPrefab.VFX) { // VFX
                InstantiateVFX(_info.Prefabs[3][i]);
            }
        }

        _greenSkillDurationRoutine ??= StartCoroutine(GreenSkillDuration());
    }

    IEnumerator GreenSkillDuration() {
        float timer = 0;

        while (timer < _info.GreenAtributes.HitBoxDuration) {
            timer += Time.deltaTime;
            yield return null;
        }

        healthManager.OnGainSheild -= IncreaseDamageWhenGainedShield;

        energyManager.SetCanGainEnergy(true);

        _greenSkillDurationRoutine = null;

        End();
    }

    void IncreaseDamageWhenGainedShield(float shieldAmountGained) {

        _amountOfShieldsGained++;

        DamageAtributes newAtributes = new(_info.GreenAtributes);
        newAtributes.Damage += (_amountOfShieldsGained* _info.GreenAmountOfDamageIncreasePerLevel[_skillLevel]);

        _greenHitbox.ChangeAtributes(newAtributes);
    }
    #endregion

    #endregion

}
