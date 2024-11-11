using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private TitleCardManager titleCardManager;  // Reference to TitleCardManager
    private bool alive;                         // Player life status
    public bool safe;                           // If player can be detected/attacked by enemy
    public float maxStamina = 100f;             // Maximum stamina
    public float currentStamina;                // Current stamina
    public float staminaRegenRate = 5f;         // Rate of stamina regeneration
    public bool isNearLitCandle = false;        // Condition for stamina regen near lit candles

    void Start()
    {
        alive = true;
        safe = false;
        currentStamina = maxStamina;            // Initialize stamina to maximum at start
    }

    void Update()
    {
        if (!alive)
        {
            titleCardManager.ShowDeathCard();
        }

        // Regenerate stamina only if near a lit candle and stamina is not full
        if (isNearLitCandle && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }

        // Check if stamina depletes entirely, causing player death
        if (currentStamina <= 0 && alive)
        {
            alive = false;
            titleCardManager.ShowDeathCard();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LitCandle")) // Assumes lit candles have the tag "LitCandle"
        {
            isNearLitCandle = true;
        }
        else if (other.CompareTag("Safe Zone"))
        {
            safe = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LitCandle"))
        {
            isNearLitCandle = false;
        }
        else if (other.CompareTag("Safe Zone"))
        {
            safe = false;
        }
    }

    public void IsHit()
    {
        if (!safe)
        {
            alive = false;
        }
    }

    // Method to reduce stamina (can be called from other scripts when player performs actions)
    public void ReduceStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }
}
