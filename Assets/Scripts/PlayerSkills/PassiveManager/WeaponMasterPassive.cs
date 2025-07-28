using UnityEngine;

public class WeaponMasterPassive : PassiveSkillManager {
    enum WeaponType { Sword, Axe, Spear, Gun }
    WeaponMasterPassiveSO _info;

    public override void OnStart(PassiveSO skill) {
        _info = skill as WeaponMasterPassiveSO;

        AxeAttackManager.OnWeaponChange += ()=> ChangePassive(WeaponType.Axe);
        SpearAttackManager.OnWeaponChange += () => ChangePassive(WeaponType.Spear);
        ShootUpUltimateManager.OnWeaponChange += () => ChangePassive(WeaponType.Gun);

        ChangePassive(WeaponType.Sword);
    }

    void ChangePassive(WeaponType type) {
        switch (type) {
            case WeaponType.Sword:
                Debug.Log("Passive -> Sword");
                break;
            case WeaponType.Axe:
                Debug.Log("Passive -> Axe");
                break;
            case WeaponType.Spear:
                Debug.Log("Passive -> Spear");
                break;
            case WeaponType.Gun:
                Debug.Log("Passive -> Gun");
                break;
        }
    }
}
