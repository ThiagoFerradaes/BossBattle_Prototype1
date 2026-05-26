using UnityEngine;
using UnityEngine.UI;

public class KrakenPopUp : MonoBehaviour
{
    [SerializeField] GameObject screen;
    [SerializeField] Button closeButton;
    [SerializeField] Sprite closeButtonSelected;
    [SerializeField] Sprite closeButtonUnSelected;
    [SerializeField] AK.Wwise.Event combatMusic;

    void Start()
    {
        if (ProgressWhiteBoard.Instance.HasSeenKrakenPopUp) TurnScreenOff();
        else TurnScreenOn();
    }

    void TurnScreenOn() {
        screen.SetActive(true);

        combatMusic.Post(gameObject);

        Time.timeScale = 0;

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(CloseButton);
    }

    void TurnScreenOff() {
        screen.SetActive(false);
    }

    void CloseButton() {
        Time.timeScale = 1;
        screen.SetActive(false);
        ProgressWhiteBoard.Instance.HasSeenKrakenPopUp = true;
    }

    public void HooverCloseButton(bool isHoover) {
        closeButton.image.sprite = isHoover ? closeButtonSelected : closeButtonUnSelected;
    }
}
