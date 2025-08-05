using UnityEngine;

public class KrakenCamera : MonoBehaviour
{
    Transform _player;
    [SerializeField] float rotationSpeed = 2f;
    [SerializeField] float deadAngle = 3f;

    private void Start() {
        _player = PlayerSpawnManager.Instance.Player.transform;
    }
    void Update() {
        if (_player == null) return;

        Vector3 playerDir = transform.position - _player.position;
        playerDir.y = 0;

        if (playerDir.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(playerDir);
        float angle = Quaternion.Angle(transform.rotation, targetRotation);

        if (angle > deadAngle) {
            transform.rotation  = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            Debug.Log("angle" + targetRotation);
        }
    }
}
