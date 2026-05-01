using UnityEngine;
using UnityEngine.VFX;


public class HeatpUpVFXManager : MonoBehaviour
{

    private VisualEffect myVFX;
    private SkinnedMeshRenderer playerSkinnedMesh;
    void Start()
    {
        playerSkinnedMesh = GameObject.Find("Corpo").GetComponent<SkinnedMeshRenderer>();
        Debug.Log(playerSkinnedMesh);
        myVFX = GetComponent<VisualEffect>();
        myVFX.SetSkinnedMeshRenderer("playerSkinnedMesh", playerSkinnedMesh);
    }

    void Update()
    {
        
    }
}
