using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CyrusPassiveManager : PassiveSkillManager {

    #region Parameters

    public enum WeaponType { Sword, Axe, Spear, Gun }
    CyrusPassiveSO _info;
    HealthManager _healthManager;
    PlayerSkillCooldownManager _playerSkillCooldownManager;
    StatusManager _statusManager;

    Action _onAxeChange;
    Action _onSpearChange;
    Action _onGunChange;
    #endregion

    #region Methods

    public override void OnStart(PassiveSO passive, GameObject parent) {

        Initialize(passive, parent);

        _onAxeChange = () => ChangePassive(WeaponType.Axe);
        _onSpearChange = () => ChangePassive(WeaponType.Spear);
        _onGunChange = () => ChangePassive(WeaponType.Gun);

        AxeAttackManager.OnWeaponChange -= _onAxeChange;
        SpearAttackManager.OnWeaponChange -= _onSpearChange;
        ShootUpUltimateManager.OnWeaponChange -= _onGunChange;

        AxeAttackManager.OnWeaponChange += _onAxeChange;
        SpearAttackManager.OnWeaponChange += _onSpearChange;
        ShootUpUltimateManager.OnWeaponChange += _onGunChange;

        ChangePassive(WeaponType.Sword);
    }
    private void OnDestroy() {
        AxeAttackManager.OnWeaponChange -= _onAxeChange;
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
}
