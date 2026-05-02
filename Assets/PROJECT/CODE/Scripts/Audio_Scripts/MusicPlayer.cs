using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    [SerializeField] bool hasSwitch;
    [SerializeField] bool hasAmbience;
    [SerializeField, ShowIf("hasSwitch")] AK.Wwise.Switch musicSwitch;

    [SerializeField] AK.Wwise.Event newMusic = null;
    [SerializeField, ShowIf("hasAmbience")] List<AK.Wwise.Event> newAmbienceList = null;

    private void Start() {
        AkUnitySoundEngine.StopAll();

        PlayMusic();
        PlayAmbience();
    }

    void PlayMusic() {
        if (hasSwitch) musicSwitch.SetValue(gameObject);

        newMusic.Post(gameObject);
    }

    void PlayAmbience() {
        if (hasAmbience) {
            foreach (var ambience in newAmbienceList) {
                ambience.Post(gameObject);
            }
        }
    }

}
