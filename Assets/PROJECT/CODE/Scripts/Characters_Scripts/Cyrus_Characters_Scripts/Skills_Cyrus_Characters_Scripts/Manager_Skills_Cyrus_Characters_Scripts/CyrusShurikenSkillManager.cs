using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyrusShurikenSkillManager : SkillObjectManager {

    CyrusShurikenSkillSO _info;
    [SerializeField] InstantDamageHitBox shurikenLevelZero;
    [SerializeField] List<InstantDamageHitBox> ListOfShurikensLevelOne;
    [SerializeField] List<InstantDamageHitBox> ListOfShurikensLevelTwo;

    int _skillLevel;

    Coroutine _rotationRoutine;

    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AnimationParameter, _info.AnimationName, 0));
    }

    void Initialize(SkillSO skill) {

        if (_info == null) _info = skill as CyrusShurikenSkillSO;

        _skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    public override void FirstFunc() {
        base.FirstFunc();

        energyManager.SetCanGainEnergy(false);
        energyManager.LooseAllEnergy();

        if (_skillLevel < 3) CyrusPassiveManager.Instance.AddUseSkill(slot, _info.AmountOfUsesPerLevel[_skillLevel]);
    }

    public override void ThirdFunc() {
        base.ThirdFunc();

        _rotationRoutine ??= StartCoroutine(Duration());
    }

    public override void FourthFunc() {
        base.FourthFunc();

        UnblockInputs();
    }

    IEnumerator Duration() {

        float duration = 360 / _info.RotationSpeed;

        transform.DORotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360).
            SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);

        float timer = 0f;

        while (timer < _info.Atributes.HitBoxDuration) {
            timer += Time.deltaTime;
            transform.position = parent.transform.position;
            yield return null;
        }

        transform.DOKill();

        energyManager.SetCanGainEnergy(true);

        _rotationRoutine = null;

        End();
    }
    public override void InstantiateHitBox(SkillAnimationEvent prefab) {
        transform.position = parent.transform.position;
        switch (_skillLevel) {
            case 0:
                StartOneShuriken();
                break;
            case 1:
                StartTwoShuriken();
                break;
            case 2:
                StartFourShuriken();
                break;
        }
    }

    void StartOneShuriken() {

        DamageContext newContext = new(_info.Atributes, statusManager);

        shurikenLevelZero.Initialize(newContext);
    }

    void StartTwoShuriken() {
        DamageContext newContext = new(_info.Atributes, statusManager);
        foreach (var shuriken in ListOfShurikensLevelOne) {
            shuriken.Initialize(newContext);
        }
    }

    void StartFourShuriken() {
        DamageContext newContext = new(_info.Atributes, statusManager);
        foreach (var shuriken in ListOfShurikensLevelTwo) {
            shuriken.Initialize(newContext);
        }
    }

    private void OnDestroy() {
        transform.DOKill();
    }
}
