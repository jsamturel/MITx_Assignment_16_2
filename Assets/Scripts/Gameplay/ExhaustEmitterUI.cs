using UnityEngine;

public class ExhaustEmitterUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform mapPanel;
    [SerializeField] private RectTransform playerRect;
    [SerializeField] private ExhaustPuffUI puffPrefab;

    [Header("Variants")]
    [Tooltip("Optional smoke sprite variants for visual variety.")]
    [SerializeField] private Sprite[] smokeVariants; // Drag Smoke_01..Smoke_05 here

    [Header("Emission")]
    [Tooltip("Puffs per second while accelerating & moving.")]
    [SerializeField] private float spawnRatePerSec = 10f;
    [Tooltip("Minimum speed (px/s) before emitting.")]
    [SerializeField] private float minSpeedToEmit = 10f;
    [Tooltip("Offset from scooter local frame; X<0 = behind.")]
    [SerializeField] private Vector2 localOffset = new Vector2(-24f, 0f);

    [Header("Puff Base")]
    [SerializeField] private float puffSpeed = 80f;      // you set this to 80
    [SerializeField] private float puffLife  = 0.95f;    // slightly longer life
    [SerializeField] private float startScale = 0.9f;
    [SerializeField] private float endScale   = 1.8f;
    [SerializeField, Range(0f,1f)] private float baseAlpha = 0.95f;

    [Header("Randomization")]
    [SerializeField] private float speedVariance = 0.2f;     // +-%
    [SerializeField] private float lifeVariance  = 0.1f;     // +-%
    [SerializeField] private float scaleVariance = 0.12f;    // +-%
    [SerializeField] private float alphaVariance = 0.1f;     // +-%
    [SerializeField] private float rotateSpeedMin = -40f;
    [SerializeField] private float rotateSpeedMax =  40f;
    [SerializeField] private Vector2 jitterMin = new Vector2(-8f, -5f);
    [SerializeField] private Vector2 jitterMax = new Vector2(  8f,  5f);

    [Header("Angle Jitter")] // NEW header you should see
    [SerializeField, Range(0f, 25f)] private float driftAngleJitterDeg = 6f;   // tiny cone spray
    [SerializeField, Range(0f, 30f)] private float startAngleJitterDeg = 12f;  // visual sprite tilt

    private PlayerController2D player;
    private float spawnTimer;

    void Awake()
    {
        if (!mapPanel || !playerRect || !puffPrefab)
            Debug.LogWarning("[ExhaustEmitterUI] Assign MapPanel, PlayerRect, PuffPrefab.");
        player = playerRect ? playerRect.GetComponentInParent<PlayerController2D>() : null;
    }

    void Update()
    {
        if (player == null || mapPanel == null || playerRect == null || puffPrefab == null)
            return;

        Vector2 vel = player.CurrentVelocity;
        bool accelerating = player.IsAccelerating;

        if (!accelerating || vel.magnitude < minSpeedToEmit)
            return;

        spawnTimer += Time.deltaTime;
        float interval = 1f / Mathf.Max(1f, spawnRatePerSec);

        while (spawnTimer >= interval)
        {
            spawnTimer -= interval;
            SpawnOne(vel);
        }
    }

    private void SpawnOne(Vector2 vel)
    {
        Vector2 dir   = vel.sqrMagnitude > 0.001f ? -vel.normalized : Vector2.left;
        Vector2 right = vel.sqrMagnitude > 0.001f ?  vel.normalized : Vector2.right;
        Vector2 up    = new Vector2(-right.y, right.x);

        // Tiny “spray” so trails aren’t laser-straight
        if (driftAngleJitterDeg > 0f)
        {
            float jitterDeg = Random.Range(-driftAngleJitterDeg, driftAngleJitterDeg);
            dir = Rotate2D(dir, jitterDeg);
        }

        Vector2 spawnPos = playerRect.anchoredPosition
                         + right * localOffset.x
                         + up    * localOffset.y;

        // Random variant and params
        Sprite chosen = null;
        if (smokeVariants != null && smokeVariants.Length > 0)
            chosen = smokeVariants[Random.Range(0, smokeVariants.Length)];

        float speed = puffSpeed * (1f + Random.Range(-speedVariance,  speedVariance));
        float life  = puffLife  * (1f + Random.Range(-lifeVariance,   lifeVariance));
        float s0    = startScale * (1f + Random.Range(-scaleVariance, scaleVariance));
        float s1    = endScale   * (1f + Random.Range(-scaleVariance, scaleVariance));
        float alpha = Mathf.Clamp01(baseAlpha * (1f + Random.Range(-alphaVariance, alphaVariance)));
        float rotSpd = Random.Range(rotateSpeedMin, rotateSpeedMax);
        Vector2 jitter = new Vector2(Random.Range(jitterMin.x, jitterMax.x),
                                     Random.Range(jitterMin.y, jitterMax.y));

        var puff = Instantiate(puffPrefab);
        puff.Play(mapPanel, spawnPos, dir, speed, life, s0, s1, alpha, rotSpd, jitter, chosen, startAngleJitterDeg);
    }

    private static Vector2 Rotate2D(Vector2 v, float degrees)
    {
        float r = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(r);
        float sin = Mathf.Sin(r);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }
}
