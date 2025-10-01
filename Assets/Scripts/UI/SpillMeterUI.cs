using UnityEngine;
using UnityEngine.UI;

public class SpillMeterUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider spillSlider;   // vertical slider (0..1), Top->Bottom
    [SerializeField] private float maxSpill = 1f;

    [Header("Shake hook")]
    [SerializeField] private ScreenShakeUI shaker; // drag MapPanel (with ScreenShakeUI) here
    [SerializeField] private float baseShake   = 0.12f;  // was 0.08
    [SerializeField] private float shakePerUnit= 3.0f;   // was 1.6


    private float current;

    void Awake()
    {
        if (spillSlider)
        {
            spillSlider.minValue = 0f;
            spillSlider.maxValue = maxSpill;
            spillSlider.value = 0f;
        }
    }

    public void AddSpill(float amount)
    {
        current = Mathf.Clamp(current + Mathf.Abs(amount), 0f, maxSpill);
        if (spillSlider) spillSlider.value = current;

        // ALWAYS ping shaker on spill (independent of anything else)
        if (shaker != null)
        {
            float intensity = Mathf.Clamp01(baseShake + Mathf.Abs(amount) * shakePerUnit);
            shaker.Shake(intensity);
        }
    }

    public void ResetSpill()
    {
        current = 0f;
        if (spillSlider) spillSlider.value = 0f;
    }

    // Optional helper for your test button
    public void DebugAddSpill(float amt) => AddSpill(amt);
}
