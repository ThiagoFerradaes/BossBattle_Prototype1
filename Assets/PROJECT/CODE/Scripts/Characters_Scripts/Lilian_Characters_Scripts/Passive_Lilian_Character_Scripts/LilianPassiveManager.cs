using System;
using System.Collections;
using UnityEngine;

public class LilianPassiveManager : PassiveSkillManager {
    public static LilianPassiveManager Instance;

    // Components
    LilianPassiveSO _info;
    StunManager _stunManager;
    HealthManager _healthManager;

    // Atributes
    float _judgmentTimerMultiplier = 1f;
    float _judgmentCostMultiplier = 1f;
    float _currentAmountOfTributes = 0f;
    int _currentCorruption = 0;

    // Coroutines
    Coroutine _judgmentTimerCoroutine;
    Coroutine _judgmentCoroutine;
    Coroutine _blessingCoroutine;
    Coroutine _wrathCoroutine;

    // Events
    public event Action<float, float> OnJudgmentTimer;
    public event Action<float> OnCorruptionChange;
    public event Action<float> OnTributesChange;
    public event Action<bool> OnJudgmentDay;

    #region Initialize
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    public override void OnStart(PassiveSO skill, GameObject parent) {
        base.OnStart(skill, parent);

        _info = skill as LilianPassiveSO;
        _stunManager = parent.GetComponent<StunManager>();
        _healthManager = parent.GetComponent<HealthManager>();

        gameObject.SetActive(true);

        AditionalUIManager.Instance.InstantiateUI(_info.LilianUI);

        _judgmentTimerCoroutine ??= StartCoroutine(JugmentTimerRoutine());

        PlayerSkillUI.Instance.TurnBaseAttackImageOn();
    }

    #endregion

    #region Judgment
    IEnumerator JugmentTimerRoutine() {
        float timer = 0f;
        OnJudgmentDay?.Invoke(false);
        while (timer < _info.TimeToJudgment) {
            timer += Time.deltaTime * _judgmentTimerMultiplier;
            OnJudgmentTimer?.Invoke(timer, _info.TimeToJudgment);
            yield return null;
        }

        _judgmentTimerCoroutine = null;
        _judgmentCoroutine ??= StartCoroutine(Judgment());
    }

    IEnumerator Judgment() {
        OnJudgmentDay?.Invoke(true);

        if (_currentAmountOfTributes >= _info.BlessingCost * _judgmentCostMultiplier) {
            ChangeTributeAmount(-_info.BlessingCost);
            _blessingCoroutine ??= StartCoroutine(Blessing());
            yield return _blessingCoroutine;
        }
        else {
            _wrathCoroutine ??= StartCoroutine(Wrath());
            yield return _wrathCoroutine;
        }

        _judgmentCoroutine = null;
        _judgmentTimerCoroutine ??= StartCoroutine(JugmentTimerRoutine());
    }

    IEnumerator Judgment(bool blessing) {
        OnJudgmentDay?.Invoke(true);

        if (blessing) {
            _blessingCoroutine ??= StartCoroutine(Blessing());
            yield return _blessingCoroutine;
        }
        else {
            _wrathCoroutine ??= StartCoroutine(Wrath());
            yield return _wrathCoroutine;
        }

        _judgmentCoroutine = null;
        _judgmentTimerCoroutine ??= StartCoroutine(JugmentTimerRoutine());
    }

    IEnumerator Blessing() {
        Vector3 pos = ArenaManager.Instance.GetRandomPosition(_info.BlessingSize);
        pos.y = ArenaManager.Instance.FindGroundHeight(pos);
        GameObject blessingArea = PoolingManager.Instance.ReturnPrefabFromPool(_info.BlessingObject, TypeOfSkillPrefab.Hitbox);
        blessingArea.transform.localScale = new(_info.BlessingSize, _info.BlessingSize, _info.BlessingSize);
        blessingArea.transform.position = pos;
        blessingArea.SetActive(true);

        if (blessingArea.TryGetComponent<HealingAreaHitBox>(out HealingAreaHitBox healingArea)) {
            healingArea.Initialize(_info.BlessingHealing, _info.BlessingDuration, _info.BlessingHealingCooldown, _info.ListOfTags);
        }

        yield return new WaitForSeconds(_info.BlessingDuration);

        _blessingCoroutine = null;
    }

    IEnumerator Wrath() {
        _stunManager.StunCharacterWithoutAnimation(true);
        _healthManager.TakeDamage(_info.WrathDamage, false);
        ChangeCorruptionAmount(1);

        yield return new WaitForSeconds(_info.WrathStunDuration);

        _stunManager.StunCharacterWithoutAnimation(false);

        _wrathCoroutine = null;
    }

    public void ForceJudgment(bool isBlessing) {
        if (_judgmentTimerCoroutine == null) return;

        StopCoroutine(_judgmentTimerCoroutine);
        _judgmentTimerCoroutine = null;

        OnJudgmentTimer?.Invoke(1, 1);

        _judgmentCoroutine ??= StartCoroutine(Judgment(isBlessing));

    }

    #endregion

    #region Change Values

    public void ChangeCorruptionAmount(int amount) {
        _currentCorruption = Mathf.Min(_currentCorruption + amount, _info.MaxAmountOfCorruption);
        OnCorruptionChange?.Invoke(_currentCorruption);
    }

    public void ChangeTributeAmount(float amount) {
        _currentAmountOfTributes = Mathf.Min(_currentAmountOfTributes + amount, _info.MaxAmountOfTributes);
        OnTributesChange?.Invoke(_currentAmountOfTributes);
    }

    /// <summary>
    /// Percent to multiply in 0-100 scale
    /// </summary>
    /// <param name="percentToMultiply"></param>
    /// <param name="increase"></param>
    public void ChangeJudgmentTimerMultiplier(float percentToMultiply, bool increase) {
        float factor = 1 + (percentToMultiply / 100);
        _judgmentTimerMultiplier = increase ? _judgmentTimerMultiplier * factor : _judgmentTimerMultiplier / factor;
    }

    /// <summary>
    /// Percent to multiply in 0-100 scale
    /// </summary>
    /// <param name="percentToMultiply"></param>
    /// <param name="increase"></param>
    public void ChangeJudgmentCostMultiplier(float percentToMultiply, bool increase) {
        float factor = 1 + (percentToMultiply / 100);
        _judgmentCostMultiplier = increase ? _judgmentCostMultiplier * factor : _judgmentCostMultiplier / factor;
    }

    #endregion

    public float ReturnAmountOfTributes() => _currentAmountOfTributes;

    public int ReturnAmountOfCorruption() => _currentCorruption;
}
