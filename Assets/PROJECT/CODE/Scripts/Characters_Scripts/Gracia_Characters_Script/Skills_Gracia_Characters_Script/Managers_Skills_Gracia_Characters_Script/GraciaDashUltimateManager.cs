using System.Collections;
using UnityEngine;

public class GraciaDashUltimateManager : SkillObjectManager
{
    #region Paramethers

    // Components
    GraciaDashUltimateSO _info;
    InstantDamageHitBox _principalDashDamageHitbox;

    // Int
    int _skillLevel;

    // Coroutines
    Coroutine _dashRoutine;

    

    #endregion

    #region Initialize

    public override void UseSkill(SkillSO skill) {
        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AttackAnimationParameter, _info.AttackAnimationParameter, 0));
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as GraciaDashUltimateSO;
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    #endregion

    #region Animation Methodes Override

    public override void FirstFunc() {
        base.FirstFunc();

        energyManager.SetCanGainEnergy(false);
        energyManager.LooseAllEnergy();
    }

    public override void SecondFunc() {
        DecideBehaviour();
        _dashRoutine ??= StartCoroutine(DashRoutine());
    }

    public override void FourthFunc() {
        base.FourthFunc();

        if (_dashRoutine != null) {
            StopCoroutine(_dashRoutine);
            _dashRoutine = null;
        }

        energyManager.SetCanGainEnergy(true);

        if (_principalDashDamageHitbox != null) _principalDashDamageHitbox.ForceEnd();

        UnblockInputs();
    }

    IEnumerator DashRoutine() {
        while (true) {
            Debug.Log("Dashing");
        }
    }

    #endregion

    #region Behaviours

    void DecideBehaviour() {
        Debug.Log("Decide Behaviour");
    }

    #endregion

    #region Instantiate

    public override void InstantiateHitBox(SkillAnimationEvent prefab) {

        // Pegando a hitbox na pool
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);

        // Setando o tamanho e a posição do objeto
        hitbox.transform.localScale = _info.Atributes.Size;
        hitbox.transform.SetParent(parent.transform, false);
        hitbox.transform.SetLocalPositionAndRotation(prefab.PreFabPosition, Quaternion.identity);

        DamageContext newContext = new(_info.Atributes, statusManager);

        _principalDashDamageHitbox = hitbox.GetComponent<InstantDamageHitBox>();
        _principalDashDamageHitbox.Initialize(newContext);
    }

    #endregion

}
