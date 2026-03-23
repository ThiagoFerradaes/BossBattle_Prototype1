using Unity.Cinemachine;
using UnityEngine;

public class TavernCameraManager : MonoBehaviour
{
    [SerializeField] CinemachineCamera cineCamera;
    void Start()
    {
        try
        {
            cineCamera.Follow = PlayerManager.Instance.Player.transform;
        }
        catch
        {
            Debug.Log("No Player to Follow");
        }
    }

}
