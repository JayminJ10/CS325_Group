using UnityEngine;

public class CandleMechanics : MonoBehaviour
{
    public PlayerMovement playerMovement;        // Reference to the PlayerMovement script to access stamina
    public ParticleSystem flameEffect;           // The flame particle system, which includes player light
    public Light playerLight;                    // The light within the flame particle system
    public Light secondaryLight;                 // The secondary light for illuminating the player's face

    private float initialPlayerLightIntensity;    // The initial intensity of the player light
    private float initialSecondaryLightIntensity; // The initial intensity of the secondary light

    private bool isLightOn = true;               // Whether the light is currently on
    private bool isShiningBrighter = false;      // Whether the light is shining brighter
    private float increaseIntensity = 1f;        // Intensity multiplier for brightening

    private ParticleSystem.MainModule flameMainModule;     // Main module of the flame particle system
    private ParticleSystem.EmissionModule flameEmission;   // Emission module to control particle count

    void Start()
    {
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
        HandleLightControl();    // Check for player input to turn off/on the light or make it brighter
        UpdateFlameProperties(); // Update flame properties based on stamina
        HandleStamina();         // Handle stamina drain/regen based on light state
    }

    void HandleLightControl()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isLightOn = !isLightOn; // Toggle the light on/off

            if (isLightOn)
            {
                flameEffect.Play();
                if (playerLight != null) playerLight.enabled = true;
                if (secondaryLight != null) secondaryLight.enabled = true;
            }
            else
            {
                flameEffect.Stop();
                if (playerLight != null) playerLight.enabled = false;
                if (secondaryLight != null) secondaryLight.enabled = false;
            }

            increaseIntensity = isLightOn ? 1f : 0f;
        }

        if (isLightOn && Input.GetMouseButton(0))
        {
            isShiningBrighter = true;
            increaseIntensity = 1.3f; // Adjusted multiplier for more controlled brightness increase
        }
        else
        {
            isShiningBrighter = false;
            increaseIntensity = 1f;
        }
    }

    void UpdateFlameProperties()
    {
        if (flameEffect != null && isLightOn)
        {
            float staminaRatio = Mathf.Clamp(playerMovement.currentStamina / playerMovement.maxStamina, 0.01f, 1f);

            // Set particle count based on stamina, fewer particles at lower stamina
            flameEmission.rateOverTime = Mathf.Lerp(1f, 80f, staminaRatio) * increaseIntensity;

            // Adjust the flame’s speed, size, and brightness based on stamina
            flameMainModule.startSpeed = Mathf.Lerp(0.1f, 0.4f, staminaRatio) * increaseIntensity;
            flameMainModule.startSize = Mathf.Lerp(0.05f, 0.2f, staminaRatio) * increaseIntensity;

            // Update the player light's intensity with separate ranges for regular and brighter states
            float baseIntensity = Mathf.Lerp(0.4f * initialPlayerLightIntensity, initialPlayerLightIntensity, staminaRatio);
            if (isShiningBrighter)
            {
                playerLight.intensity = Mathf.Lerp(playerLight.intensity, baseIntensity * increaseIntensity, Time.deltaTime * 5f);
            }
            else
            {
                playerLight.intensity = Mathf.Lerp(playerLight.intensity, baseIntensity, Time.deltaTime * 5f);
            }

            // Adjust secondary light's intensity based on stamina, dimming to almost zero
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
            if (isShiningBrighter)
            {
                playerMovement.currentStamina -= playerMovement.staminaDrainRate * 2.5f * Time.deltaTime; // Slightly slower drain when brighter
            }
            else
            {
                playerMovement.currentStamina -= playerMovement.staminaDrainRate * Time.deltaTime; // Normal drain
            }

            playerMovement.currentStamina = Mathf.Clamp(playerMovement.currentStamina, 0, playerMovement.maxStamina);
        }
        else
        {
            playerMovement.currentStamina += playerMovement.staminaRegenRate * Time.deltaTime;
            playerMovement.currentStamina = Mathf.Clamp(playerMovement.currentStamina, 0, playerMovement.maxStamina);
        }
    }
}
