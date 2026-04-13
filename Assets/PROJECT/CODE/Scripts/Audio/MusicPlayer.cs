using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] bool hasSwitch = false;
    [SerializeField, ShowIf("hasSwitch")] AK.Wwise.Switch musicSwitch = null;
    [SerializeField] AK.Wwise.Event newMusic = null;

    private void Start() {
        AkUnitySoundEngine.StopAll();
        if (hasSwitch) musicSwitch.SetValue(gameObject);
        newMusic.Post(gameObject);
    }
}
