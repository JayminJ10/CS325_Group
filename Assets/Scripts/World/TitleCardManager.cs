using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;  // For transitioning to the next level

public class TitleCardManager : MonoBehaviour
{
    // References for the Title Card
    public GameObject titleCardCanvas;   // Reference to the Title Card Canvas (UI)
    public TextMeshProUGUI titleText;    // Reference to the TextMeshPro title text

    //References for Death card
    public GameObject deathCardCanvas;   //Reference to the Death Card Canvas (UI)
    public TextMeshProUGUI deathText;    //Reference to the Death Card text

    // References for the End Card
    public GameObject endCardCanvas;     // Reference to the End Card Canvas (UI)
    public TextMeshProUGUI endText;      // Reference to the End Card text

    public UnityEngine.UI.Image blackBackground; // Reference to the black background (Unity UI Image)
    public float displayTime = 3f;       // Time to display the title or end card
    public float fadeDuration = .5f;      // Duration of fade in and fade out

    public GameObject playerHUD;         // Reference to the Player HUD (e.g., WASD tutorial and Stamina bar)

    private Color candlelightGold = new Color(1f, 0.843f, 0f); // Candlelight gold color (RGB 255, 215, 0)

    void Start()
    {
        // Ensure the player HUD is hidden during the title screen
        if (playerHUD != null)
        {
            playerHUD.SetActive(false);  // Disable the entire HUD during the title card
        }

        // Set the title card and end card and death card inactive at the start
        if (titleCardCanvas != null)
        {
            titleCardCanvas.SetActive(true);  // Ensure title card is shown on start
        }
        if (endCardCanvas != null)
        {
            endCardCanvas.SetActive(false);  // Ensure the end card is hidden on start
        }
        if (deathCardCanvas != null)
        {
            deathCardCanvas.SetActive(false);
        }

        // Start the title card sequence when the scene starts
        StartCoroutine(ShowTitleCard());
    }

    // Public method to show the end card, called when all candles are lit
    public void ShowEndCard()
    {
        if (endCardCanvas != null)
        {
            endCardCanvas.SetActive(true);  // Activate the end card
            StartCoroutine(ShowEndCardSequence());
        }
    }

    // Public method to show the death card, called when player is hit
    public void ShowDeathCard()
    {
        if (deathCardCanvas != null)
        {
            deathCardCanvas.SetActive(true);  // Activate the end card
            StartCoroutine(ShowDeathCardSequence());
        }
    }

    // Coroutine to handle the title card sequence
    IEnumerator ShowTitleCard()
    {
        // Freeze the game and audio during the title card
        Time.timeScale = 0f;
        AudioListener.pause = true; // Pause all game audio

        // Wait for the display time
        yield return new WaitForSecondsRealtime(displayTime);

        // Fade out the title card
        yield return StartCoroutine(FadeOutText(titleText));

        // Hide the title card once the fade-out is complete
        titleCardCanvas.SetActive(false);

        // Reactivate the player HUD after the title card disappears
        if (playerHUD != null)
        {
            playerHUD.SetActive(true);  // Enable the HUD
        }

        // Resume the game and audio
        Time.timeScale = 1f;
        AudioListener.pause = false;  // Resume all game audio
    }

    IEnumerator ShowDeathCardSequence()
    {
        // Freeze the game and audio during the title card
        Time.timeScale = 0f;
        AudioListener.pause = true; // Pause all game audio

        // Fade in the death card
        yield return StartCoroutine(FadeInText(deathText));

        // Wait for the display time
        yield return new WaitForSecondsRealtime(displayTime);

        // Fade in the death card
        yield return StartCoroutine(FadeOutText(deathText));

        //Reload the current level
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        //Hide death card once the fade-out is complete
        deathCardCanvas.SetActive(false);

        // Resume the game and audio
        Time.timeScale = 1f;
        AudioListener.pause = false;  // Resume all game audio
    }

    // Coroutine to handle the end card sequence and transition to the next level
    IEnumerator ShowEndCardSequence()
    {
        // Freeze the game and audio during the end card
        Time.timeScale = 0f;
        AudioListener.pause = true; // Pause all game audio


        // Fade in the end card text
        yield return StartCoroutine(FadeInText(endText));

        // Wait for the display time
        yield return new WaitForSecondsRealtime(displayTime);

        // Fade out the end card text
        yield return StartCoroutine(FadeOutText(endText));

        // Transition to the next level
        LoadNextLevel();

        // Hide the end card once the fade-out is complete
        endCardCanvas.SetActive(false);

        

        // Resume the game (just before the scene transition)
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    // Fade-in effect for the text
    IEnumerator FadeInText(TextMeshProUGUI text)
    {
        float elapsedTime = 0f;
        Color originalColor = text.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            blackBackground.color = new Color(0, 0, 0, alpha);  // Fade in the black background too
            yield return null;
        }
    }

    // Fade-out effect for the text
    IEnumerator FadeOutText(TextMeshProUGUI text)
    {
        float elapsedTime = 0f;
        Color originalColor = text.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            blackBackground.color = new Color(0, 0, 0, alpha);  // Fade out the black background too
            yield return null;
        }
    }

    // Load the next level
    private void LoadNextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            Debug.Log("No more levels to load. End of game.");
        }
    }
}
