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

    Action<float, GraciaTypeOfSkill> _onUpdateBarValue;
    Action<int, GraciaTypeOfSkill> _onUpdateBarAreaNumber;
    #endregion

    #region Initialize

    private void Start() {
        _onUpdateBarValue = UpdateBarValue;
        _onUpdateBarAreaNumber = UpdateBarArea;

        SubscribeEvent();
    }

    void SubscribeEvent() {
        GraciaPassiveManager.Instance.OnGraciaBarValueChanged -= _onUpdateBarValue;
        GraciaPassiveManager.Instance.OnGraciaBarValueChanged += _onUpdateBarValue;

        GraciaPassiveManager.Instance.OnGraciaBarAreaChanged -= _onUpdateBarAreaNumber;
        GraciaPassiveManager.Instance.OnGraciaBarAreaChanged += _onUpdateBarAreaNumber;
    }

    void UnsubscribeEvent() {
        GraciaPassiveManager.Instance.OnGraciaBarValueChanged -= _onUpdateBarValue;
        GraciaPassiveManager.Instance.OnGraciaBarAreaChanged -= _onUpdateBarAreaNumber;
    }
    private void OnDestroy() {
        UnsubscribeEvent();
    }
    #endregion

    #region Image Update Methods

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
                leftBarAreaNumber.text = newValue.ToString("F0");
                break;
            case GraciaTypeOfSkill.Right:
                rightBarAreaNumber.text = newValue.ToString("F0");
                break;
        }
    }
    #endregion
}
