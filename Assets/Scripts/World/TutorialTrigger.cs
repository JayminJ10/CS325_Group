using UnityEngine;
using TMPro;  // Required if using TextMeshPro, else use `using UnityEngine.UI;` for standard Text

public class TutorialTrigger : MonoBehaviour
{
    public TextMeshProUGUI messageText;  // Drag your UI Text here in the Inspector
    public string messageToDisplay;      // The message to show for this zone

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            messageText.text = messageToDisplay;  // Set the message text
            messageText.gameObject.SetActive(true);  // Display the message
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            messageText.gameObject.SetActive(false);  // Hide the message when the player leaves
        }
    }
}
