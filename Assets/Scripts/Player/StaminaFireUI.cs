using UnityEngine;
using UnityEngine.UI;

public class StaminaFireUI : MonoBehaviour
{
    public PlayerStats playerStats;       // Reference to PlayerStats for stamina information
    private Image fireImage;              // Reference to the Image component

    void Start()
    {
        fireImage = GetComponent<Image>();

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats reference is not set in StaminaFireUI script.");
        }
    }

    void Update()
    {
        if (playerStats != null)
        {
            // Calculate the fill amount based on stamina
            float fillAmount = playerStats.currentStamina / playerStats.maxStamina;
            fireImage.fillAmount = fillAmount;
        }
    }
}
