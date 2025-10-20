using UnityEngine;

public class ConfigurationWhiteBoard : MonoBehaviour
{
    public static ConfigurationWhiteBoard Instance;

    private ConfigurationSo _config;
    
    public bool PreCastOn = false;
    public bool DashToMouse = true;

    private void Awake() {
        if (Instance == null) {
            _config = GameConfig.Config;
            Instance = this;
            DontDestroyOnLoad(this);
            _config.LoadFromJson();
        }
        else {
            Destroy(this);
        }
    }
    
    private void OnApplicationQuit()
    {
        _config.SaveToJson();
    }
}
