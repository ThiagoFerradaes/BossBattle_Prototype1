using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LilianUiManager : MonoBehaviour
{

    [SerializeField] Image judgmentTimerBar;
    [SerializeField] TextMeshProUGUI judgmentTimerText;
    [SerializeField] TextMeshProUGUI corruptionText;
    [SerializeField] TextMeshProUGUI tributesText;

    LilianPassiveManager passive;
    Action<float, float> _onJudgmentTimer;
    Action<float> _onCorruptionUpdate, _onTributeUpdte;
    Action<bool> _onJudgmentDay;
    void Start()
    {
        InitializeTextsAndImages();

        _onJudgmentTimer = UpdateJudgmentTimerBar;
        _onCorruptionUpdate = UpdateCorruptionText;
        _onTributeUpdte = UpdateTributeText;
        _onJudgmentDay = UpdateJudgmentText;

        passive = LilianPassiveManager.Instance;

        passive.OnJudgmentTimer += _onJudgmentTimer;
        passive.OnCorruptionChange += _onCorruptionUpdate;
        passive.OnTributesChange += _onTributeUpdte;
        passive.OnJudgmentDay += _onJudgmentDay;
    }

    private void OnDestroy() {
        passive.OnJudgmentTimer -= _onJudgmentTimer;
        passive.OnCorruptionChange -= _onCorruptionUpdate;
        passive.OnTributesChange -= _onTributeUpdte;
        passive.OnJudgmentDay -= _onJudgmentDay;
    }

    void InitializeTextsAndImages() {
        UpdateCorruptionText(0);
        UpdateJudgmentTimerBar(0, 1);
        UpdateJudgmentText(false);
        UpdateTributeText(0);
    }
    void UpdateJudgmentTimerBar(float current, float max) {
        judgmentTimerBar.fillAmount = current / max;
    }

    void UpdateJudgmentText(bool isJudgmentDay) {
        if (isJudgmentDay) {
            judgmentTimerText.text = "JUDGMENT DAY";
        }
        else {
            judgmentTimerText.text = "Judgment Timer";
        }
    }
    void UpdateCorruptionText(float amount) {
        corruptionText.text = amount.ToString("F0");
    }

    void UpdateTributeText(float amount) {
        tributesText.text = amount.ToString("F0");
    }
}
