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

    // Actions
    Action<float, float> _updateHeatBarAction;

    private void Start() {

        _updateHeatBarAction = UpdateHeatBar;

        BastianPassiveManager.Instance.OnHeatGain -= _updateHeatBarAction;
        BastianPassiveManager.Instance.OnHeatGain += _updateHeatBarAction;

        ChangeColors(0);
    }

    private void OnDestroy() {
        BastianPassiveManager.Instance.OnHeatGain -= _updateHeatBarAction;
    }
    void UpdateHeatBar(float currentHeat, float maxHeat) {
        heatBar.fillAmount = currentHeat / maxHeat;

        ChangeColors(currentHeat);
    }

    void ChangeColors(float currentHeat) {
        if (currentHeat >= info.HeatToHitLastOverHeatArea) {
            heatBar.color = info.LastOverHeatColor;
            heatText.text = info.LastOverHeatText;
            heatText.color = info.LastOverHeatColor;
        }
        else if (currentHeat >= info.HeatToHitOverHeatArea) {
            heatBar.color = info.OverHeatColor;
            heatText.text = info.OverHeatText;
            heatText.color = info.OverHeatColor;
        }
        else if (currentHeat >= info.HeatToHitSuperHeatArea) {
            heatBar.color = info.SuperHeatColor;
            heatText.text = info.SuperHeatText;
            heatText.color = info.SuperHeatColor;
        }
        else if (currentHeat >= info.HeatToHitHeatArea) {
            heatBar.color = info.HeatColor;
            heatText.text = info.HeatText;
            heatText.color = info.HeatColor;
        }
        else {
            heatBar.color = info.CoolColor;
            heatText.text = info.CoolText;
            heatText.color = info.CoolColor;
        }
    }
}
