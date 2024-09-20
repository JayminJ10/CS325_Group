using UnityEngine;

public class CandleMechanics : MonoBehaviour
{
    public PlayerMovement playerMovement;    // Reference to the PlayerMovement script to access stamina
    public Light playerLight;                // The player's main candle light
    public Light secondaryLight;             // The secondary light (for illuminating the area)

    public float minFlickerIntensity = 0.8f; // Minimum flicker intensity
    public float maxFlickerIntensity = 1.2f; // Maximum flicker intensity
    public float flickerSpeed = 5f;          // Speed of flicker effect

    private float initialLightIntensity;     // The initial intensity of the main light
    private float initialSecondaryLightIntensity; // The initial intensity of the secondary light

    private bool isLightOn = true;           // Whether the light is currently on
    private bool isShiningBrighter = false;  // Whether the light is shining brighter
    private float increaseIntensity = 1f;    // Intensity multiplier for brightening

    void Start()
    {
        // Store the initial light intensities
        initialLightIntensity = playerLight.intensity;
        initialSecondaryLightIntensity = secondaryLight.intensity;
    }

    void Update()
    {
        HandleLightControl();    // Check for player input to turn off/on the light or make it brighter
        UpdateLightIntensity();  // Update light intensity based on stamina and flicker
        HandleStamina();         // Handle stamina drain/regen based on light state
    }

    void HandleLightControl()
    {
        // Toggle lights off/on when 'F' is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            isLightOn = !isLightOn; // Toggle the light on/off

            // Toggle both lights
            playerLight.enabled = isLightOn;
            secondaryLight.enabled = isLightOn;

            // Reset intensity multiplier when turning light off
            increaseIntensity = isLightOn ? 1f : 0f;
        }

        // If holding down left-click, the light shines brighter
        if (isLightOn && Input.GetMouseButton(0))
        {
            // Increase light intensity
            isShiningBrighter = true;
            increaseIntensity = 2f; // 50% brighter
        }
        else
        {
            // Reset light intensity back to normal
            isShiningBrighter = false;
            increaseIntensity = 1f;
        }
    }

    void UpdateLightIntensity()
    {
        // If the light is on and stamina is above 0
        if (playerMovement.currentStamina > 0 && isLightOn)
        {
            // Apply a flicker effect based on Perlin noise
            float flickerNoise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0);
            float flickerIntensity = Mathf.Lerp(minFlickerIntensity, maxFlickerIntensity, flickerNoise);

            // Calculate the base intensity using stamina and any brightness multipliers
            float baseIntensity = (playerMovement.currentStamina / playerMovement.maxStamina) * initialLightIntensity * increaseIntensity;

            // Apply the flicker effect on top of the base intensity
            playerLight.intensity = baseIntensity * flickerIntensity;

            // Update the secondary light (without flicker)
            secondaryLight.intensity = (playerMovement.currentStamina / playerMovement.maxStamina) * initialSecondaryLightIntensity * increaseIntensity;
        }
        else if (playerMovement.currentStamina <= 0)
        {
            // Turn off both lights when stamina is depleted
            playerLight.enabled = false;
            secondaryLight.enabled = false;
        }
    }

    void HandleStamina()
    {
        // If the lights are off, regenerate stamina, otherwise drain it
        if (isLightOn)
        {
            // Drain stamina
            if (isShiningBrighter)
            {
                playerMovement.currentStamina -= playerMovement.staminaDrainRate * 3f * Time.deltaTime; // Faster drain when brighter
            }
            else
            {
                playerMovement.currentStamina -= playerMovement.staminaDrainRate * Time.deltaTime; // Normal drain
            }

            // Prevent stamina from going below 0
            playerMovement.currentStamina = Mathf.Clamp(playerMovement.currentStamina, 0, playerMovement.maxStamina);
        }
        else
        {
            // Regenerate stamina when the light is off, even when the player is moving
            playerMovement.currentStamina += playerMovement.staminaRegenRate * Time.deltaTime;
            playerMovement.currentStamina = Mathf.Clamp(playerMovement.currentStamina, 0, playerMovement.maxStamina);
        }
    }
}
