using NaughtyAttributes;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    [SerializeField] bool hasSwitch;
    [SerializeField, ShowIf("hasSwitch")] AK.Wwise.Switch musicSwitch;

    [SerializeField] AK.Wwise.Event newMusic = null;

    private void Start() {
        AkUnitySoundEngine.StopAll();

        if (hasSwitch) musicSwitch.SetValue(gameObject);

        newMusic.Post(gameObject);
    }

}
