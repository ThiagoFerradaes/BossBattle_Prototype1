using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CyrusRank { E, D, C, B, A, S, SS }
public class CyrusPassiveManager : PassiveSkillManager {

    #region Parameters

    public static CyrusPassiveManager Instance;

    // Components
    CyrusPassiveSO _info;

    // Atributes
    float _currentAmountOfExp;
    float _expMultiplier = 1f;
    bool _rankUP;
    CyrusRank _currentRank = CyrusRank.E;
    Dictionary<SkillSlot, int> _skillLevel = new() {
        { SkillSlot.SkillOne, 0 },
        { SkillSlot.SkillTwo, 0 },
        { SkillSlot.Ultimate, 0 },
    };

    // Actions
    public event Action OnRankLevelUp, OnSkillLevelUp;

    #endregion

    #region Methods

    #region Initialize
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);

    }
    public override void OnStart(PassiveSO passive, GameObject parent) {

        Initialize(passive, parent);

        gameObject.SetActive(true);

        UpgradeSkill();

        AditionalUIManager.Instance.InstantiateUI(_info.CyrusUI);
    }

    void Initialize(PassiveSO passive, GameObject parent) {
        _info = passive as CyrusPassiveSO;
    }

    #endregion

    #endregion

    #region ExpGain
    public void GainExp(float amountOfExp) {
        if (_rankUP) return;

        _currentAmountOfExp += amountOfExp * _expMultiplier;
        CheckRankLevelUp();
    }

    void CheckRankLevelUp() {

        float nextRankExp = _info.AmountOfExpPerClassification[_currentRank + 1];

        CyrusRank newRank = _currentAmountOfExp >= nextRankExp ? _currentRank + 1 : _currentRank;

        if (newRank != _currentRank) {
            _currentRank = newRank;
            _rankUP = true;
            OnRankLevelUp?.Invoke();
        }
    }

    /// <summary>
    /// The exp multiplier = expMultiplier * or / (1 + amountToMultiply/100) 
    /// </summary>
    /// <param name="amountToMultiply"></param>
    /// <param name="increase"></param>
    public void ChangeExpMultiplier(float amountToMultiply, bool increase, float duration) {
        float realMultiplier = 1 + amountToMultiply / 100;
        _expMultiplier = increase ? _expMultiplier * realMultiplier : _expMultiplier / realMultiplier;

        StartCoroutine(ExpMultiplierTimer(amountToMultiply, !increase, duration));
    }

    IEnumerator ExpMultiplierTimer(float amountToMultiply, bool increase, float duration) {
        yield return new WaitForSeconds(duration);

        float realMultiplier = 1 + amountToMultiply / 100;
        _expMultiplier = increase ? _expMultiplier * realMultiplier : _expMultiplier / realMultiplier;
    }

    void UpgradeSkill() {
        _info.UpgradeSkillOne.Enable();
        _info.UpgradeSkillTwo.Enable();
        _info.UpgradeUltimate.Enable();

        _info.UpgradeSkillOne.performed += ctx => {
            if (_rankUP && _skillLevel[SkillSlot.SkillOne] < 3) {
                _rankUP = false;
                _skillLevel[SkillSlot.SkillOne]++;
                OnSkillLevelUp?.Invoke();
            }
        };
        _info.UpgradeSkillTwo.performed += ctx => {
            if (_rankUP && _skillLevel[SkillSlot.SkillTwo] < 3) {
                _rankUP = false;
                _skillLevel[SkillSlot.SkillTwo]++;
                OnSkillLevelUp?.Invoke();
            }
        };
        _info.UpgradeUltimate.performed += ctx => {
            if (_rankUP && _skillLevel[SkillSlot.Ultimate] < 3) {
                _rankUP = false;
                _skillLevel[SkillSlot.Ultimate]++;
                OnSkillLevelUp?.Invoke();
            }
        };
    }

    public int ReturnSkillLevel(SkillSlot slot) => _skillLevel[slot];

    public CyrusRank ReturnCyrusRank() => _currentRank;
    #endregion

}
