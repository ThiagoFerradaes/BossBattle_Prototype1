using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeatUIManager : MonoBehaviour
{
    [SerializeField] Image heatBar;
    [SerializeField] TextMeshProUGUI heatText;

    [SerializeField] BastianPassiveSO info;

    Action<float, float> _updateHeatBarAction;


    private void Start() {

        _updateHeatBarAction = (currentHeat, maxHeat) => UpdateHeatBar(currentHeat, maxHeat);

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
            heatText.color = info.LastOverHeatColor;
        }
        else if (currentHeat >= info.HeatToHitOverHeatArea) {
            heatBar.color = info.OverHeatColor;
            heatText.color = info.OverHeatColor;
        }
        else if (currentHeat >= info.HeatToHitSuperHeatArea) {
            heatBar.color = info.SuperHeatColor;
            heatText.color = info.SuperHeatColor;
        }
        else if (currentHeat >= info.HeatToHitHeatArea) {
            heatBar.color = info.HeatColor;
            heatText.color = info.HeatColor;
        }
        else {
            heatBar.color = info.CoolColor;
            heatText.color = info.CoolColor;
        }
    }
}
