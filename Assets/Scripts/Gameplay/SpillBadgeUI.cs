using UnityEngine;

/// <summary>
/// Spill badge that stays next to the scooter in UI space.
/// Works whether the badge is a CHILD of PlayerScooter (local mode)
/// or a SIBLING under MapPanel (absolute mode). Also supports subtle pulse.
/// </summary>
public class SpillBadgeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform badgeRect;   // The badge (this object)
    [SerializeField] private RectTransform playerRect;  // PlayerScooter RectTransform

    [Header("Positioning")]
    [Tooltip("Offset from player. If badge is a CHILD of the player, this is LOCAL offset.\nIf it's a SIBLING, this is ABSOLUTE (MapPanel) offset.")]
    [SerializeField] private Vector2 offset = new Vector2(-20f, 0f);

    [Header("Pulse & Scale")]
    [SerializeField, Range(0.5f, 1.5f)] private float baseScale = 1f;
    [SerializeField, Range(0f, 0.5f)]  private float pulseAmplitude = 0.05f;
    [Tooltip("If true, keeps the visual gap constant even when scaling (for sibling/absolute mode).")]
    [SerializeField] private bool keepOffsetConstant = true;

    [Header("Drive Source")]
    [SerializeField] private bool driveFromSpeed = true;
    [SerializeField, Range(0f,1f)] private float spill01 = 0f;   // used when driveFromSpeed = false

    [Header("Smoothing (optional)")]
    [SerializeField, Min(0f)] private float followSmoothing = 0f; // 0 = snap

    private PlayerController2D player;
    private Vector2 currentPos;     // used only in absolute/sibling mode
    private bool isChildMode = false;

    void Awake()
    {
        if (!badgeRect)   badgeRect   = GetComponent<RectTransform>();
        if (!playerRect)  Debug.LogWarning("[SpillBadgeUI] Assign Player Rect.");
        player = playerRect ? playerRect.GetComponentInParent<PlayerController2D>() : null;

        // Auto-detect: child mode if badge is a direct child of player
        isChildMode = (badgeRect && playerRect && badgeRect.parent == playerRect);
    }

    void OnEnable()
    {
        // Snap to correct initial position so it doesn't start off-screen
        SnapToTarget(immediate:true);
    }

    void LateUpdate()
    {
        UpdatePulseAndFollow();
    }

    private void UpdatePulseAndFollow()
    {
        if (!badgeRect || !playerRect) return;

        // 1) Pulse factor t
        float t = spill01;
        if (driveFromSpeed && player != null)
            t = Mathf.Clamp01(player.CurrentSpeed01 * 0.8f);

        // 2) Compute scale
        float minS = baseScale - pulseAmplitude;
        float maxS = baseScale + pulseAmplitude;
        float s = Mathf.Lerp(minS, maxS, t);

        // 3) Position update
        if (isChildMode)
        {
            // CHILD MODE: badge local to player → just use local offset
            // (No need to add player position; that caused the off-screen issue)
            badgeRect.anchoredPosition = offset; // local offset only
        }
        else
        {
            // SIBLING/ABSOLUTE MODE: badge under MapPanel (same parent as player)
            // Keep the visible gap constant even when scaling if requested
            Vector2 appliedOffset = (keepOffsetConstant && s != 0f) ? (offset / s) : offset;
            Vector2 targetPos = playerRect.anchoredPosition + appliedOffset;

            if (followSmoothing > 0f)
            {
                // Exponential smoothing toward target
                currentPos = Vector2.Lerp(currentPos, targetPos, 1f - Mathf.Exp(-followSmoothing * Time.unscaledDeltaTime));
                badgeRect.anchoredPosition = currentPos;
            }
            else
            {
                badgeRect.anchoredPosition = targetPos;
                currentPos = targetPos; // keep in sync
            }
        }

        // 4) Apply scale
        badgeRect.localScale = Vector3.one * s;
    }

    private void SnapToTarget(bool immediate)
    {
        if (!badgeRect || !playerRect) return;

        // Compute current target once and set position immediately (no first-frame drift)
        float s = baseScale;
        Vector2 appliedOffset = offset;

        if (!isChildMode)
        {
            if (keepOffsetConstant && s != 0f) appliedOffset = offset / s;
            Vector2 targetPos = playerRect.anchoredPosition + appliedOffset;
            badgeRect.anchoredPosition = targetPos;
            currentPos = targetPos;
        }
        else
        {
            badgeRect.anchoredPosition = offset; // local
        }

        badgeRect.localScale = Vector3.one * s;
    }

    // Public API
    public void SetSpill01(float v) => spill01 = Mathf.Clamp01(v);
    public void Flash(float amount = 0.2f) => spill01 = Mathf.Clamp01(spill01 + Mathf.Abs(amount));
    public void SetPulseAmplitude(float amplitude) => pulseAmplitude = Mathf.Clamp(amplitude, 0f, 0.5f);
    public void SetBaseScale(float scale) => baseScale = Mathf.Clamp(scale, 0.5f, 1.5f);
}
