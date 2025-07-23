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

    // Animation
    [Header("Animation")]
    [SerializeField] string xMovementParameterName;
    [SerializeField] string zMovementParameterName;

    // Components
    Animator anim;
    Rigidbody rb;

    // Atributes
    [Header("Atributes")]
    [SerializeField] float speed;

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
    }

    public void OnDash(InputAction.CallbackContext ctx) {
        if (ctx.performed) BlockMovement(!_canMove);
    }

    #endregion

    #region Update
    private void FixedUpdate() {
        Walk();
    }

    private void Walk() {
        if (!_canMove || !_canWalk) {
            xInput = 0;
            zInput = 0;
        }

        rb.linearVelocity = new Vector3(xInput * speed, 0, zInput * speed);
        anim.SetFloat(xMovementParameterName, xInput);
        anim.SetFloat(zMovementParameterName, zInput);
    }
    #endregion
    #region Setters
    public void BlockMovement(bool block) => _canMove = block;
    public void BlockWalk(bool block) => _canWalk = block;
    public void BlockRotation(bool block) => _canRotate = block;
    public void BlockDash(bool block) => _canDash = block;

    #endregion
}
