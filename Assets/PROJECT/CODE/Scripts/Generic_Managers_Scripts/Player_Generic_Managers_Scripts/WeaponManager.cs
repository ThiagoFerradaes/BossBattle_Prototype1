using NaughtyAttributes;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] Transform rightWeaponPosition;
    [SerializeField] Transform leftWeaponPosition;
    GameObject _currentRightHandWeapon, _currentLeftHandWeapon;
    bool _rightHandOccupied, _leftHandOccupied;

    public void OnEquipRightHand(GameObject prefab, string prefabName, Vector3 weaponPosition, Vector3 weaponRotation) {
        if (_rightHandOccupied) {
            _currentRightHandWeapon.SetActive(false);
            _currentRightHandWeapon = null;
        }

        GameObject weapon = PoolingManager.Instance.ReturnPrefabFromPool(prefab, TypeOfSkillPrefab.VFX);
        weapon.transform.SetParent(rightWeaponPosition);
        weapon.transform.SetLocalPositionAndRotation(weaponPosition, Quaternion.Euler(weaponRotation));
        weapon.SetActive(true);
        _currentRightHandWeapon = weapon;
        _rightHandOccupied = true;
    }

    public void OnEquipLeftHand(GameObject prefab, string prefabName, Vector3 weaponPosition, Vector3 weaponRotation) {
        if (_leftHandOccupied) {
            _currentLeftHandWeapon.SetActive(false);
            _currentLeftHandWeapon = null;
        }

        GameObject weapon = PoolingManager.Instance.ReturnPrefabFromPool(prefab, TypeOfSkillPrefab.VFX);
        weapon.transform.SetParent(leftWeaponPosition);
        weapon.transform.SetLocalPositionAndRotation(weaponPosition, Quaternion.Euler(weaponRotation));
        weapon.SetActive(true);
        _currentLeftHandWeapon = weapon;
        _leftHandOccupied = true;
    }

    public void OnDesequipRightHand() {
        if (!_rightHandOccupied) return;

        PoolingManager.Instance.ReturnObjectToPool(_currentRightHandWeapon.gameObject, TypeOfSkillPrefab.VFX);
        _currentRightHandWeapon = null;
        _rightHandOccupied = false;
    }

    public void OnDesequipLeftHand() {
        if (!_leftHandOccupied) return;

        PoolingManager.Instance.ReturnObjectToPool(_currentLeftHandWeapon.gameObject, TypeOfSkillPrefab.VFX);
        _currentLeftHandWeapon = null;
        _leftHandOccupied = false;
    }
}
