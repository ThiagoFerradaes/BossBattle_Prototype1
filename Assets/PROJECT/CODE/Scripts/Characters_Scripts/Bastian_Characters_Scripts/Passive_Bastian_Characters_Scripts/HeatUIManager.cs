using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeatUIManager : MonoBehaviour
{
    // Components
    [SerializeField] Image heatBar;
    [SerializeField] TextMeshProUGUI heatText;
    [SerializeField] BastianPassiveSO info;
    [SerializeField] GameObject barAnimation;

    // Actions
    Action<float, float> _updateHeatBarAction;

    private void Start() {

        _updateHeatBarAction = UpdateHeatBar;

        BastianPassiveManager.Instance.OnHeatGain -= _updateHeatBarAction;
        BastianPassiveManager.Instance.OnHeatGain += _updateHeatBarAction;

        ChangeColors(0);

        barAnimation.SetActive(false);
    }

    private void OnDestroy() {
        BastianPassiveManager.Instance.OnHeatGain -= _updateHeatBarAction;
    }
    void UpdateHeatBar(float currentHeat, float maxHeat) {
        heatBar.fillAmount = currentHeat / maxHeat;

        ChangeColors(currentHeat);

        barAnimation.SetActive(currentHeat >= info.HeatToHitSuperHeatArea);
    }

    void ChangeColors(float currentHeat) {
        if (currentHeat >= info.HeatToHitLastOverHeatArea) {
            heatText.text = info.LastOverHeatText.GetLocalizedString();
        }
        else if (currentHeat >= info.HeatToHitOverHeatArea) {
            heatText.text = info.OverHeatText.GetLocalizedString();
        }
        else if (currentHeat >= info.HeatToHitSuperHeatArea) {
            heatText.text = info.SuperHeatText.GetLocalizedString();
        }
        else if (currentHeat >= info.HeatToHitHeatArea) {
            heatText.text = info.HeatText.GetLocalizedString();
        }
        else {
            heatText.text = info.CoolText.GetLocalizedString();
        }
    }
}
