using UnityEngine;
using UnityEngine.SceneManagement;

public class CandleToNextLevel : MonoBehaviour
{
    public CandleManager candleManager;        // Reference to the CandleManager to track progress
    public PlayerStats playerStats;            // Reference to PlayerStats for stamina handling
    public Light candleLight;                  // Light component of the candle
    public ParticleSystem flameEffect;         // Particle System for the flame effect
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
        float distanceToPlayer = Vector3.Distance(transform.position, playerStats.transform.position);

        if (distanceToPlayer <= interactionRange && Input.GetKeyDown(KeyCode.E) && !isCandleLit && playerStats.currentStamina >= staminaCost)
        {
            LightCandle();
        }

        // Update player’s near lit candle status if candle is lit and within range
        playerStats.isNearLitCandle = isCandleLit && distanceToPlayer <= interactionRange;
    }

    // Method to light the candle and consume stamina
    void LightCandle()
    {
        // Check if player has enough stamina to light the candle
        if (playerStats.currentStamina >= staminaCost)
        {
            // Enable the candle light and particle effect
            if (candleLight != null)
            {
                candleLight.enabled = true;
            }
            if (flameEffect != null)
            {
                flameEffect.Play();
            }

            isCandleLit = true;

            // Deduct stamina through PlayerStats
            playerStats.ReduceStamina(staminaCost);

            // Set this candle's position as the new spawn point
            playerStats.transform.position = transform.position;

            // Change the tag to "LitCandle" to allow PlayerStats to detect it for stamina regeneration
            gameObject.tag = "LitCandle";

            // Set isNearLitCandle to true immediately so stamina can start regenerating without re-entering
            playerStats.isNearLitCandle = true;

            // Notify the CandleManager that this candle has been lit
            if (candleManager != null)
            {
                candleManager.LightCandle();
            }
        }
    }
}
