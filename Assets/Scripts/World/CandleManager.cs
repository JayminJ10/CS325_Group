using UnityEngine;
using TMPro;
using System.Collections;

public class CandleManager : MonoBehaviour
{
    public int totalCandles;  // Total number of candles in the level
    private int candlesLit = 0;  // Number of candles lit by the player

    public TextMeshProUGUI candlesLitText;  // Reference to the UI text to display progress
    public GameObject candleCounterUI;      // The UI that shows the number of candles lit
    public TitleCardManager titleCardManager;  // Reference to the TitleCardManager script

    public float litTextDisplayTime = 3f;   // Time to display the "Candles Lit" text

    void Start()
    {
        // Initialize the candle count UI
        UpdateCandleUI();
        candleCounterUI.SetActive(false);
    }

    // This method should be called whenever a candle is lit
    public void LightCandle()
    {
        candlesLit++;
        UpdateCandleUI();

        // Check if all candles are lit
        if (candlesLit >= totalCandles)
        {
            // Wait for a few seconds and then trigger the end card through the TitleCardManager
            StartCoroutine(TriggerEndCard());
        }
    }

    // Update the candle count UI
    private void UpdateCandleUI()
    {
        candleCounterUI.SetActive(true);  // Show the UI
        candlesLitText.text = $"Candles Lit: {candlesLit}/{totalCandles}";
        StartCoroutine(HideCandleCounterAfterDelay());
    }

    // Hide the candle count UI after a few seconds
    private IEnumerator HideCandleCounterAfterDelay()
    {
        yield return new WaitForSeconds(litTextDisplayTime);
        candleCounterUI.SetActive(false);
    }

    // Coroutine to trigger the end card after all candles are lit
    private IEnumerator TriggerEndCard()
    {
        // Wait for the candle lit text to disappear
        yield return new WaitForSeconds(litTextDisplayTime);

        // Call the TitleCardManager to show the end card and transition to the next level
        if (titleCardManager != null)
        {
            titleCardManager.ShowEndCard();  // Trigger the end card in the TitleCardManager
        }
    }
}
