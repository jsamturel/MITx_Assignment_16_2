using UnityEngine;
using UnityEngine.UI;

public class SpillMeterUI : MonoBehaviour
{
    [SerializeField] private Slider spillSlider;
    [SerializeField] private float decayPerSecond = 0.1f; // recovery speed
    public float Spill01 { get; private set; } = 0f; // 0..1

    void Start()
    {
        if (spillSlider)
        {
            spillSlider.minValue = 0f;
            spillSlider.maxValue = 1f;
            spillSlider.value = Spill01;
        }
    }

    void Update()
    {
        Spill01 = Mathf.MoveTowards(Spill01, 0f, decayPerSecond * Time.unscaledDeltaTime);
        Sync();
    }

    public void AddSpill(float amount)
    {
        Spill01 = Mathf.Clamp01(Spill01 + Mathf.Abs(amount));
        Sync();
    }

    void Sync()
    {
        if (spillSlider) spillSlider.value = Spill01;
    }
}
