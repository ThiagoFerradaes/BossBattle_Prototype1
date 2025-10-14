using UnityEngine;

/// <summary>
/// Static class responsible for managing game configuration access.
/// Provides singleton-like access to the game's configuration data.
/// </summary>
public static class GameConfig
{
    /// <summary>
    /// Private reference to the configuration-scriptable object instance
    /// </summary>
    private static ConfigurationSo _config;

    /// <summary>
    /// Public property to access the game configuration.
    /// Loads the configuration from the Resources folder if not already loaded.
    /// </summary>
    public static ConfigurationSo Config
    {
        get
        {
            if (_config != null) return _config;
            _config = Resources.Load<ConfigurationSo>("Configuration/ConfigurationSO");
            if (_config == null)
                Debug.LogError("ConfigurationSO not found in Resources/Configuration!");
            return _config;
        }
    }
}
