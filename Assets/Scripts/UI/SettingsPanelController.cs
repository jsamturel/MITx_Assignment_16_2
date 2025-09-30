using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanelController : MonoBehaviour
{
    [Header("Optional UI")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private TMP_Text masterVolumeValue;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Text sfxVolumeValue;
    [SerializeField] private TMP_Dropdown graphicsDropdown;

    private void OnEnable()
    {
        // Initialize defaults
        if (masterVolumeSlider)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;
            masterVolumeSlider.value = AudioListener.volume;
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            OnMasterVolumeChanged(masterVolumeSlider.value);
        }
        if (sfxVolumeSlider)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.value = 1f;
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            OnSfxVolumeChanged(sfxVolumeSlider.value);
        }
        if (graphicsDropdown)
        {
            graphicsDropdown.ClearOptions();
            graphicsDropdown.AddOptions(new System.Collections.Generic.List<string> { "Low", "Medium", "High" });
            graphicsDropdown.value = Mathf.Clamp(QualitySettings.GetQualityLevel(), 0, 2);
            graphicsDropdown.onValueChanged.AddListener(OnGraphicsChanged);
        }
    }

    private void OnDisable()
    {
        if (masterVolumeSlider) masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        if (sfxVolumeSlider) sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
        if (graphicsDropdown) graphicsDropdown.onValueChanged.RemoveListener(OnGraphicsChanged);
    }

    private void OnMasterVolumeChanged(float v)
    {
        AudioListener.volume = v;
        if (masterVolumeValue) masterVolumeValue.text = Mathf.RoundToInt(v * 100f) + "%";
    }

    private void OnSfxVolumeChanged(float v)
    {
        if (sfxVolumeValue) sfxVolumeValue.text = Mathf.RoundToInt(v * 100f) + "%";
        // Hook your SFX mixer here if you add one later.
    }

    private void OnGraphicsChanged(int idx)
    {
        QualitySettings.SetQualityLevel(idx, true);
    }
}
