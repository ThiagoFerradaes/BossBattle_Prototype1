using UnityEngine;
using UnityEngine.VFX;

public class StalactiteVFXManager : MonoBehaviour
{

    private VisualEffect myVFX;

    void Start()
    {
        myVFX = transform.GetComponentInChildren<VisualEffect>();
    }

    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        myVFX.SendEvent("OnCollision");
    }
}
