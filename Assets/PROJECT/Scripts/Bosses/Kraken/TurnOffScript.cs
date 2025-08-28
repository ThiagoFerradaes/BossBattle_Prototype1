using System.Collections;
using UnityEngine;

public class TurnOffScript : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitToTurnOff());
    }

    IEnumerator WaitToTurnOff() {
        yield return new WaitForEndOfFrame();

        gameObject.SetActive(false);
    }
}
