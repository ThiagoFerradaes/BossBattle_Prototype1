using UnityEngine;

public class WallsTransparencyManager : MonoBehaviour
{

    //GameObject manager;
    [SerializeField]
    PlayerManager playerManager;

    void Start()
    {
        //playerManager = manager.GetComponent<PlayerManager>();
    }

    void Update()
    {
        Shader.SetGlobalVector("_CharScreenPos_Alpha", Camera.main.WorldToViewportPoint(playerManager.Player.transform.position));
    }
}
