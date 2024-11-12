using UnityEngine;

public class CandleMechanics : MonoBehaviour
{
    public PlayerMovement playerMovement;        // Reference to PlayerMovement to access stamina controls
    public PlayerStats playerStats;              // Reference to PlayerStats for stamina management
    public ParticleSystem flameEffect;           // Flame particle system
    public Light playerLight;                    // Light in the flame particle system
    public Light secondaryLight;                 // Secondary light for player's face

    [SerializeField] private AudioClip candleOn;
    [SerializeField] private AudioClip candleOff;
    private float defaultVolume = 0.7f;
    private AudioSource audioSource;

    private float initialPlayerLightIntensity;    // Initial player light intensity
    private float initialSecondaryLightIntensity; // Initial secondary light intensity

    private bool isLightOn = true;               // Whether the light is currently on
    private bool isShiningBrighter = false;      // Whether the light is shining brighter
    private float increaseIntensity = 1f;        // Intensity multiplier for brightening

    private ParticleSystem.MainModule flameMainModule;     // Main module of the flame particle system
    private ParticleSystem.EmissionModule flameEmission;   // Emission module to control particle count

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (flameEffect != null)
        {
            flameMainModule = flameEffect.main;
            flameEmission = flameEffect.emission;

            if (playerLight != null)
            {
                initialPlayerLightIntensity = playerLight.intensity;
            }
            if (secondaryLight != null)
            {
                initialSecondaryLightIntensity = secondaryLight.intensity;
            }
        }
    }

    void Update()
    {
        HandleLightControl();    // Check for player input to toggle light or make it brighter
        UpdateFlameProperties(); // Update flame properties based on stamina level
        HandleStamina();         // Manage stamina drain/regen based on light state
    }

    void HandleLightControl()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isLightOn = !isLightOn; // Toggle the light on/off

            if (isLightOn)
            {
                CandleOpenSound();
                flameEffect.Play();
                if (playerLight != null) playerLight.enabled = true;
                if (secondaryLight != null) secondaryLight.enabled = true;
            }
            else
            {
                CandleCloseSound();
                flameEffect.Stop();
                if (playerLight != null) playerLight.enabled = false;
                if (secondaryLight != null) secondaryLight.enabled = false;
            }

            increaseIntensity = isLightOn ? 1f : 0f;
        }

        if (isLightOn && Input.GetMouseButton(0))
        {
            isShiningBrighter = true;
            increaseIntensity = 1.3f; // Adjusted multiplier for controlled brightness increase
        }
        else
        {
            isShiningBrighter = false;
            increaseIntensity = 1f;
        }
    }

    //Update audio source settings to handle candle open audio
    void CandleOpenSound()
    {
        audioSource.Pause();
        audioSource.clip = candleOn;
        audioSource.volume = defaultVolume * 1.1f;
        audioSource.loop = false;
        audioSource.Play();
    }

    //Update audio source settings to handle candle close audio
    void CandleCloseSound()
    {
        audioSource.Pause();
        audioSource.clip = candleOff;
        audioSource.volume = defaultVolume * 0.7f;
        audioSource.loop = false;
        audioSource.Play();
    }

    void UpdateFlameProperties()
    {
        if (flameEffect != null && isLightOn)
        {
            float staminaRatio = Mathf.Clamp(playerStats.currentStamina / playerStats.maxStamina, 0.01f, 1f);

            // Adjust particle count, speed, and size based on stamina
            flameEmission.rateOverTime = Mathf.Lerp(1f, 80f, staminaRatio) * increaseIntensity;
            flameMainModule.startSpeed = Mathf.Lerp(0.1f, 0.4f, staminaRatio) * increaseIntensity;
            flameMainModule.startSize = Mathf.Lerp(0.05f, 0.2f, staminaRatio) * increaseIntensity;

            // Update player light intensity with different ranges for regular and brighter states
            float baseIntensity = Mathf.Lerp(0.4f * initialPlayerLightIntensity, initialPlayerLightIntensity, staminaRatio);
            playerLight.intensity = Mathf.Lerp(playerLight.intensity, isShiningBrighter ? baseIntensity * increaseIntensity : baseIntensity, Time.deltaTime * 5f);

            // Adjust secondary light's intensity based on stamina
            if (secondaryLight != null)
            {
                secondaryLight.intensity = Mathf.Lerp(0f, initialSecondaryLightIntensity, staminaRatio);
            }
        }
        else if (!isLightOn)
        {
            if (playerLight != null) playerLight.enabled = false;
            if (secondaryLight != null) secondaryLight.enabled = false;
        }
    }

    void HandleStamina()
    {
        if (isLightOn)
        {
            float drainRate = isShiningBrighter ? playerMovement.staminaDrainRate * 2.5f : playerMovement.staminaDrainRate;
            playerStats.ReduceStamina(drainRate * Time.deltaTime);
        }
    }

    public void IncreaseStamina(float amount)
    {
        playerStats.maxStamina += amount;
        UpdateFlameProperties();
    }

    public void IncreaseRegen(float amount)
    {
        playerStats.staminaRegenRate += amount;
    }
}
