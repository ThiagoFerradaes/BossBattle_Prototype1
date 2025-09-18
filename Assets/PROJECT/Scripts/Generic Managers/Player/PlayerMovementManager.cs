using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum RotationType { MouseRotation, MoveRotation }
public class PlayerMovementManager : MonoBehaviour {
    #region Parameters

    // Inputs
    [Header("Input Action Reference")]
    [SerializeField] private InputActionReference moveActionReference;

    // Movement floats
    float _zInput;
    float _xInput;

    // Booleans
    bool _canMove = true;
    bool _canWalk = true;
    bool _canRotate = true;
    bool _isDashing = false;
    bool _isPaused = false;

    // Animation
    [Header("Animation")]
    [SerializeField] string walkingAnimationParameter;

    // Components
    Animator _anim;
    Rigidbody _rb;
    StatusManager _statusManager;
    Transform _cameraCenter;
    StunManager _stunManager;

    // Atributes
    [Header("Atributes")]
    [SerializeField] float rotationSpeed;

    // LayerMask
    [Header("Layer")]
    [SerializeField] LayerMask floorLayer;

    // Rotation
    Vector2 _mousePosition;
    RotationType _rotationType = RotationType.MoveRotation;

    // Actions
    Action<bool> _onStun;

    #endregion

    #region Initialize

    private void Awake() {
        _anim = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody>();
        _statusManager = GetComponent<StatusManager>();
        _stunManager = GetComponent<StunManager>();

        _onStun = (bool isStunned) => {
            BlockMovement(isStunned);
        };
    }

    private void Start() {
        _cameraCenter = PlayerManager.Instance.CameraCenter;

        _stunManager.OnStun += _onStun;
    }

    private void OnDestroy() {
        _stunManager.OnStun -= _onStun;
    }
    #endregion

    #region Input Events
    public void OnRotate(InputAction.CallbackContext ctx) {
        if (!_canRotate || !_canMove || Time.timeScale == 0) return;

        _mousePosition = ctx.ReadValue<Vector2>();
    }

    public void OnPause(InputAction.CallbackContext ctx) {
        if (ctx.phase == InputActionPhase.Performed) {
            if (!_isPaused) {
                _isPaused = true;
                ScreensInGameUI.Instance.TurnScreenOn(TypeOfScreen.Pause);
            }
            else {
                _isPaused = false;
                ScreensInGameUI.Instance.TurnScreenOff(TypeOfScreen.Pause);
            }
        }
    }

    #endregion

    #region Update
    private void FixedUpdate() {
        Walk();
        Rotate();
    }

    private void Walk() {
        if (!_canMove || !_canWalk) {
            _xInput = 0;
            _zInput = 0;
        }
        else {
            Vector2 value = moveActionReference.action.ReadValue<Vector2>();
            _xInput = value.x;
            _zInput = value.y;
        }

        if (!_isDashing) {
            float moveSpeed = _statusManager.ReturnStatusValue(StatusType.MoveSpeed);
            Vector3 movedir = new Vector3(_xInput, 0, _zInput).normalized;
            Vector3 moveDirection = _cameraCenter.transform.TransformDirection(movedir);
            _rb.linearVelocity = moveDirection * moveSpeed;
        }

        UpdateWalkingAnimation();
    }

    void Rotate() {
        if (!_canRotate || !_canMove || Time.timeScale == 0) return;

        if (_rotationType == RotationType.MouseRotation) {
            RotateMouse(true);
        }

        else {
            Vector3 input = new(_xInput, 0f, _zInput);
            Vector3 moveDirection = _cameraCenter.transform.TransformDirection(input);

            if (moveDirection.sqrMagnitude > 0.001f) {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    public void RotateMouse(bool lerp) {
        if (!_canRotate || !_canMove || Time.timeScale == 0) return;

        Ray ray = Camera.main.ScreenPointToRay(_mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, floorLayer)) {
            Vector3 direction = hit.point - transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude > 0.001f) {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                if (lerp)
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                else
                    transform.rotation = targetRotation;
            }
        }
    }
    #endregion

    #region Setters
    public void BlockMovement(bool block) => _canMove = !block;
    public void BlockWalk(bool block) => _canWalk = !block;
    public void BlockRotation(bool block) => _canRotate = !block;
    public void ChangeRotationType(RotationType type) => _rotationType = type;
    public void ChangeIsDashing(bool isDashing) => _isDashing = isDashing;

    #endregion

    #region Animation
    public void UpdateWalkingAnimation() {
        bool isWalking = new Vector2(_xInput, _zInput).magnitude > 0.1f;
        _anim.SetBool(walkingAnimationParameter, isWalking);
    }
    #endregion

}
