using UnityEngine;

[DisallowMultipleComponent]
public class ObstacleUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform rect;            // auto-filled if null
    [Tooltip("Optional: use a tighter RectTransform for collision instead of the visual one.")]
    [SerializeField] private RectTransform hitboxOverride;  // NEW

    [Header("Effects")]
    [SerializeField] private bool causeSpill = true;
    [SerializeField, Range(0f, 1f)] private float spillAmount = 0.15f;

    [SerializeField] private bool causeShake = true;
    [SerializeField, Range(0f, 1f)] private float shakeIntensity = 0.25f;

    [Header("Collision Tuning")]
    [Tooltip("Extra pixels added around this obstacle's rect for detection.")]
    [SerializeField] private float padding = 0f;

    [Tooltip("Seconds to wait before this obstacle can trigger again.")]
    [SerializeField] private float cooldown = 0.35f;

    [Tooltip("If true, this obstacle blocks the scooter and pushes it out.")]
    [SerializeField] private bool solidBarrier = false;     // NEW

    [Tooltip("If true, this triggers only once ever.")]
    [SerializeField] private bool triggerOnce = false;

    private float lastTriggerTime = -999f;
    private bool hasTriggeredOnce = false;

    public RectTransform Rect => rect ? rect : (rect = GetComponent<RectTransform>());
    public RectTransform Hitbox => hitboxOverride ? hitboxOverride : Rect;   // NEW

    public bool CanTrigger()
    {
        if (triggerOnce && hasTriggeredOnce) return false;
        return (Time.time - lastTriggerTime) >= cooldown;
    }

    public void MarkTriggered()
    {
        lastTriggerTime = Time.time;
        if (triggerOnce) hasTriggeredOnce = true;
    }

    public bool CausesSpill => causeSpill;
    public float SpillAmount => spillAmount;

    public bool CausesShake => causeShake;
    public float ShakeIntensity => shakeIntensity;

    public float Padding => padding;
    public bool IsSolid => solidBarrier;   // NEW

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!Hitbox) return;
        var r = Hitbox.rect;
        var center = Hitbox.anchoredPosition;
        var size = r.size + Vector2.one * (padding * 2f);
        var p0 = center - size * 0.5f;
        var p1 = center + size * 0.5f;
        Gizmos.color = new Color(1f, 0.4f, 0.1f, 0.6f);
        Vector3 a = new Vector3(p0.x, p0.y, 0f);
        Vector3 b = new Vector3(p1.x, p0.y, 0f);
        Vector3 c = new Vector3(p1.x, p1.y, 0f);
        Vector3 d = new Vector3(p0.x, p1.y, 0f);
        Gizmos.DrawLine(a,b); Gizmos.DrawLine(b,c); Gizmos.DrawLine(c,d); Gizmos.DrawLine(d,a);
    }
#endif
}
