using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeFinishedController : MonoBehaviour
{
    [SerializeField] private GameObject timeFinishedPanel; // assign TimeFinishedPanel
    [SerializeField] private GameObject pauseMenuPanel;    // optional: to auto-hide if open

    private bool isFinished = false;

    void Start()
    {
        if (timeFinishedPanel) timeFinishedPanel.SetActive(false);
    }

    // Hook this to the timer's onTimerFinished event
    public void ShowTimeFinished()
    {
        if (isFinished) return;
        isFinished = true;

        // Hide pause menu if it's open, so overlays don't stack
        if (pauseMenuPanel && pauseMenuPanel.activeSelf)
            pauseMenuPanel.SetActive(false);

        if (timeFinishedPanel) timeFinishedPanel.SetActive(true);

        // Stop gameplay
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        // Resume time before reloading
        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void QuitToDesktop()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
