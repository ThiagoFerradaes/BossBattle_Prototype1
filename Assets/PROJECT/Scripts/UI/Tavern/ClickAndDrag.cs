using UnityEngine;
using UnityEngine.InputSystem;

public class ClickAndDrag : MonoBehaviour
{
    [Header("Camera movement")]
    [SerializeField] float xlimit;
    [SerializeField] float zlimit;
    [SerializeField] private float dragSpeed = 0.01f;

    private Vector2 _lastMousePosition;
    private bool _isDragging;
   
    [Header("Camera Zoom")]
    [SerializeField] float yMinlimit;
    [SerializeField] float yMaxlimit;
    [SerializeField] float zoomSpeed;
    private float targetY;

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
        HandleDrag();
        HandleZoom();
        ApplyZoom();
    }

    void HandleDrag() {
        if (!_isDragging) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 delta = mousePosition - _lastMousePosition;
        _lastMousePosition = mousePosition;


        Vector3 move = new(-delta.x * dragSpeed, 0, -delta.y * dragSpeed);

        Vector3 pos = transform.position + move;
        pos.x = Mathf.Clamp(pos.x, -xlimit, xlimit);
        pos.z = Mathf.Clamp(pos.z, -zlimit, zlimit);

        transform.position = pos;
    }

    private void HandleZoom() {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) > 0.01f) {
            targetY -= scroll * zoomSpeed * Time.deltaTime; 
            targetY = Mathf.Clamp(targetY, yMinlimit, yMaxlimit); 
        }
    }

    private void ApplyZoom() {
        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * 10); 
        transform.position = pos;
    }
}
