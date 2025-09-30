using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

public void ToggleSettings()
{
    if (!settingsPanel) return;
    bool next = !settingsPanel.activeSelf;
    settingsPanel.SetActive(next);
    Time.timeScale = next ? 0f : 1f;  // <- pauses while settings is open
}

}
