using UnityEngine;
using UnityEngine.UI;


public class SettingsMenu : MonoBehaviour
{

    public Slider volumeSlider;
    void Start()
    {
        // Set the initial slider value to match the current audio volume
        volumeSlider = GetComponent<Slider>();
        volumeSlider.value = AudioListener.volume;
    }

    public void SetVolume(float newValue)
    {
        // Adjust the global audio volume based on slider value
        float newVol = AudioListener.volume;
        newVol = newValue;
        AudioListener.volume = newVol;
    }

}
