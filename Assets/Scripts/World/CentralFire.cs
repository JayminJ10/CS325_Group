using System.Collections.Generic;
using UnityEngine;

public class CentralFire : MonoBehaviour
{
    [Header("References")]
    public List<CandleToNextLevel> candles;  // List of all candles in the level
    public Light centralFireLight;          // Light component for the central fire
    public ParticleSystem centralFireEffect; // Particle system for the central fire
    public AudioSource fireAudio;           // Optional: Sound effect for the central fire

    [Header("Settings")]
    public float initialFireSpeed = 2f;     // Initial speed of fire particles
    public float maxFireSpeed = 8f;         // Maximum speed of fire particles
    public float initialFireHeight = 1.5f;  // Initial lifetime of particles (height effect)
    public float maxFireHeight = 4f;        // Maximum lifetime of particles (height effect)
    public float initialStartSize = 0.5f;   // Initial particle size
    public float maxStartSize = 4f;         // Maximum particle size
    public float maxLightIntensity = 8f;    // Maximum light intensity when all candles are lit
    public int initialEmissionRate = 50;    // Initial particle emission rate
    public int maxEmissionRate = 300;       // Maximum particle emission rate

    void Update()
    {
        UpdateFireAppearance();
    }

    private void UpdateFireAppearance()
    {
        int currentlyLit = 0;

        // Count currently lit candles
        foreach (CandleToNextLevel candle in candles)
        {
            if (candle.IsLit)
            {
                currentlyLit++;
            }
        }

        // Adjust the fire's intensity and size dynamically
        float progress = (float)currentlyLit / candles.Count;

        // Activate the central fire light and particle effect if any candles are lit
        if (currentlyLit > 0)
        {
            if (centralFireLight != null && !centralFireLight.enabled)
            {
                centralFireLight.enabled = true;
            }
            if (centralFireEffect != null && !centralFireEffect.isPlaying)
            {
                centralFireEffect.Play();
            }
        }
        else
        {
            // Turn off the central fire if no candles are lit
            if (centralFireLight != null) centralFireLight.enabled = false;
            if (centralFireEffect != null) centralFireEffect.Stop();
            return; // No need to update further if no candles are lit
        }

        // Scale the light intensity
        if (centralFireLight != null)
        {
            centralFireLight.intensity = Mathf.Lerp(0, maxLightIntensity, progress);
        }

        // Adjust particle system properties
        if (centralFireEffect != null)
        {
            var main = centralFireEffect.main;
            main.startSpeed = Mathf.Lerp(initialFireSpeed, maxFireSpeed, progress);
            main.startLifetime = Mathf.Lerp(initialFireHeight, maxFireHeight, progress);
            main.startSize = Mathf.Lerp(initialStartSize, maxStartSize, progress);

            var emission = centralFireEffect.emission;
            emission.rateOverTime = Mathf.Lerp(initialEmissionRate, maxEmissionRate, progress);
        }
    }
}
