using UnityEngine;

/// <summary>
/// Subtle UI screen shake for Canvas-based games.
/// Attach to a parent RectTransform (e.g., MapPanel) and call Shake(intensity).
/// Intensity is 0..1; effect decays automatically.
/// </summary>
public class ScreenShakeUI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private RectTransform target; // e.g., MapPanel
    private Vector2 basePos;

    [Header("Shake")]
    [Tooltip("Maximum pixel offset at intensity=1 (before trauma curve).")]
    [SerializeField] private float maxOffset = 6f;   // small: 4–8 px
    [Tooltip("Maximum rotation (degrees) at intensity=1 (optional flair).")]
    [SerializeField] private float maxAngle = 1.2f;  // tiny tilt
    [Tooltip("How quickly shake decays per second.")]
    [SerializeField] private float decayPerSecond = 1.5f; // higher = faster stop
    [Tooltip("Perlin noise frequency (movement speed).")]
    [SerializeField] private float frequency = 28f;

// Add this new field near the other [Header("Shake")] values:
    [SerializeField, Range(0.1f, 5f)] private float globalMultiplier = 1.0f;


    [Header("Response Curve")]
    [Tooltip("Exponent for trauma curve (2 = quadratic, 3 = cubic). Higher = softer at low intensities.")]
    [SerializeField] private float traumaExponent = 2.5f;

    private float trauma;      // 0..1
    private float tNoiseX;     // time seeds
    private float tNoiseY;
    private float tNoiseR;

    void Awake()
    {
        if (!target) target = GetComponent<RectTransform>();
        if (!target) Debug.LogWarning("[ScreenShakeUI] No RectTransform target assigned.");
        if (target) basePos = target.anchoredPosition;

        // randomize noise phase so multiple shakers don't sync
        tNoiseX = Random.value * 10f;
        tNoiseY = Random.value * 10f;
        tNoiseR = Random.value * 10f;
    }

// Add this after Awake()
void Start()
{
    // In case layout ran after Awake
    if (target) basePos = target.anchoredPosition;
}

    void OnDisable()
    {
        // reset to base pose
        if (target)
        {
            target.anchoredPosition = basePos;
            target.localEulerAngles = Vector3.zero;
        }
        trauma = 0f;
    }

    void Update()
    {
        if (target == null) return;

        // decay
        if (trauma > 0f)
        {
            trauma = Mathf.Max(0f, trauma - decayPerSecond * Time.deltaTime);
            ApplyShake();
        }
        else
        {
            // ensure perfectly reset
            target.anchoredPosition = basePos;
            target.localEulerAngles = Vector3.zero;
        }
    }

    private void ApplyShake()
    {
        // shape the trauma (quadratic/cubic feel)
        float power = Mathf.Pow(Mathf.Clamp01(trauma), traumaExponent);

        // progress noise
        tNoiseX += frequency * Time.deltaTime;
        tNoiseY += frequency * Time.deltaTime;
        tNoiseR += frequency * Time.deltaTime;

        // centered Perlin (-1..+1)
        float nx = (Mathf.PerlinNoise(tNoiseX, 0f) * 2f - 1f);
        float ny = (Mathf.PerlinNoise(0f, tNoiseY) * 2f - 1f);
        float nr = (Mathf.PerlinNoise(tNoiseR, tNoiseR) * 2f - 1f);

        // offsets & tiny rotation
        Vector2 offset = new Vector2(nx, ny) * (maxOffset * power) * globalMultiplier;    
        float  angle  = nr * (maxAngle * power) * globalMultiplier;

        target.anchoredPosition = basePos + offset;
        target.localEulerAngles = new Vector3(0f, 0f, angle);
    }

    /// <summary> Add shake. Intensity is typically small (e.g., 0.1–0.4).</summary>
    public void Shake(float intensity = 0.2f)
    {
        trauma = Mathf.Clamp01(trauma + Mathf.Abs(intensity));
    }

    /// <summary> Optionally reset base position if layout changes at runtime. </summary>
    public void ReAnchorNow()
    {
        if (target) basePos = target.anchoredPosition;
    }
}
