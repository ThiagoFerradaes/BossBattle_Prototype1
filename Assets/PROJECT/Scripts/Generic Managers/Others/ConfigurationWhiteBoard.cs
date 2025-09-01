using UnityEngine;

public class ConfigurationWhiteBoard : MonoBehaviour
{
    public static ConfigurationWhiteBoard Instance;

    public bool PreCastOn = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(this);
        }
    }
}
