using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;


public class InputDeviceManager : MonoBehaviour {
    [SerializeField] GameObject firstButtonSelected;

    public static InputDeviceManager Instance;

    GameObject _currentSelectedButton;

    InputType currentInputType;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(this);
    }
    void Start() {
        InputSystem.onEvent += HandleInput;
        ChangeCurrentSelectedButton(firstButtonSelected);

        SceneManager.activeSceneChanged += (scene, sceneTwo) => ResetCurrentSelectedButton();
    }

    private void OnDestroy() {
        InputSystem.onEvent -= HandleInput;
        SceneManager.activeSceneChanged -= (scene, sceneTwo) => ResetCurrentSelectedButton();
    }
    void HandleInput(InputEventPtr eventPtr, InputDevice device) {
        if (device is Gamepad) {
            SetDeviceAsGamepad();
        }
        else if (device is Mouse || device is Keyboard) {
            SetDeviceAsMouseKeyboard();
        }
    }

    void SetDeviceAsGamepad() {
        if (currentInputType == InputType.Gamepad) return;

        currentInputType = InputType.Gamepad;
        Cursor.visible = false;
        EventSystem.current.SetSelectedGameObject(null);
        if (_currentSelectedButton != null) EventSystem.current.SetSelectedGameObject(_currentSelectedButton);
    }

    void SetDeviceAsMouseKeyboard() {
        if (currentInputType == InputType.MouseKeyboard) return;

        currentInputType = InputType.MouseKeyboard;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ChangeCurrentSelectedButton(GameObject newButton) {
        _currentSelectedButton = newButton;
    }

    void ResetCurrentSelectedButton() {
        _currentSelectedButton = null;
    }
}
