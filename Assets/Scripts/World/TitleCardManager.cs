using UnityEngine;
using TMPro;  // For TextMeshPro
using UnityEngine.UI; // For UI elements
using System.Collections;

public class TitleCardManager : MonoBehaviour
{
    public GameObject titleCardCanvas;   // Reference to the Title Card Canvas (UI)
    public TextMeshProUGUI titleText;    // Reference to the TextMeshPro title text
    public UnityEngine.UI.Image blackBackground; // Reference to the black background (Unity UI Image)
    public float displayTime = 3f;       // Time to display the title card
    public float fadeDuration = 1f;      // Duration of fade in and fade out

    public GameObject playerHUD;         // Reference to the Player HUD (e.g., WASD tutorial and Stamina bar)

    private Color candlelightGold = new Color(1f, 0.843f, 0f); // Candlelight gold color (RGB 255, 215, 0)

    void Start()
    {
        // Hide the player HUD during the title screen
        if (playerHUD != null)
        {
            playerHUD.SetActive(false);  // Disable the entire HUD during title card
        }

        // Set the text to be initially invisible (alpha = 0)
        titleText.color = new Color(candlelightGold.r, candlelightGold.g, candlelightGold.b, 0f);
        blackBackground.color = new Color(0, 0, 0, 1);  // Full black background

        // Start the title card sequence
        StartCoroutine(ShowTitleCard());
    }

    IEnumerator ShowTitleCard()
    {
        // Freeze the game
        Time.timeScale = 0f;
        AudioListener.pause = true; // Pause all game audio

        // Ensure the title card is active
        titleCardCanvas.SetActive(true);

        // Fade in the title text like a candlelight
        yield return StartCoroutine(FadeInText());

        // Wait for the display time
        yield return new WaitForSecondsRealtime(displayTime);  // Use unscaled time

        // Fade out the title card
        yield return StartCoroutine(FadeOutText());

        // Deactivate the title card once faded out
        titleCardCanvas.SetActive(false);

        // Reactivate the player HUD after the title screen disappears
        if (playerHUD != null)
        {
            playerHUD.SetActive(true);  // Enable the HUD
        }

        // Resume the game
        Time.timeScale = 1f;
        AudioListener.pause = false;  // Resume all game audio
    }

    // Fade-in effect for the text (like a candlelight)
    IEnumerator FadeInText()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);

            // Fade in the text and keep the background black
            titleText.color = new Color(candlelightGold.r, candlelightGold.g, candlelightGold.b, alpha);
            yield return null;
        }
    }

    // Fade-out effect for the text
    IEnumerator FadeOutText()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            // Fade out the text and the black background
            titleText.color = new Color(candlelightGold.r, candlelightGold.g, candlelightGold.b, alpha);
            blackBackground.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}
