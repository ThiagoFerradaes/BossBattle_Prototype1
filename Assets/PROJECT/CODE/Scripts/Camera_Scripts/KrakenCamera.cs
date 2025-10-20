using System;
using System.Collections;
using UnityEngine;

public class KrakenCamera : MonoBehaviour {
    #region Parameters

    [Header("Atributes")]
    [SerializeField] float rotationSpeed = 2f;
    [SerializeField] float deadAngle = 3f;

    [Header("Camera Shake")]
    [SerializeField, Range(1, 5)] float shakeIntensity = 1f;
    [SerializeField] AnimationCurve shakeCurve;
    [SerializeField, Range(0, 1)] float shakeDuration = 1f;

    // Components
    Transform _player;

    #endregion

    #region Methods

    private void Start() {
        _player = PlayerManager.Instance.Player.transform;
        _player.GetComponent<HealthManager>().OnDamageTaken += ShakeCamera;

    }

    void Update()
    {
        if (_player == null) return;

        Vector3 playerDir = transform.position - _player.position;
        playerDir.y = 0;

        if (playerDir.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(playerDir);
        float angle = Quaternion.Angle(transform.rotation, targetRotation);

        if (angle > deadAngle)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

    }

    private void ShakeCamera(float damage)
    {
        StartCoroutine(CameraShaking(damage));
    }

    IEnumerator CameraShaking(float damage)
    {
        float elapsedTime = 0f;
        while (elapsedTime < shakeDuration)
        {
            float damageMultiplier = damage / 400f;
            Vector3 startPosition = transform.position;
            elapsedTime += Time.deltaTime;
            float shakeStrength = shakeCurve.Evaluate(elapsedTime / shakeDuration) * shakeIntensity + damageMultiplier;
            transform.position = startPosition + UnityEngine.Random.insideUnitSphere * shakeStrength;
            yield return null;
            transform.position = startPosition;
        }
    }

    #endregion
}
