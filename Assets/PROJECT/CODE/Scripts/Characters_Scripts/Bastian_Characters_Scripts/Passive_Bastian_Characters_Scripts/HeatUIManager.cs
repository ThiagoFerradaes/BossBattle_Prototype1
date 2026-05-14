using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeatUIManager : MonoBehaviour {
    // Components
    [SerializeField] Image heatBar;
    [SerializeField] TextMeshProUGUI heatText;
    [SerializeField] BastianPassiveSO info;
    [SerializeField] GameObject barAnimation;
    [SerializeField] Animator anim;
    [SerializeField] float animationCooldown;
    [SerializeField] string animationStateName;

    // Actions
    Action<float, float> _updateHeatBarAction;

    Coroutine animationRoutine;

    WaitForSeconds animationWaitForSeconds;

    float _currentHeat;

    private void Start() {

        _updateHeatBarAction = UpdateHeatBar;

        BastianPassiveManager.Instance.OnHeatAreaChange += UpdateText;
        BastianPassiveManager.Instance.OnHeatGain += _updateHeatBarAction;

        UpdateText(HeatArea.CoolArea);

        barAnimation.SetActive(false);
    }

    private void OnDestroy() {
        BastianPassiveManager.Instance.OnHeatGain -= _updateHeatBarAction;
        BastianPassiveManager.Instance.OnHeatAreaChange -= UpdateText;
    }
    void UpdateHeatBar(float currentHeat, float maxHeat) {
        heatBar.fillAmount = currentHeat / maxHeat;

        barAnimation.SetActive(currentHeat >= info.HeatToHitSuperHeatArea);
    }

    void UpdateText(HeatArea newHeatArea) {

        switch (newHeatArea) {
            case HeatArea.CoolArea:
                heatText.text = info.CoolText.GetLocalizedString();
                break;
            case HeatArea.HeatArea:
                heatText.text = info.HeatText.GetLocalizedString();
                break;
            case HeatArea.SuperHeatArea:
                heatText.text = info.SuperHeatText.GetLocalizedString();
                break;
            case HeatArea.OverHeatArea:
                heatText.text = info.OverHeatText.GetLocalizedString();
                break;
            case HeatArea.ExtremeHeatArea:
                heatText.text = info.LastOverHeatText.GetLocalizedString();
                break;
        }

        animationRoutine ??= StartCoroutine(AnimationRoutine());

    }

    IEnumerator AnimationRoutine() {
        anim.CrossFade(animationStateName, 0);
        yield return animationWaitForSeconds;
        animationRoutine = null;
    }
}
