using UnityEngine;
using UnityEngine.UI;

public class PopUpPostKraken : MonoBehaviour {

    [SerializeField] Button closeButton;
    [SerializeField] GameObject popUpScreen;
    [SerializeField] Sprite closeButtonHoover;
    [SerializeField] Sprite closeButtonUnHoover;

    void Start() {

        bool kraken = ProgressWhiteBoard.Instance.DictionaryOfProgressBools[ProgressBools.IsKrakenDefeated];
        bool hasSeen = ProgressWhiteBoard.Instance.HasSeenPostKrakenPopUp;

        if (kraken && !hasSeen) {
            TurnScreenOn();
            SetButton();
        }
        else {
            {
                TurnScreenOff();
            }
        }
    }

    void SetButton() {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(CloseButton);
    }

    void TurnScreenOn() {
        popUpScreen.SetActive(true);
        Time.timeScale = 0;
    }
    void TurnScreenOff() {
        popUpScreen.SetActive(false);
        Time.timeScale = 1;
    }

    void CloseButton() {
        ProgressWhiteBoard.Instance.HasSeenPostKrakenPopUp = true;
        TurnScreenOff();
    }

    public void OnMouseCloseButton(bool enter) {
        closeButton.image.sprite = enter ? closeButtonHoover : closeButtonUnHoover;
    }

}

