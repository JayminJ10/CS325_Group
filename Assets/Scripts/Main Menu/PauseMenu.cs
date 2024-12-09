using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void ContinueGame()
    {
        Time.timeScale = 1.0f;
        GameObject pauseMenu = GameObject.Find("PauseMenu");
        GameObject settingsPanel = GameObject.Find("SettingsPanel");
        if (settingsPanel != null )
        {
            settingsPanel.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quit to main menu");
        SceneManager.LoadScene("MainMenu");
    }
}
