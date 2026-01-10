using UnityEngine;

public class FireFaceCamera : MonoBehaviour
{
    void Update()
    {
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform.position);

            // The default Unity quad's visible side faces the negative Z-axis (backward).
            // To display the visible side to the camera, reverse the forward direction.
            transform.forward = -transform.forward;
        }
    }
}