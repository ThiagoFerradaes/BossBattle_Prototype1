using Unity.Mathematics;
using UnityEngine;

public class AxeStoneVFXManager : MonoBehaviour
{
    GameObject player;
    void Start()
    {
        player = PlayerManager.Instance.Player;
        //transform.SetParent(player.transform);
    }

    void Update()
    {
        transform.rotation = Quaternion.identity;
        transform.position = player.transform.position;
    }
}
