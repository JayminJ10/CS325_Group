using UnityEngine;
using UnityEngine.SceneManagement;

public class CandleToNextLevel : MonoBehaviour
{
    public CandleManager candleManager;        // Reference to the CandleManager to track progress
    public PlayerMovement playerMovement;      // Reference to the PlayerMovement script for stamina handling
    public Light candleLight;                  // Reference to the Light component of the candle
    public ParticleSystem flameEffect;         // Reference to the Particle System for the flame effect
    public float staminaCost = 10f;            // Stamina cost to light the candle
    public float interactionRange = 2f;        // Distance within which the player can interact with the candle

    private bool isCandleLit = false;          // Track if the candle is already lit

    void Start()
    {
        // Ensure the candle light and flame effect are off at the start
        if (candleLight != null)
        {
            candleLight.enabled = false;
        }

        if (flameEffect != null)
        {
            flameEffect.Stop(); // Ensure the particle system is not playing at the start
        }
    }

    void Update()
    {
        // Check if the player is close enough and presses "E", and the candle is not yet lit
        float distanceToPlayer = Vector3.Distance(transform.position, playerMovement.transform.position);

        if (distanceToPlayer <= interactionRange && Input.GetKeyDown(KeyCode.E) && !isCandleLit && playerMovement.currentStamina >= staminaCost)
        {
            LightCandle();
        }
    }

    // Method to light the candle and consume stamina
    void LightCandle()
    {
        if (playerMovement.currentStamina >= staminaCost)
        {
            // Light the candle
            if (candleLight != null)
            {
                candleLight.enabled = true;
            }

            if (flameEffect != null)
            {
                flameEffect.Play(); // Start the flame particle effect
            }

            isCandleLit = true;

            // Deduct stamina
            playerMovement.currentStamina -= staminaCost;

            // Notify the CandleManager that this candle has been lit
            if (candleManager != null)
            {
                candleManager.LightCandle();
            }
        }
    }
}
