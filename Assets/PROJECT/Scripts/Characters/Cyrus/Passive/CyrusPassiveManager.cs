using System;
using System.Collections.Generic;
using UnityEngine;

public enum CyrusClassification { E, D, C, B, A, S, SS }
public class CyrusPassiveManager : PassiveSkillManager {

    #region Parameters

    public static CyrusPassiveManager Instance;
    public enum WeaponType { Sword, Axe, Spear, Gun }
    CyrusPassiveSO _info;
    HealthManager _healthManager;
    PlayerSkillCooldownManager _playerSkillCooldownManager;
    StatusManager _statusManager;

    Action _onSpearChange;
    Action _onGunChange;

    // Nova passiva
    Dictionary<SkillSlot, int> _skillLevel = new() {
        { SkillSlot.SkillOne, 0 },
        { SkillSlot.SkillTwo, 0 },
        { SkillSlot.Ultimate, 0 },
    };
    CyrusClassification _currentRank = CyrusClassification.E;
    float _currentAmountOfExp;
    float _expMultiplier = 1f;
    bool _rankUP;

    public event Action OnRankLevelUp;

    #endregion

    #region Methods

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);

    }
    public override void OnStart(PassiveSO passive, GameObject parent) {

        Initialize(passive, parent);

        _onSpearChange = () => ChangePassive(WeaponType.Spear);
        _onGunChange = () => ChangePassive(WeaponType.Gun);

        SpearAttackManager.OnWeaponChange -= _onSpearChange;
        ShootUpUltimateManager.OnWeaponChange -= _onGunChange;

        SpearAttackManager.OnWeaponChange += _onSpearChange;
        ShootUpUltimateManager.OnWeaponChange += _onGunChange;

        ChangePassive(WeaponType.Sword);

        gameObject.SetActive(true);

        UpgradeSkill();
    }
    private void OnDestroy() {

        SpearAttackManager.OnWeaponChange -= _onSpearChange;
        ShootUpUltimateManager.OnWeaponChange -= _onGunChange;
    }
    void Initialize(PassiveSO passive, GameObject parent) {
        _info = passive as CyrusPassiveSO;
        _healthManager = parent.GetComponent<HealthManager>();
        _playerSkillCooldownManager = parent.GetComponent<PlayerSkillCooldownManager>();
        _statusManager = parent.GetComponent<StatusManager>();
    }
    void ChangePassive(WeaponType type) {
        switch (type) {
            case WeaponType.Sword:
                break;
            case WeaponType.Axe:
                _healthManager.RecieveShield(_info.AmountOfFirstShieldRecieved, _info.ShieldDuration);
                break;
            case WeaponType.Spear:
                _playerSkillCooldownManager.ResetCooldown(SkillSlot.Dash);
                break;
            case WeaponType.Gun:
                _statusManager.ChangeStatus(StatusType.AttackSpeed, _info.AttackSpeedBuff, true, _info.AttackSpeedBuffDuration);
                break;
        }
    }

    #endregion

    #region ExpGain
    public void GainExp(float amountOfExp) {
        if (_rankUP) return;

        _currentAmountOfExp += amountOfExp * _expMultiplier;
        CheckRankLevelUp();
    }

    void CheckRankLevelUp() {

        float nextRankExp = _info.AmountOfExpPerClassification[_currentRank + 1];

        CyrusClassification newRank = _currentAmountOfExp >= nextRankExp ? _currentRank + 1 : _currentRank;

        if (newRank != _currentRank) {
            _currentRank = newRank;
            _rankUP = true;
        }
    }

    /// <summary>
    /// The exp multiplier = expMultiplier * or / (1 + amountToMultiply/100) 
    /// </summary>
    /// <param name="amountToMultiply"></param>
    /// <param name="increase"></param>
    public void ChangeExpMultiplier(float amountToMultiply, bool increase) {
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
            }
        };
        _info.UpgradeSkillTwo.performed += ctx => {
            if (_rankUP && _skillLevel[SkillSlot.SkillTwo] < 3) {
                _rankUP = false;
                _skillLevel[SkillSlot.SkillTwo]++;
            }
        };
        _info.UpgradeUltimate.performed += ctx => {
            if (_rankUP && _skillLevel[SkillSlot.Ultimate] < 3) {
                _rankUP = false;
                _skillLevel[SkillSlot.Ultimate]++;
            }
        };
    }

    public int ReturnSkillLeve(SkillSlot slot) {
        return _skillLevel[slot];
    }
    #endregion

}
