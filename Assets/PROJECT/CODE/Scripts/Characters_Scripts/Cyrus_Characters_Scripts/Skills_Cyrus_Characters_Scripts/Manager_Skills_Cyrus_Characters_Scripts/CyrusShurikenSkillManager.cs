using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyrusShurikenSkillManager : SkillObjectManager
{

    CyrusShurikenSkillSO _info;

    float _rotationSpeed;
    int _skillLevel;

    Coroutine _rotationRoutine, _accelerateRoutine;

    List<InstantDamageHitBox> _listOfShurikens = new();

    public override void UseSkill(SkillSO skill)
    {
        base.UseSkill(skill);

        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AnimationParameter, _info.AnimationName, 0));
    }

    void Initialize(SkillSO skill)
    {

        if (_info == null) _info = skill as CyrusShurikenSkillSO;

        _skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    public override void FirstFunc()
    {
        base.FirstFunc();

        energyManager.SetCanGainEnergy(false);
        energyManager.LooseAllEnergy();

        if (_skillLevel < 3) CyrusPassiveManager.Instance.AddUseSkill(slot, _info.AmountOfUsesPerLevel[_skillLevel]);
    }

    public override void ThirdFunc()
    {
        base.ThirdFunc();

        if (_skillLevel < 3) _rotationRoutine ??= StartCoroutine(Duration());
        else
        {
            _accelerateRoutine ??= StartCoroutine(Accelerate());
            _rotationRoutine ??= StartCoroutine(DurationLevelThree());
        }
    }

    public override void FourthFunc()
    {
        base.FourthFunc();

        UnblockInputs();
    }

    IEnumerator Duration()
    {
        Debug.Log("Duration");
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

        energyManager.SetCanGainEnergy(true);
        _rotationRoutine = null;
        End();
    }

    IEnumerator DurationLevelThree()
    {
        Debug.Log("Duration3");
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

        energyManager.SetCanGainEnergy(true);
        _rotationSpeed = _info.RotationSpeed;
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
        }

        for (int i = 0; i < amountOfShurikens; i++)
        {
            GameObject shuriken = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);
            Vector2 pos = GetPosition(i, amountOfShurikens);
            Vector3 shurikenPosition = new(pos.x, prefab.PreFabPosition.y, pos.y);

            shuriken.transform.SetParent(transform, false);

            shuriken.transform.SetLocalPositionAndRotation(shurikenPosition, Quaternion.identity);

            DamageContext newContext = new(_info.Atributes, statusManager);

            InstantDamageHitBox collider = shuriken.GetComponent<InstantDamageHitBox>();
            _listOfShurikens.Add(collider);
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
