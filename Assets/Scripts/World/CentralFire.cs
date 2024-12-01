using System.Collections.Generic;
using UnityEngine;

public class CentralFire : MonoBehaviour
{
    [Header("References")]
    public List<CandleToNextLevel> candles;  // List of all candles in the circle
    public Light centralFireLight;          // Light component for the central fire
    public ParticleSystem centralFireEffect; // Particle system for the central fire
    public AudioSource fireAudio;           // Optional: Sound effect for the central fire

    [Header("Settings")]
    public float initialFireSpeed = 2f;     // Initial speed of fire particles
    public float maxFireSpeed = 8f;         // Maximum speed of fire particles
    public float initialFireHeight = 1.5f;  // Initial lifetime of particles (height effect)
    public float maxFireHeight = 4f;        // Maximum lifetime of particles (height effect)
    public float initialStartSize = 0.5f;   // Initial particle size
    public float maxStartSize = 4f;       // Maximum particle size
    public float maxLightIntensity = 8f;    // Maximum light intensity when all candles are lit
    public int initialEmissionRate = 50;    // Initial particle emission rate
    public int maxEmissionRate = 300;       // Maximum particle emission rate
    public float checkInterval = 0.5f;      // How often to check candle status

    private int litCandleCount = 0;         // Number of candles currently lit
    private bool isFireActive = false;      // Whether the fire has started

    void Start()
    {
        // Disable central fire visuals at the start
        if (centralFireLight != null)
        {
            centralFireLight.enabled = false;
            centralFireLight.intensity = 0;
        }
        if (centralFireEffect != null)
        {
            centralFireEffect.Stop();
        }

        // Start periodic checks for candle status
        InvokeRepeating(nameof(CheckCandles), 0, checkInterval);
    }

    void CheckCandles()
    {
        // Count how many candles are lit
        int currentLitCount = 0;
        foreach (CandleToNextLevel candle in candles)
        {
            if (candle.IsLit)
            {
                currentLitCount++;
            }
        }

        // If no new candles are lit, return early
        if (currentLitCount == litCandleCount) return;

        // Update lit candle count
        litCandleCount = currentLitCount;

        // Activate the fire if not already active
        if (!isFireActive && litCandleCount > 0)
        {
            ActivateFire();
        }

        // Adjust fire appearance based on lit candles
        UpdateFireAppearance();
    }

    void ActivateFire()
    {
        isFireActive = true;

        // Enable the light and particle system
        if (centralFireLight != null)
        {
            centralFireLight.enabled = true;
        }
        if (centralFireEffect != null)
        {
            centralFireEffect.Play();
        }
        if (fireAudio != null)
        {
            fireAudio.Play();
        }

        Debug.Log("Central fire has been activated!");
    }

    void UpdateFireAppearance()
    {
        float progress = (float)litCandleCount / candles.Count;

        // Scale the light intensity
        if (centralFireLight != null)
        {
            centralFireLight.intensity = Mathf.Lerp(0, maxLightIntensity, progress);
        }

        // Adjust particle system properties
        if (centralFireEffect != null)
        {
            var main = centralFireEffect.main;
            main.startSpeed = Mathf.Lerp(initialFireSpeed, maxFireSpeed, progress); // Speed up flames
            main.startLifetime = Mathf.Lerp(initialFireHeight, maxFireHeight, progress); // Extend flame height
            main.startSize = Mathf.Lerp(initialStartSize, maxStartSize, progress); // Add volume to the flames

            var emission = centralFireEffect.emission;
            emission.rateOverTime = Mathf.Lerp(initialEmissionRate, maxEmissionRate, progress); // Increase particle density

            var noise = centralFireEffect.noise;
            noise.enabled = true;
            noise.strength = Mathf.Lerp(0.1f, 1f, progress); // Add slight turbulence
            noise.frequency = Mathf.Lerp(0.2f, 0.8f, progress); // Smooth out the noise frequency
        }

        Debug.Log($"Central fire updated: {litCandleCount}/{candles.Count} candles lit.");
    }
}
