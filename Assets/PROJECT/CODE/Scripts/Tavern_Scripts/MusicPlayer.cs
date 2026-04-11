using UnityEngine;

public class MusicPlayer : MonoBehaviour {
    [SerializeField] AK.Wwise.Event newMusic = null;

    private void Start() {
        AkUnitySoundEngine.StopAll();
        newMusic.Post(gameObject);
    }

}
