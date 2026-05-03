using UnityEngine;
using UnityEngine.UI;

public class DemoPopUpManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] GameObject demoPopUp;
    [SerializeField] GameObject firstPage;
    [SerializeField] GameObject secondPage;
    [SerializeField] GameObject thirdPage;
    [SerializeField] Button closePopUpButton;
    [SerializeField] Button firstArrowButton;
    [SerializeField] Button secondArrowButton;

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
        thirdPage.SetActive(false);

        demoPopUp.SetActive(true);
    }

    void SetButtons() {
        firstArrowButton.onClick.AddListener(() => {
            firstPage.SetActive(false);
            thirdPage.SetActive(false);
            secondPage.SetActive(true);
        });

        secondArrowButton.onClick.AddListener(() => {
            firstPage.SetActive(false);
            thirdPage.SetActive(true);
            secondPage.SetActive(false);
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
    public void MouseEnterArrow(Image image) {
        image.sprite = arrowSelected;
    }
    public void MouseExitArrow(Image image) {
        image.sprite = arrowUnselected;
    }
}
