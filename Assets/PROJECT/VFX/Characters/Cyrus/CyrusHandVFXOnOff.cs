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
        if(toggleVFX_leftHand)
        {
            leftHandVFX.enabled = true;
        }
        else
        {
            leftHandVFX.enabled = false;
        }
        
        if(toggleVFX_rightHand)
        {
            rightHandVFX.enabled = true;
        }
        else
        {
            rightHandVFX.enabled = false;
        }
        //leftHandVFX.SetBool("emit_trail", toggleVFX_leftHand);
        //rightHandVFX.SetBool("emit_trail", toggleVFX_rightHand);
    }
}
