using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel; // Assign the PauseMenu panel in Inspector
    private bool isPaused;

    void Start()
    {
        // Ensure menu starts hidden and gameplay runs
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
    }

    // Wire this from HUD Pause AND from PauseMenu Resume buttons
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        if (pauseMenuPanel) pauseMenuPanel.SetActive(isPaused);
    }

    // Wire this from PauseMenu Quit button
    public void QuitToDesktop()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
