using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyrusShurikenSkillManager : SkillObjectManager
{

    CyrusShurikenSkillSO _info;

    float _rotationSpeed;
    int _skillLevel;
    bool isFirstTimeLevelThree = true;

    Coroutine _rotationRoutine, _accelerateRoutine;

    public List<InstantDamageHitBox> _listOfShurikens = new();

    public override void UseSkill(SkillSO skill)
    {
        base.UseSkill(skill);

        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine());
    }

    void Initialize(SkillSO skill)
    {

        if (_info == null) _info = skill as CyrusShurikenSkillSO;

        _skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    protected override void FirstFunc()
    {
        base.FirstFunc();

        energyManager.SetCanGainEnergy(false);
        energyManager.LooseAllEnergy();
    }

    protected override void ThirdFunc()
    {
        base.ThirdFunc();

        if (_skillLevel < 3) _rotationRoutine ??= StartCoroutine(Duration());
        else
        {
            _accelerateRoutine ??= StartCoroutine(Accelerate());
            _rotationRoutine ??= StartCoroutine(DurationLevelThree());
        }
    }

    protected override void FourthFunc()
    {
        base.FourthFunc();

        if (_skillLevel < 3) CyrusPassiveManager.Instance.AddUseSkill(slot, _info.AmountOfUsesPerLevel[_skillLevel], _info.ListOfSprites);

        UnblockInputs();
    }

    IEnumerator Duration()
    {
        float timer = 0f;
        float currentAngle = 0f;

        transform.localRotation = Quaternion.identity;

        _rotationSpeed = _skillLevel < 2 ? _info.RotationSpeed : _info.RotationSpeedLevelTwo;

        while (timer < _info.Atributes.HitBoxDuration)
        {
            timer += Time.deltaTime;

            float speed = _rotationSpeed;

            currentAngle += speed * Time.deltaTime;
            if (currentAngle > 360f) currentAngle -= 360f;

            transform.SetPositionAndRotation(parent.transform.position, Quaternion.Euler(0f, currentAngle, 0f));

            yield return null;
        }

        foreach (var shuriken in _listOfShurikens) shuriken.ForceEnd();

        _listOfShurikens.Clear();

        energyManager.SetCanGainEnergy(true);
        _rotationRoutine = null;
        End();
    }

    IEnumerator DurationLevelThree()
    {
        float currentAngle = 0f;

        while (true)
        {
            float speed = _rotationSpeed;

            currentAngle += speed * Time.deltaTime;
            if (currentAngle > 360f) currentAngle -= 360f;

            transform.SetPositionAndRotation(parent.transform.position, Quaternion.Euler(0f, currentAngle, 0f));

            yield return null;
        }
    }

    IEnumerator Accelerate()
    {
        _rotationSpeed = _info.RotationSpeedLevelThree;

        yield return new WaitForSeconds(_info.Atributes.HitBoxDuration);

        for (int i = 0; i < _listOfShurikens.Count; i++)
        {
            if ((i + 1) % 2 == 0)
            {
                _listOfShurikens[i].ForceEnd();
                _listOfShurikens[i] = null;
            }
        }

        isFirstTimeLevelThree = false;
        energyManager.SetCanGainEnergy(true);
        _rotationSpeed = _info.RotationSpeed;
        _accelerateRoutine = null;
    }
    public override void InstantiateHitBox(SkillAnimationEvent prefab)
    {
        transform.position = parent.transform.position;

        int amountOfShurikens = 0;
        switch (_skillLevel)
        {
            case 0:
                amountOfShurikens = _info.AmountOfShurikensLevelZero;
                break;
            case 1:
                amountOfShurikens = _info.AmountOfShurikensLevelOne;
                break;
            case 2:
                amountOfShurikens = _info.AmountOfShurikensLevelTwo;
                break;
            case 3:
                amountOfShurikens = _info.AmountOfShurikensLevelTwo;
                break;
        }

        InstantiateShuriken(prefab, amountOfShurikens);
    }

    void InstantiateShuriken(SkillAnimationEvent prefab, int amountOfShurikens)
    {

        for (int i = 0; i < amountOfShurikens; i++)
        {
            if (_skillLevel >= 3 && !isFirstTimeLevelThree)
            {
                if (i < _listOfShurikens.Count && _listOfShurikens[i] != null) continue;
            }

            GameObject shuriken = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);
            Vector2 pos = GetPosition(i, amountOfShurikens);
            Vector3 shurikenPosition = new(pos.x, prefab.PreFabPosition.y, pos.y);

            shuriken.transform.SetParent(transform, false);

            shuriken.transform.SetLocalPositionAndRotation(shurikenPosition, Quaternion.identity);

            DamageAtributes newAtributes = new(_info.Atributes);
            if (_skillLevel > 0) newAtributes.ExtraAtributes[ExtraDamageContextAtributes.Penetration] = _info.PenetrationLevelOne;
            DamageContext newContext = new(newAtributes, statusManager);

            InstantDamageHitBox collider = shuriken.GetComponent<InstantDamageHitBox>();

            if (i >= _listOfShurikens.Count) _listOfShurikens.Add(collider);
            else _listOfShurikens[i] = collider;

            collider.Initialize(newContext, false);
        }
    }

    Vector2 GetPosition(int index, int maxAmount)
    {
        float angle = _info.InitialAngle + (360 / maxAmount) * index;

        float angleInRad = Mathf.Deg2Rad * angle;

        float x = _info.Radius * Mathf.Cos(angleInRad);
        float z = _info.Radius * Mathf.Sin(angleInRad);

        return new Vector2(x, z);
    }
    private void OnDestroy()
    {
        _rotationRoutine = null;
    }
}
