using UnityEngine;
using UnityEngine.VFX;

[ExecuteInEditMode]
public class CyrusHandVFXColorManager : MonoBehaviour
{

    [SerializeField]
    Material handMaterial;

    VisualEffect vfx;

    Color handColor;
    Color tailColor;

    void Start()
    {
        vfx = GetComponent<VisualEffect>();
        
        handColor = handMaterial.GetColor("_Hand_Color");
        tailColor = handMaterial.GetColor("_Tail_Color");
        vfx.SetVector4("Color_1", handColor);
        vfx.SetVector4("Color_2", tailColor);
    }

    void Update()
    {
        #if UNITY_EDITOR
            handColor = handMaterial.GetColor("_Hand_Color");
            tailColor = handMaterial.GetColor("_Tail_Color");
            vfx.SetVector4("Color_1", handColor);
            vfx.SetVector4("Color_2", tailColor);
        #endif
    }
}
