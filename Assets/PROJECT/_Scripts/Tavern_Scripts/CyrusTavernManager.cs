using UnityEngine;

public class CyrusTavernManager : MonoBehaviour
{
    [SerializeField] GameObject exclamationIcon;

    private void Awake() {
        ProgressWhiteBoard.Instance.OnChangedBoolValue += HandleExclamationIcon;
    }

    private void OnDestroy() {
        ProgressWhiteBoard.Instance.OnChangedBoolValue -= HandleExclamationIcon;
    }

    private void Start() {
        if (ProgressWhiteBoard.Instance.DictionaryOfProgressBools[ProgressBools.IsKrakenDefeated]) {
            HandlePositionPostKraken();
        }
        else {
            HandlePositionPreKraken();
        }
    }

    void HandlePositionPreKraken() {
        gameObject.SetActive(false);
    }

    void HandlePositionPostKraken() {
        gameObject.SetActive(true);

        exclamationIcon.SetActive(!ProgressWhiteBoard.Instance.DictionaryOfProgressBools[ProgressBools.TalkedToCyrus]);
    }

    void HandleExclamationIcon(ProgressBools type, bool newValue) {
        if (type != ProgressBools.TalkedToCyrus) return;

        exclamationIcon.SetActive(false);
    }
}
