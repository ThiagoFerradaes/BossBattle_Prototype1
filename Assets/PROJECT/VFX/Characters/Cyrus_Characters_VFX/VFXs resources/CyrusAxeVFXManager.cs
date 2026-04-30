using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class CyrusAxeVFXManager : MonoBehaviour
{

    [SerializeField]
    Vector3 translateVFX;
    
    private VisualEffect myVFX;
    private GameObject playerObj;

    private void Awake()
    {
        myVFX = GetComponent<VisualEffect>();
        CyrusAxeAttackManager.OnAxeUp += StartVFX;
        CyrusAxeAttackManager.OnAxeDown += StopVFX;
    }
    private void OnDestroy() {
        CyrusAxeAttackManager.OnAxeUp -= StartVFX;
        CyrusAxeAttackManager.OnAxeDown -= StopVFX;
    }
    void Update()
    {
        transform.position = playerObj.transform.position;
        transform.rotation = playerObj.transform.rotation;
        transform.Translate(translateVFX, Space.Self);
    }
    private void StartVFX(GameObject parentObj)
    {
        playerObj = parentObj;
        StartCoroutine(AxeUpCountdown(myVFX.GetFloat("axe_duration")));
    }
    private void StopVFX(GameObject parentObj)
    {
        myVFX.SendEvent("MyStopEvent");
        StartCoroutine(AxeDownCountdown(myVFX.GetFloat("axe_duration")));
    }

    private IEnumerator AxeDownCountdown(float totalTime)
    {
        float timeTrack = 0f;
        while (timeTrack < totalTime)
        {
            timeTrack += Time.deltaTime;
            float normalizedTime = timeTrack/totalTime;
            myVFX.SetFloat("axe_timer", normalizedTime);
            yield return null;
        }

    }

        private IEnumerator AxeUpCountdown(float totalTime)
    {
        float timeTrack = totalTime;
        while (timeTrack > 0f)
        {
            timeTrack -= Time.deltaTime;
            float normalizedTime = timeTrack/totalTime;
            myVFX.SetFloat("axe_timer", normalizedTime);
            yield return null;
        }

    }
}
