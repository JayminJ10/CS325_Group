using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Loads the next scene (your main game scene)
        SceneManager.LoadScene("TutorialScene"); // Make sure the scene name matches your actual game scene name
    }

    // Function to quit the game
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();  // Quit the game (won't work in the editor)
    }
}
