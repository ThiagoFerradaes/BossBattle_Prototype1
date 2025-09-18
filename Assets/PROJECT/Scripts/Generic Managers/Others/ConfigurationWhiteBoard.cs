using UnityEngine;

public class ConfigurationWhiteBoard : MonoBehaviour
{
    public static ConfigurationWhiteBoard Instance;

    public bool PreCastOn = false;
    public bool DashToMouse = true;

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
