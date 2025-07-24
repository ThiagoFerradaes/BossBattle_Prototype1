using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementManager : MonoBehaviour {
    #region Parameters

    // Movement floats
    float zInput;
    float xInput;

    // Booleans
    bool _canMove = true;
    bool _canWalk = true;
    bool _canRotate = true;
    bool _canDash = true;
    bool _rotateWithMouse = false;

    // Animation
    [Header("Animation")]
    [SerializeField] string walkingAnimationParameter;
    //[SerializeField] string zMovementParameterName;

    // Components
    Animator anim;
    Rigidbody rb;

    // Atributes
    [Header("Atributes")]
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;

    // LayerMask
    [Header("Layer")]
    [SerializeField] LayerMask floorLayer;

    // Rotation
    Vector2 _mousePosition;

    #endregion

    #region Initialize

    private void Awake() {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    #endregion

    #region Input Events

    public void OnMove(InputAction.CallbackContext ctx) {
        if (!_canMove || !_canWalk) return;

        var value = ctx.ReadValue<Vector2>();

        xInput = value.x;
        zInput = value.y;

        if (ctx.phase == InputActionPhase.Started) anim.SetBool(walkingAnimationParameter, true); 
        else if (ctx.phase == InputActionPhase.Canceled) anim.SetBool(walkingAnimationParameter, false);
    }

    public void OnRotate(InputAction.CallbackContext ctx) {
        if (!_canRotate || !_canMove) return;

        _mousePosition = ctx.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext ctx) {
        if (ctx.performed) BlockMovement(!_canMove);
    }

    #endregion

    #region Update
    private void FixedUpdate() {
        Walk();
        Rotate();
    }

    private void Walk() {
        if (!_canMove || !_canWalk) {
            xInput = 0;
            zInput = 0;
        }

        rb.linearVelocity = new Vector3(xInput * speed, 0, zInput * speed);
    }

    void Rotate() {
        if (!_canRotate) return;

        if (_rotateWithMouse) {
            Ray ray = Camera.main.ScreenPointToRay(_mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, floorLayer)) {
                Vector3 direction = hit.point - transform.position;
                direction.y = 0;

               if (direction.sqrMagnitude > 0.001f) {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = targetRotation;
                } 
            }
        }

        else {
            Vector3 moveDirection = new (xInput, 0f, zInput);

            if (moveDirection.sqrMagnitude > 0.001f) {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
    #endregion

    #region Setters
    public void BlockMovement(bool block) => _canMove = !block;
    public void BlockWalk(bool block) => _canWalk = !block;
    public void BlockRotation(bool block) => _canRotate = !block;
    public void BlockDash(bool block) => _canDash = !block;

    #endregion

}
