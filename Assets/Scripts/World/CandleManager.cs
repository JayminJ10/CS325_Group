using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CandleManager : MonoBehaviour
{
    public int totalCandles;  // Total number of candles in the level
    private int candlesLit = 0;  // Number of candles lit by the player

    public TextMeshProUGUI candlesLitText;  // Reference to the UI text to display progress
    public TextMeshProUGUI wellDoneText;  // Reference to the "Well Done" UI text
    public GameObject candleCounterUI;  // The UI that shows the number of candles lit

    public float messageDisplayTime = 3f;  // Time to display the candle count and well done message

    void Start()
    {
        // Initialize the candle count UI and hide the "Well Done" message
        UpdateCandleUI();
        wellDoneText.gameObject.SetActive(false);
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
            StartCoroutine(DisplayWellDoneMessage());
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
        yield return new WaitForSeconds(messageDisplayTime);
        candleCounterUI.SetActive(false);
    }

    // Display the "Well Done" message when all candles are lit
    private IEnumerator DisplayWellDoneMessage()
    {
        wellDoneText.gameObject.SetActive(true);  // Show the "Well Done" message
        yield return new WaitForSeconds(messageDisplayTime);
        wellDoneText.gameObject.SetActive(false);  // Hide the "Well Done" message
    }
}
