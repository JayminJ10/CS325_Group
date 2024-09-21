using UnityEngine;

public class CandleInteraction : MonoBehaviour
{
    public PlayerMovement playerMovement;  // Reference to the PlayerMovement script to set the spawn point
    public Light candleLight;              // Reference to the candle's Light component
    public float candleLightStaminaCost = 10f; // Stamina cost to light the candle
    private bool isCandleLit = false;      // Track if the candle is already lit

    void Start()
    {
        // Ensure the candle light is off at the start
        if (candleLight != null)
        {
            candleLight.enabled = false;
        }
    }

    void Update()
    {
        // Check if the player is within range and presses "E" to light the candle
        if (Input.GetKeyDown(KeyCode.E) && !isCandleLit)
        {
            LightCandle();
        }
    }

    // Method to light the candle and set it as the new spawn point
    void LightCandle()
    {
        if (candleLight != null && playerMovement.currentStamina >= candleLightStaminaCost)
        {
            // Light the candle
            candleLight.enabled = true;
            isCandleLit = true;

            // Consume stamina
            playerMovement.currentStamina -= candleLightStaminaCost;

            // Set this candle's position as the new spawn point
            playerMovement.SetSpawnPoint(transform.position);

            Debug.Log("Candle lit, spawn point set to candle position: " + transform.position);
        }
    }
}
