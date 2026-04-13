using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphicsConfig : MonoBehaviour
{
    [SerializeField] TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;
    List<Resolution> filteredResolutions = new();
    List<string> options = new();

    float currentRefreshRate;
    int currentResolutionIndex;

    private void Start() {
        
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;

        for (int i = 0; i < resolutions.Length; i++) {
            if ((float)resolutions[i].refreshRateRatio.value == currentRefreshRate) {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        for (int i = 0; i < filteredResolutions.Count; i++) {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height; 
            options.Add(resolutionOption);
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    void SetResolution(int resolutionIndex) {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);

        Debug.Log("Resolution set to: " + resolution.width + "x" + resolution.height);
    }
}
