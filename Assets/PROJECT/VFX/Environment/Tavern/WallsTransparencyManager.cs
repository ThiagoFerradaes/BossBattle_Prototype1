using UnityEngine;

public class WallsTransparencyManager : MonoBehaviour
{

    [SerializeField]
    GameObject manager;
    [SerializeField]
    Material wallMaterial;

    PlayerManager playerManager;

    void Start()
    {
        playerManager = manager.GetComponent<PlayerManager>();
    }

    void Update()
    {
        wallMaterial.SetVector("_Character_Position", playerManager.Player.transform.position);
        //Debug.Log(playerManager.Player.transform.position);
    }
}
