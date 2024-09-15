using UnityEngine;
using UnityEngine.UI;

public class StaminaFireUI : MonoBehaviour
{
    public PlayerMovement playerMovement; // Reference to the player's movement script
    private Image fireImage;              // Reference to the Image component

    void Start()
    {
        fireImage = GetComponent<Image>();

        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement reference is not set in StaminaFireUI script.");
        }
    }

    void Update()
    {
        if (playerMovement != null)
        {
            // Calculate the fill amount based on stamina
            float fillAmount = playerMovement.currentStamina / playerMovement.maxStamina;
            fireImage.fillAmount = fillAmount;
        }
    }
}
