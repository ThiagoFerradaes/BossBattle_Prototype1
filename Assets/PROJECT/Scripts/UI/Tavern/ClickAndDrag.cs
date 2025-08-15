using UnityEngine;
using UnityEngine.InputSystem;

public class ClickAndDrag : MonoBehaviour
{
    private Vector2 _lastMousePosition;
    private bool _isDragging;

    [SerializeField] private float dragSpeed = 0.1f;

    public void OnDrag(InputAction.CallbackContext ctx) {
        if (ctx.started) {
            _lastMousePosition = Mouse.current.position.ReadValue();
            _isDragging = true;
        }
        else if (ctx.canceled) {
            _isDragging = false;
        }
    }

    private void LateUpdate() {
        if (!_isDragging) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 delta = mousePosition - _lastMousePosition;
        _lastMousePosition = mousePosition;

        Vector3 move = new(-delta.x * dragSpeed, 0, -delta.y * dragSpeed);
        transform.Translate(move, Space.World);
    }
}
