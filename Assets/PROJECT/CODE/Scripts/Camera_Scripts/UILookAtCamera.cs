using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
    [SerializeField] GameObject cameraObject;
    [SerializeField] bool updateRotation;

    private void Start()
    {
        transform.LookAt(cameraObject.transform);
    }
    void LateUpdate()
    {
        if (!updateRotation) return;

        transform.LookAt(cameraObject.transform);
    }


}
