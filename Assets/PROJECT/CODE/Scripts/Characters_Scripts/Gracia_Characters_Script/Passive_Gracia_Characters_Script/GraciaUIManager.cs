using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraciaUIManager : MonoBehaviour
{
    #region Paramethers

    [SerializeField] Image leftBarValue;
    [SerializeField] Image rightBarValue;
    [SerializeField] TextMeshProUGUI leftBarAreaNumber;
    [SerializeField] TextMeshProUGUI rightBarAreaNumber;
    [SerializeField] TextMeshProUGUI currentAuraText;
    [SerializeField] Color blueCollor;
    [SerializeField] Color yellowCollor;
    [SerializeField] Color redCollor;
    [SerializeField] Color greenCollor;

    Action<float, GraciaTypeOfSkill> _onUpdateBarValue;
    Action<int, GraciaTypeOfSkill> _onUpdateBarAreaNumber;
    Action<GraciaAura> _onChangeCurrentAura;

    #endregion

    #region Initialize

    private void Awake()
    {
        _onUpdateBarValue = UpdateBarValue;
        _onUpdateBarAreaNumber = UpdateBarArea;
        _onChangeCurrentAura = ChangeCurrentAuraIndicator;
    }

    private void Start() {      
        SubscribeEvent();
        SetBarColors();
    }

    void SubscribeEvent() {
        GraciaPassiveManager.Instance.OnGraciaBarValueChanged -= _onUpdateBarValue;
        GraciaPassiveManager.Instance.OnGraciaBarValueChanged += _onUpdateBarValue;

        GraciaPassiveManager.Instance.OnGraciaBarAreaChanged -= _onUpdateBarAreaNumber;
        GraciaPassiveManager.Instance.OnGraciaBarAreaChanged += _onUpdateBarAreaNumber;

        GraciaPassiveManager.Instance.OnCurrentAuraChanged -= _onChangeCurrentAura;
        GraciaPassiveManager.Instance.OnCurrentAuraChanged += _onChangeCurrentAura;
    }

    void UnsubscribeEvent() {
        GraciaPassiveManager.Instance.OnGraciaBarValueChanged -= _onUpdateBarValue;
        GraciaPassiveManager.Instance.OnGraciaBarAreaChanged -= _onUpdateBarAreaNumber;
        GraciaPassiveManager.Instance.OnCurrentAuraChanged -= _onChangeCurrentAura;
    }
    private void OnDestroy() {
        UnsubscribeEvent();
    }
    #endregion

    #region Image Update Methods

    void ChangeCurrentAuraIndicator(GraciaAura newAura)
    {
        currentAuraText.text = newAura.ToString();
    }
    void SetBarColors()
    {
        GraciaAura leftAura = GraciaPassiveManager.Instance.ReturnLeftAura();
        GraciaAura rightAura = GraciaPassiveManager.Instance.ReturnRighttAura();
        GraciaAura currentAura = GraciaPassiveManager.Instance.ReturnCurrentAura();

        if (leftAura == GraciaAura.Blue) leftBarValue.color = blueCollor;
        else leftBarValue.color = redCollor;

        if (rightAura == GraciaAura.Green) rightBarValue.color = greenCollor;
        else rightBarValue.color = yellowCollor;

        currentAuraText.text = currentAura.ToString();
    }
    void UpdateBarValue(float newValue, GraciaTypeOfSkill bar) {
        switch (bar) {
            case GraciaTypeOfSkill.Left:
                leftBarValue.fillAmount = newValue / 100;
                break;
            case GraciaTypeOfSkill.Right:
                rightBarValue.fillAmount = newValue / 100;
                break;
        }
    }

    void UpdateBarArea(int newValue, GraciaTypeOfSkill bar) {
        switch (bar) {
            case GraciaTypeOfSkill.Left:
                leftBarAreaNumber.text = (newValue + 1 ).ToString("F0");
                break;
            case GraciaTypeOfSkill.Right:
                rightBarAreaNumber.text = (newValue + 1).ToString("F0");
                break;
        }
    }
    #endregion
}
