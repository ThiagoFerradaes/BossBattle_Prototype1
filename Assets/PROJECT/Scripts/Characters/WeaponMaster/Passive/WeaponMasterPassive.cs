using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WeaponMasterPassive : PassiveSkillManager {

    #region Parameters

    public enum WeaponType { Sword, Axe, Spear, Gun }
    WeaponMasterPassiveSO _info;
    HealthManager _healthManager;
    PlayerSkillCooldownManager _playerSkillCooldownManager;
    StatusManager _statusManager;

    #endregion

    #region Methods

    public override void OnStart(PassiveSO passive, GameObject parent) {

        Initialize(passive, parent);

        AxeAttackManager.OnWeaponChange += ()=> ChangePassive(WeaponType.Axe);
        SpearAttackManager.OnWeaponChange += () => ChangePassive(WeaponType.Spear);
        ShootUpUltimateManager.OnWeaponChange += () => ChangePassive(WeaponType.Gun);

        ChangePassive(WeaponType.Sword);
    }

    void Initialize(PassiveSO passive, GameObject parent) {
        _info = passive as WeaponMasterPassiveSO;
        _healthManager = parent.GetComponent<HealthManager>();
        _playerSkillCooldownManager = parent.GetComponent<PlayerSkillCooldownManager>();
        _statusManager = parent.GetComponent<StatusManager>();  
    }
    void ChangePassive(WeaponType type) {
        switch (type) {
            case WeaponType.Sword:
                Debug.Log("Passive -> Sword");
                break;
            case WeaponType.Axe:
                Debug.Log("Passive -> Axe");
                _healthManager.RecieveShield(_info.AmountOfFirstShieldRecieved, _info.ShieldDuration);
                break;
            case WeaponType.Spear:
                Debug.Log("Passive -> Spear");
                _playerSkillCooldownManager.ResetCooldown(SkillSlot.Dash);
                break;
            case WeaponType.Gun:
                Debug.Log("Passive -> Gun");
                _statusManager.ChangeStatus(StatusType.AttackSpeed, _info.AttackSpeedBuff, true, _info.AttackSpeedBuffDuration);
                break;
        }
    }

    #endregion
}
