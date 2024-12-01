using UnityEngine;

public class CandleInteraction : MonoBehaviour
{
    public PlayerStats playerStats;            // Reference to PlayerStats for stamina management
    public Light candleLight;                  // Reference to the candle's Light component
    public float candleLightStaminaCost = 10f; // Stamina cost to light the candle
    private bool isCandleLit = false;          // Track if the candle is already lit

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
        // Check if player has enough stamina to light the candle
        if (playerStats.currentStamina >= candleLightStaminaCost)
        {
            if (candleLight != null)
            {
                candleLight.enabled = true;
            }
            isCandleLit = true;

            // Consume stamina via PlayerStats
            playerStats.ReduceStamina(candleLightStaminaCost);

            // Change the tag to "LitCandle" to allow PlayerStats to detect it
            gameObject.tag = "LitCandle";

            Debug.Log("Candle lit, spawn point set to candle position: " + transform.position);
        }
    }

    public bool IsLit { get { return isCandleLit; } }

}
