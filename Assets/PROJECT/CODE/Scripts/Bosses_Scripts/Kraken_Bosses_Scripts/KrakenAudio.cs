using UnityEngine;
using System.Collections;

public class KrakenAudio : MonoBehaviour
{
    [Header("Idle Audio Event")]
    [SerializeField] AK.Wwise.Event tentacleIdleEvent;

    [Header("Attack Audio Events")]
    [SerializeField] AK.Wwise.Event warningTentacleAttackEvent;
    [SerializeField] AK.Wwise.Event tentacleSwingEvent;
    [SerializeField] AK.Wwise.Event airSwingEvent;
    [SerializeField] AK.Wwise.Event tentacleImpactEvent;
    [SerializeField] AK.Wwise.Event rocksCrackingEvent;
    [SerializeField] AK.Wwise.Event waterSplashingEvent;

    [Header("Prepare Audio Event")]
    [SerializeField] AK.Wwise.Event tentaclePrepareEvent;

    [Header("Get Back Up Audio Events")]
    [SerializeField] AK.Wwise.Event tentacleDragEvent;
    [SerializeField] AK.Wwise.Event tentacleRisingEvent;

    [Header("Stalactite Audio Events")]
    [SerializeField] AK.Wwise.Event tentacleStalactiteWipEvent;
    [SerializeField] AK.Wwise.Event stopTentacleStalactiteWipEvent;

    [Header("RTPCs")]
    [SerializeField] AK.Wwise.RTPC idleVolumeRTPC;

    private Coroutine fadeCoroutine;
    private bool warningPlayed = false;

    public void PlayIdle()
    {
        tentacleIdleEvent?.Post(gameObject);
        FadeIdleTo(50f, 0.5f);
    }

    public void PlayWarning()
    {
        if (!warningPlayed)
        {
            FadeIdleTo(20f, 0.5f);
            warningTentacleAttackEvent?.Post(gameObject);
            warningPlayed = true;
            StartCoroutine(ResetWarningAfterDelay(20f));
        }
    }

    public void PlayTentacleSwing()
    {
        tentacleSwingEvent?.Post(gameObject);
    }

    public void PlayAirSwing()
    {
        airSwingEvent?.Post(gameObject);
    }

    public void PlayImpact()
    {
        tentacleImpactEvent?.Post(gameObject);
    }

    public void PlayRocksCracking()
    {
        rocksCrackingEvent?.Post(gameObject);
    }

    public void PlayWaterSplashing()
    {
        waterSplashingEvent?.Post(gameObject);
    }

    public void PlayPrepare()
    {
        FadeIdleTo(20f, 0.5f);
        tentaclePrepareEvent?.Post(gameObject);
    }

    public void PlayDrag()
    {
        FadeIdleTo(20f, 0.5f);
        tentacleDragEvent?.Post(gameObject);
    }

    public void PlayRising()
    {
        tentacleRisingEvent?.Post(gameObject);
    }

    public void PlayStalactiteWip()
    {
        FadeIdleTo(20f, 0.5f);
        tentacleStalactiteWipEvent?.Post(gameObject);
    }

    public void StopStalactiteWip()
    {
        stopTentacleStalactiteWipEvent?.Post(gameObject);
    }

    public void ResumeIdle()
    {
        FadeIdleTo(50f, 0.5f);
    }

    private void FadeIdleTo(float targetValue, float duration)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRTPC(targetValue, duration));
    }

    private IEnumerator FadeRTPC(float targetValue, float duration)
    {
        float currentValue = 50f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newValue = Mathf.Lerp(currentValue, targetValue, elapsed / duration);
            idleVolumeRTPC?.SetValue(gameObject, newValue);
            yield return null;
        }

        idleVolumeRTPC?.SetValue(gameObject, targetValue);
    }

    private IEnumerator ResetWarningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        warningPlayed = false;
    }
}




