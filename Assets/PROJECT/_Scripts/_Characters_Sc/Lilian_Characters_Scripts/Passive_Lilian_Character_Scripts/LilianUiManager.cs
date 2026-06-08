using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LilianUiManager : MonoBehaviour
{
    // Components
    [SerializeField] Image tributesBar;
    LilianPassiveManager _passive;

    // Actions
    Action<float, float> _onTributeUpdte;
    void Start()
    {
        InitializeTextsAndImages();
        _onTributeUpdte = UpdateTributesBar;

        _passive = LilianPassiveManager.Instance;

        _passive.OnTributesChange += _onTributeUpdte;
    }

    private void OnDestroy() {
        _passive.OnTributesChange -= _onTributeUpdte;
    }

    void InitializeTextsAndImages() {
        UpdateTributesBar(0, 1);
    }
    void UpdateTributesBar(float current, float max) {
        tributesBar.fillAmount = current / max;
    }

}
