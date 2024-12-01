using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private TitleCardManager titleCardManager;  // Reference to TitleCardManager
    private bool alive = true;                  // Player life status
    public bool safe;                           // If player can be detected/attacked by enemy
    public float maxStamina = 100f;             // Maximum stamina
    public float currentStamina;                // Current stamina
    public float staminaRegenRate = 5f;         // Rate of stamina regeneration
    public float proximityRange = 5f;          // Range to check proximity to lit candles
    public bool isNearLitCandle = false;  

    private List<Transform> litCandles = new List<Transform>(); // List of lit candles


    void Start()
    {
        safe = false;
        currentStamina = maxStamina;            // Initialize stamina to maximum at start
    }

    void Update()
    {
        if (!alive)
        {
            titleCardManager.ShowDeathCard();
            return;
        }

        // Dynamically check proximity to lit candles
        isNearLitCandle = false;
        foreach (Transform candle in litCandles)
        {
            if (Vector3.Distance(transform.position, candle.position) <= proximityRange)
            {
                isNearLitCandle = true;
                break;
            }
        }

        // Regenerate stamina if near a lit candle
        if (isNearLitCandle && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }

        // Trigger death if stamina depletes completely
        if (currentStamina <= 0 && alive)
        {
            Die();
        }
    }

   // Register a lit candle in the list
    public void RegisterLitCandle(Transform candleTransform)
    {
        if (!litCandles.Contains(candleTransform))
        {
            litCandles.Add(candleTransform);
        }
    }

    // Unregister a lit candle (optional for advanced use)
    public void UnregisterLitCandle(Transform candleTransform)
    {
        if (litCandles.Contains(candleTransform))
        {
            litCandles.Remove(candleTransform);
        }
    }

    public void IsHit(string enemyType, int damage)
    {
        if (string.Equals(enemyType, "Spider") && alive) {
            ReduceStamina(damage);
            return;
        }
        if (!safe && alive)
        {
            Debug.Log("Player has been hit by enemy.");
            Die();
        }
    }

    private void Die()
    {
        alive = false;
        titleCardManager.ShowDeathCard();
        Debug.Log("Player has died.");
    }

    // Method to reduce stamina (can be called from other scripts when player performs actions)
    public void ReduceStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        if (currentStamina <= 0 && alive)
        {
            Die();
        }
    }
}
