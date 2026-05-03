using UnityEngine;
using UnityEngine.VFX;


public class HeatpUpVFXManager : MonoBehaviour
{

    private VisualEffect myVFX;
    private SkinnedMeshRenderer playerSkinnedMesh;
    GameObject player;
    private Transform bastianTransform;
    void Start()
    {
        playerSkinnedMesh = GameObject.Find("Corpo").GetComponent<SkinnedMeshRenderer>(); //temporary fix, change the findbyname later
        player = PlayerManager.Instance.Player;
        //bastianTransform = GameObject.Find("Bastian_Prefab(Clone)").GetComponent<Transform>(); //temporary fix, change the findbyname later  
        myVFX = transform.GetComponent<VisualEffect>();
        myVFX.SetSkinnedMeshRenderer("playerSkinnedMesh", playerSkinnedMesh);
    }

    void Update()
    {
        transform.position = player.transform.position;
    }
}
