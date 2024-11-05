using UnityEngine;

public class RegenPickup : MonoBehaviour
{
    public float regenIncreaseAmount = 5f;  // Amount of stamina to increase

    void OnTriggerEnter(Collider other)
    {
        // Check if the object colliding is the player
        if (other.CompareTag("Player"))
        {
            // Get the player's CandleMechanics component
            CandleMechanics candleMechanics = other.GetComponent<CandleMechanics>();

            if (candleMechanics != null)
            {
                // Increase the player's stamina
                candleMechanics.IncreaseRegen(regenIncreaseAmount);

                // Optionally, add a visual or sound effect here to indicate pickup

                // Destroy the pickup after it's collected
                Destroy(gameObject);
            }
        }
    }
}
