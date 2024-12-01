using UnityEngine;

public class CandleToNextLevel : MonoBehaviour
{
    public bool IsLit { get; private set; } = false; // Track if the candle is lit

    public CandleManager candleManager;        // Reference to the CandleManager to track progress
    public PlayerStats playerStats;            // Reference to PlayerStats for stamina handling
    public Light candleLight;                  // Light component of the candle
    public ParticleSystem flameEffect;         // Particle System for the flame effect
    public float staminaCost = 10f;            // Stamina cost to light the candle
    public float interactionRange = 2f;        // Distance within which the player can interact with the candle

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

        if (distanceToPlayer <= interactionRange && Input.GetKeyDown(KeyCode.E) && !IsLit && playerStats.currentStamina >= staminaCost)
        {
            LightCandle();
        }
    }

    public void LightCandle()
    {
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

            IsLit = true;

            // Deduct stamina through PlayerStats
            playerStats.ReduceStamina(staminaCost);

            // Notify the CandleManager
            if (candleManager != null)
            {
                candleManager.UpdateCandleUI();
            }

            // Register the lit candle with PlayerStats
            playerStats.RegisterLitCandle(this.transform);

            Debug.Log($"{gameObject.name} is now lit!");
        }
    }

    public void TurnOffCandle()
    {
        if (IsLit)
        {
            IsLit = false;

            // Disable the candle light and particle effect
            if (candleLight != null) candleLight.enabled = false;
            if (flameEffect != null) flameEffect.Stop();

            // Notify the CandleManager
            if (candleManager != null)
            {
                candleManager.UpdateCandleUI();
            }

            // Unregister the candle from PlayerStats
            playerStats.UnregisterLitCandle(this.transform);

            Debug.Log($"{gameObject.name} has been turned off!");
        }
    }
}
