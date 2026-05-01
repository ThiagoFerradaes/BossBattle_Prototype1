using Unity.AppUI.UI;
using UnityEngine;

public class LilianTavernManager : MonoBehaviour
{
    [SerializeField] GameObject exclamationIcon;

    private void Awake() {
        ProgressWhiteBoard.Instance.OnChangedBoolValue += HandleExclamationIcon;
    }
    private void OnDestroy() {
        ProgressWhiteBoard.Instance.OnChangedBoolValue -= HandleExclamationIcon;
    }
    void Start()
    {
        if (ProgressWhiteBoard.Instance.DictionaryOfProgressBools[ProgressBools.IsKrakenDefeated]) {
            HandlePositionPostKraken();
        }
        else {
            HandlePositionPreKraken();
        }
    }

    void HandlePositionPreKraken() {
        bool talkedToLilian = ProgressWhiteBoard.Instance.DictionaryOfProgressBools[ProgressBools.HasTalkedToLilianBGFDemo];
        exclamationIcon.SetActive(!talkedToLilian);
    }

    void HandlePositionPostKraken() {
        exclamationIcon.SetActive(false);
    }
    void HandleExclamationIcon(ProgressBools type, bool value) {
        if (type != ProgressBools.HasTalkedToLilianBGFDemo) return;

        exclamationIcon.SetActive(false);
    }
}
