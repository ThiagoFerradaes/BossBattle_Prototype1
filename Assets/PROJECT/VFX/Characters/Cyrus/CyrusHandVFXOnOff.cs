using UnityEngine;
using UnityEngine.VFX;

[ExecuteInEditMode]
public class CyrusHandVFXOnOff : MonoBehaviour
{
    
    [SerializeField]
    VisualEffect leftHandVFX, rightHandVFX;
    
    [SerializeField]
    bool toggleVFX_leftHand, toggleVFX_rightHand;

    bool testBool_leftHand, testBool_rightHand;

    void Start()
    {
        //test
    }

    void Update()
    {
        if(toggleVFX_leftHand) {
            leftHandVFX.enabled = true;
        }
        else if(!toggleVFX_leftHand)
        {
            leftHandVFX.enabled = false;
        }

        if(toggleVFX_rightHand) {
            rightHandVFX.enabled = true;
        }
        else if(!toggleVFX_rightHand)
        {
            rightHandVFX.enabled = false;
        }
        /*
        if(toggleVFX_leftHand && !testBool_leftHand)
        {
            leftHandVFX.Reinit();
            testBool_leftHand = true;
        }
        if(!toggleVFX_leftHand)
        {
            testBool_leftHand = false;
        }
        
        if(toggleVFX_rightHand && !testBool_rightHand)
        {
            rightHandVFX.Reinit();
            testBool_rightHand = true;
        }
        if(!toggleVFX_rightHand)
        {
            testBool_rightHand = false;
        }

        leftHandVFX.SetBool("emit_trail", toggleVFX_leftHand);
        rightHandVFX.SetBool("emit_trail", toggleVFX_rightHand);*/
    }
}
