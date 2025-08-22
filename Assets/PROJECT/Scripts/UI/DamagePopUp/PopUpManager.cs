using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopUpManager : MonoBehaviour {
    public static PopUpManager Instance { get; private set; }

    private List<DamagePopUp> _damagePopUpList = new();

    [Header("Damage Label Popup")]
    [SerializeField] private DamagePopUp damageLabelPrefab;

    [Header("Display Setup")]
    [Range(0.8f, 1.5f), SerializeField] public float displayLength = 1f;
    private Camera _mainCamera;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else
            Destroy(gameObject);

        _mainCamera = Camera.main;

    }

    public void DamageDone(int damage, Vector3 position, bool isCrit) {
        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(position);
        screenPosition.z = 0;
        bool direction = screenPosition.x < Screen.width * 0.5f;

        SpawnDamagePopup(damage, screenPosition, direction, isCrit);
    }

    private void SpawnDamagePopup(int damage, Vector3 position, bool direction, bool isCrit) {

        for (int i = 0; i < _damagePopUpList.Count; i++) {
            if (!_damagePopUpList[i].gameObject.activeInHierarchy) {
                _damagePopUpList[i].Display(damage, position, direction, isCrit);
            }
        }

        DamagePopUp damageLabel = Instantiate(damageLabelPrefab);
        damageLabel.Initialize(displayLength, this);
        damageLabel.gameObject.SetActive(false);
        damageLabel.transform.SetParent(this.transform);
        _damagePopUpList.Add(damageLabel);   
        damageLabel.Display(damage, position, direction, isCrit);
    }
}
