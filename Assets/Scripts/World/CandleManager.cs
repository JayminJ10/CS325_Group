using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CandleManager : MonoBehaviour
{
    public List<CandleToNextLevel> candles;  // List of all candles in the level
    public TextMeshProUGUI candlesLitText;  // Reference to the UI text to display progress
    public GameObject candleCounterUI;      // The UI that shows the number of candles lit
    public TitleCardManager titleCardManager;  // Reference to the TitleCardManager script
    public EnemyCandleExtinguisher enemy;   // Reference to the enemy script
    public float nextLevelDelay = 3f;       // Time delay before transitioning to the next level
    public string nextLevelSceneName;       // Name of the next level scene

    public float litTextDisplayTime = 3f;   // Time to display the "Candles Lit" text

    void Start()
    {
        // Initialize the candle count UI
        UpdateCandleUI();
        candleCounterUI.SetActive(false);
    }

    public void UpdateCandleUI()
    {
        int currentlyLit = 0;
        foreach (CandleToNextLevel candle in candles)
        {
            if (candle.IsLit)
            {
                currentlyLit++;
            }
        }

        candleCounterUI.SetActive(true);  // Show the UI
        candlesLitText.text = $"Candles Lit: {currentlyLit}/{candles.Count}";
        StartCoroutine(HideCandleCounterAfterDelay());

        // If all candles are lit, trigger level completion
        if (currentlyLit == candles.Count)
        {
            StopEnemy();
            StartCoroutine(TransitionToNextLevel());
        }
    }

    private void StopEnemy()
    {
        if (enemy != null)
        {
            enemy.enabled = false; // Disable the enemy's behavior script
            Debug.Log("Enemy has been stopped!");
        }
    }

    private IEnumerator HideCandleCounterAfterDelay()
    {
        yield return new WaitForSeconds(litTextDisplayTime);
        candleCounterUI.SetActive(false);
    }

    private IEnumerator TransitionToNextLevel()
    {
        yield return new WaitForSeconds(nextLevelDelay);

        if (titleCardManager != null)
        {
            titleCardManager.ShowEndCard();  // Display the next level card
        }

        yield return new WaitForSeconds(2f);  // Optional extra delay for the end card

        // Load the next level scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelSceneName);
    }
}
