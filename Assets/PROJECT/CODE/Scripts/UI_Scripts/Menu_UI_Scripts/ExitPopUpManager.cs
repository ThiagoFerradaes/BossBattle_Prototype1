using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ExitPopUpManager : MonoBehaviour {
    [Header("Components")]
    [SerializeField] GameObject exitPopUp;
    [SerializeField] GameObject selectedButtonBackground;

    [Header("Buttons")]
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;
    [SerializeField] Button maskButton;

    [Header("Atributes")]
    [SerializeField] InputActionReference cancelAction;

    [Header("Events")]
    [SerializeField] UnityEvent onInitializeEvent;

    GameObject _oldSelectedButton = null;

    void Awake() {
        yesButton.onClick.AddListener(ExitGame);
        noButton.onClick.AddListener(() => HandleExitPopUp(false));
        maskButton.onClick.AddListener(() => HandleExitPopUp(false));
        cancelAction.action.performed += CancelButton;

        HandleExitPopUp(false, true);
    }
    private void OnDestroy() {
        cancelAction.action.performed -= CancelButton;
    }
    public void HandleExitPopUp(bool isOn, bool firstCall = false) {

        if (!firstCall && exitPopUp.activeInHierarchy) {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_oldSelectedButton);
        }

        if (isOn) {
            _oldSelectedButton = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
            HandleSelectedButtonBackground(yesButton.gameObject);
            onInitializeEvent?.Invoke();
        }

        exitPopUp.SetActive(isOn);
    }
    public void HandleSelectedButtonBackground(GameObject button) {

        selectedButtonBackground.transform.position = button.transform.position;
        selectedButtonBackground.SetActive(true);
    }

    public void HandleSelectedButtonBackgroundOff() {
        selectedButtonBackground.SetActive(false);
    }

    public void CancelButton(InputAction.CallbackContext context) {
        if (context.performed) {
            HandleExitPopUp(false);
        }
    }

    void ExitGame() {
        Application.Quit();
    }
}
