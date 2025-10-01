using System.Collections.Generic;
using UnityEngine;

public class CollisionManagerUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform mapPanel;
    [SerializeField] private RectTransform playerRect;
    [SerializeField] private PlayerController2D player;   // optional (for rebound tap)
    [SerializeField] private ScreenShakeUI shaker;        // optional
    [SerializeField] private SpillMeterUI spillMeter;      // drag the component here

    [Header("Obstacle Discovery")]
    [SerializeField] private bool autoFindObstacles = true;
    [SerializeField] private List<ObstacleUI> obstacles = new List<ObstacleUI>();

    [Header("Player Bounds Padding (px)")]
    [SerializeField] private float playerPadding = 2f;

    [Header("Solid Resolution")]
    [Tooltip("Extra pixels to push out so we don't get stuck on edges.")]
    [SerializeField] private float separationBias = 1.0f;
    [Tooltip("Tiny rebound strength applied to player's velocity (0..1 scale).")]
    [SerializeField, Range(0f, 1f)] private float reboundStrength = 0.15f;

    [SerializeField] private bool debugLogs = false;

    void Reset()
    {
        mapPanel = GetComponent<RectTransform>();
    }

    void Awake()
    {
        if (!mapPanel) mapPanel = GetComponent<RectTransform>();
        if (!player && playerRect) player = playerRect.GetComponentInParent<PlayerController2D>();
        if (!shaker) shaker = FindObjectOfType<ScreenShakeUI>();

        if (autoFindObstacles && mapPanel)
        {
            obstacles.Clear();
            foreach (var o in mapPanel.GetComponentsInChildren<ObstacleUI>(true))
                obstacles.Add(o);
        }
    }

    void Update()
    {
        if (!playerRect || obstacles == null || obstacles.Count == 0) return;

        Rect playerR = GetRectInParentSpace(playerRect, mapPanel, playerPadding);

        for (int i = 0; i < obstacles.Count; i++)
        {
            var o = obstacles[i];
            if (o == null || !o.enabled || !o.gameObject.activeInHierarchy) continue;
            if (!o.CanTrigger()) continue;

            // Use the hitbox override if present
            Rect obsR = GetRectInParentSpace(o.Hitbox, mapPanel, o.Padding);

            if (!Overlaps(playerR, obsR))
                continue;

            // 1) Resolve solid barriers (push player out a tiny bit)
            if (o.IsSolid)
            {
                Vector2 delta = ResolveAABBCollision(ref playerR, obsR, separationBias);
                // Apply the position correction to the playerRect
                playerRect.anchoredPosition += delta;

                // Optional: tiny rebound tap into PlayerController2D if available
                if (player != null)
                {
                    // Simple: nudge velocity opposite to penetration direction
                    Vector2 v = player.CurrentVelocity;
                    v += -delta.normalized * (v.magnitude * reboundStrength);
                    // We don't set directly (no setter). If you want stronger behavior,
                    // add a method on PlayerController2D to accept external nudges.
                }
            }

            // 2) Trigger effects (independent toggles)
            if (o.CausesShake && shaker != null)
                shaker.Shake(o.ShakeIntensity);

         if (o.CausesSpill && spillMeter != null)
            {
                spillMeter.AddSpill(o.SpillAmount);
            }

            if (debugLogs)
                Debug.Log($"[Collision] {o.name}  spill:{o.CausesSpill}  shake:{o.CausesShake}  solid:{o.IsSolid}");

            o.MarkTriggered();
        }
    }

    // --- RECT UTILS ---

    private static Rect GetRectInParentSpace(RectTransform rt, RectTransform parent, float padding)
    {
        Bounds b = RectTransformUtility.CalculateRelativeRectTransformBounds(parent, rt);
        Vector2 size = new Vector2(b.size.x, b.size.y);
        Vector2 half = size * 0.5f + Vector2.one * padding;
        Vector2 min = new Vector2(b.center.x, b.center.y) - half;
        Vector2 rectSize = half * 2f;
        return new Rect(min, rectSize);
    }

    private static bool Overlaps(Rect a, Rect b)
    {
        return !(a.xMin > b.xMax || a.xMax < b.xMin || a.yMin > b.yMax || a.yMax < b.yMin);
    }

    /// <summary>
    /// Returns the minimal translation vector to separate A from B (A is the player).
    /// Also mutates 'a' by the separation, and returns the delta applied to A.
    /// </summary>
    private static Vector2 ResolveAABBCollision(ref Rect a, Rect b, float bias)
    {
        float left   = b.xMin - a.xMax; // if negative, penetration from right
        float right  = b.xMax - a.xMin; // if positive, penetration from left
        float down   = b.yMin - a.yMax; // negative = penetration from top
        float up     = b.yMax - a.yMin; // positive = penetration from bottom

        // Overlap distances along each axis (positive numbers)
        float overlapX = (Mathf.Abs(left) < right) ? left : right;
        float overlapY = (Mathf.Abs(down) < up)    ? down : up;

        // Choose the smaller magnitude axis to resolve
        Vector2 delta;
        if (Mathf.Abs(overlapX) < Mathf.Abs(overlapY))
        {
            // separate along X
            float dx = overlapX;
            dx += (dx > 0 ? bias : -bias);
            a.x += dx;
            delta = new Vector2(dx, 0f);
        }
        else
        {
            // separate along Y
            float dy = overlapY;
            dy += (dy > 0 ? bias : -bias);
            a.y += dy;
            delta = new Vector2(0f, dy);
        }
        return delta;
    }

    // Call if you add/remove obstacles at runtime
    public void RebuildObstacleList()
    {
        if (!mapPanel) return;
        obstacles.Clear();
        foreach (var o in mapPanel.GetComponentsInChildren<ObstacleUI>(true))
            obstacles.Add(o);
    }
}
