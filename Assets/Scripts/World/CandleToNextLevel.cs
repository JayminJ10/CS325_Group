using UnityEngine;
using UnityEngine.SceneManagement;

public class CandleToNextLevel : MonoBehaviour
{
    public PlayerMovement playerMovement;  // Reference to the PlayerMovement script for stamina handling
    public Light candleLight;              // Reference to the Light component of the candle
    public float staminaCost = 10f;        // Stamina cost to light the candle

    private bool isCandleLit = false;      // Track if the candle is already lit

    void Start()
    {
        // Ensure the candle is off at the start
        if (candleLight != null)
        {
            candleLight.enabled = false;
        }
    }

    void Update()
    {
        // Check if the player presses "E" and the candle is not yet lit
        if (Input.GetKeyDown(KeyCode.E) && !isCandleLit && playerMovement.currentStamina >= staminaCost)
        {
            LightCandle();
        }
    }

    // Method to light the candle and load the next level
    void LightCandle()
    {
        // Consume stamina if available
        if (playerMovement.currentStamina >= staminaCost)
        {
            // Light the candle
            candleLight.enabled = true;
            isCandleLit = true;

            // Deduct stamina cost
            playerMovement.currentStamina -= staminaCost;

            Debug.Log("Candle lit, loading next level...");

            // Load the next scene after a short delay
            Invoke("LoadNextLevel", 2f);  // Optional delay to let the candle light fully appear before transitioning
        }
    }

    // Method to load the next scene
    void LoadNextLevel()
    {
        // Check if there are more scenes to load
        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            // Load the next scene in the build index
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            Debug.Log("No more levels to load. End of game.");
        }
    }
}
