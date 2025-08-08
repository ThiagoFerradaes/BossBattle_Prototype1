using NaughtyAttributes;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] Transform rightWeaponPosition;
    [SerializeField] Transform leftWeaponPosition;
    GameObject _currentRightHandWeapon, _currentLeftHandWeapon;
    bool _rightHandOccupied, _leftHandOccupied;

    public void OnEquipRightHand(GameObject prefab, string prefabName, Vector3 weaponPosition) {
        if (_rightHandOccupied) {
            _currentRightHandWeapon.SetActive(false);
            _currentRightHandWeapon = null;
        }

        GameObject weapon = SkillPoolingManager.Instance.ReturnHitboxFromPool(prefabName, prefab);
        weapon.transform.SetParent(rightWeaponPosition);
        weapon.transform.localPosition = weaponPosition;
        weapon.SetActive(true);
        _currentRightHandWeapon = weapon;
        _rightHandOccupied = true;
    }

    public void OnEquipLeftHand(GameObject prefab, string prefabName, Vector3 weaponPosition) {
        if (_leftHandOccupied) {
            _currentLeftHandWeapon.SetActive(false);
            _currentLeftHandWeapon = null;
        }

        GameObject weapon = SkillPoolingManager.Instance.ReturnHitboxFromPool(prefabName, prefab);
        weapon.transform.SetParent(leftWeaponPosition);
        weapon.transform.localPosition = weaponPosition;
        weapon.SetActive(true);
        _currentLeftHandWeapon = weapon;
        _leftHandOccupied = true;
    }

    public void OnDesequipRightHand() {
        if (!_rightHandOccupied) return;

        _currentRightHandWeapon.SetActive(false);
        _currentRightHandWeapon = null;
        _rightHandOccupied = false;
    }

    public void OnDesequipLeftHand() {
        if (!_leftHandOccupied) return;

        _currentLeftHandWeapon.SetActive(false);
        _currentLeftHandWeapon = null;
        _leftHandOccupied = false;
    }
}
