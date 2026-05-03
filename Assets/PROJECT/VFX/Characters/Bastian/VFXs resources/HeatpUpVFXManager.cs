using UnityEngine;
using UnityEngine.VFX;


public class HeatpUpVFXManager : MonoBehaviour
{

    private VisualEffect myVFX;
    private SkinnedMeshRenderer playerSkinnedMesh;
    private Transform bastianTransform;
    void Start()
    {
        playerSkinnedMesh = GameObject.Find("Corpo").GetComponent<SkinnedMeshRenderer>(); //temporary fix, change the findbyname later  
        bastianTransform = GameObject.Find("Bastian_Prefab(Clone)").GetComponent<Transform>(); //temporary fix, change the findbyname later  
        myVFX = transform.GetComponent<VisualEffect>();
        myVFX.SetSkinnedMeshRenderer("playerSkinnedMesh", playerSkinnedMesh);
    }

    void Update()
    {
        transform.position = bastianTransform.position; //temporary fix, change later
    }
}
