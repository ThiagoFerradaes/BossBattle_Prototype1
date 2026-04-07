using UnityEngine;
using UnityEngine.VFX;

[ExecuteInEditMode]
public class CyrusHandVFXOnOff : MonoBehaviour
{
    
    [SerializeField]
    VisualEffect leftHandVFX, rightHandVFX;
    
    [SerializeField]
    bool toggleVFX_leftHand, toggleVFX_rightHand;


    void Start()
    {
        //test
    }

    void Update()
    {
        leftHandVFX.SetBool("emit_trail", toggleVFX_leftHand);
        rightHandVFX.SetBool("emit_trail", toggleVFX_rightHand);
    }
}
