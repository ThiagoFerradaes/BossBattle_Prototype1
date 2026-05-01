using UnityEngine;
using UnityEngine.UI;

public class DemoPopUpManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] GameObject demoPopUp;
    [SerializeField] GameObject firstPage;
    [SerializeField] GameObject secondPage;
    [SerializeField] Button closePopUpButton;
    [SerializeField] Button arrowButton;

    [Header("Sprites")]
    [SerializeField] Sprite closeSelected;
    [SerializeField] Sprite closeUnselected;
    [SerializeField] Sprite arrowSelected;
    [SerializeField] Sprite arrowUnselected;

    private void Start() {
        if (!ProgressWhiteBoard.Instance.HasSeenDemoPopUp) {
            TurnPopUpDemoOn();
            SetButtons();
        }
        else {
            demoPopUp.SetActive(false);
        }
    }

    void TurnPopUpDemoOn() {
        firstPage.SetActive(true);
        secondPage.SetActive(false);

        demoPopUp.SetActive(true);
    }

    void SetButtons() {
        arrowButton.onClick.AddListener(() => {
            firstPage.SetActive(false);
            secondPage.SetActive(true);
        });

        closePopUpButton.onClick.AddListener(() => {
            demoPopUp.SetActive(false);
            ProgressWhiteBoard.Instance.HasSeenDemoPopUp = true;
        });
    }

    public void MouseEnterCloseButton() {
        closePopUpButton.image.sprite = closeSelected;
    }
    public void MouseExitCloseButton() {
        closePopUpButton.image.sprite = closeUnselected;
    }
    public void MouseEnterArrow() {
        arrowButton.image.sprite = arrowSelected;
    }
    public void MouseExitArrow() {
        arrowButton.image.sprite = arrowUnselected;
    }
}
