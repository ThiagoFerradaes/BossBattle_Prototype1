using UnityEngine;

public static class GameConfig
{
    private static ConfigurationSo _config;

    public static ConfigurationSo Config
    {
        get
        {
            if (_config != null) return _config;
            _config = Resources.Load<ConfigurationSo>("Configuration/ConfigurationSO");
            if (_config == null)
                Debug.LogError("ConfigurationSO não encontrado em Resources/Configuration!");
            return _config;
        }
    }
}

