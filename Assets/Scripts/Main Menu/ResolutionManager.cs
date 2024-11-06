using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ResolutionManager : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;  // Reference for the fullscreen toggle

    private List<Resolution> uniqueResolutions;

    void Start()
    {
        // Get all available screen resolutions
        Resolution[] allResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        // Track unique resolutions by width and height
        HashSet<string> uniqueResolutionStrings = new HashSet<string>();
        uniqueResolutions = new List<Resolution>();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < allResolutions.Length; i++)
        {
            // Create a string to represent the width and height
            string resolutionString = allResolutions[i].width + " x " + allResolutions[i].height;

            // Only add if it's not a duplicate width-height combination
            if (uniqueResolutionStrings.Add(resolutionString))
            {
                // Add to unique resolutions list and dropdown options
                uniqueResolutions.Add(allResolutions[i]);
                options.Add(resolutionString);

                // Check if this is the current resolution
                if (allResolutions[i].width == Screen.currentResolution.width &&
                    allResolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = uniqueResolutions.Count - 1;
                }
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Set initial fullscreen state
        fullscreenToggle.isOn = Screen.fullScreen;

        // Add listeners to handle resolution and fullscreen changes
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = uniqueResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
