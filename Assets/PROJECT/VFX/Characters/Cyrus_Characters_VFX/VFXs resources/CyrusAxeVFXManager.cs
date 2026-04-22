using UnityEngine;
using UnityEngine.VFX;

public class CyrusAxeVFXManager : MonoBehaviour
{
    private VisualEffect myVFX;
    private void Awake()
    {
        myVFX = GetComponent<VisualEffect>();
        CyrusAxeAttackManager.OnAxeDown += StopVFX;

    }
    private void StopVFX()
    {
        myVFX.SendEvent("MyStopEvent");
        Debug.Log("Axe Down");
    }
}
