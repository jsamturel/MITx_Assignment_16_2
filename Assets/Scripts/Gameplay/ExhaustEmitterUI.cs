using UnityEngine;

public class ExhaustEmitterUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform mapPanel;
    [SerializeField] private RectTransform playerRect;
    [SerializeField] private ExhaustPuffUI puffPrefab;

    [Header("Emission")]
    [SerializeField] private float spawnRatePerSec = 12f;
    [SerializeField] private float minSpeedToEmit = 60f;
    [SerializeField] private Vector2 localOffset = new Vector2(-24f, 0f);

    [Header("Puff Motion (base)")]
    [SerializeField] private float puffSpeed = 80f;
    [SerializeField] private float puffLife  = 0.6f;
    [SerializeField] private float startScale = 0.8f;
    [SerializeField] private float endScale   = 1.6f;
    [SerializeField, Range(0f,1f)] private float baseAlpha = 0.9f;

    [Header("Randomization")]
    [SerializeField] private float speedVariance = 0.25f;      // +-%
    [SerializeField] private float lifeVariance  = 0.15f;      // +-%
    [SerializeField] private float scaleVariance = 0.15f;      // +-%
    [SerializeField] private float alphaVariance = 0.15f;      // +-%
    [SerializeField] private float rotateSpeedMin = -45f;
    [SerializeField] private float rotateSpeedMax =  45f;
    [SerializeField] private Vector2 jitterMin = new Vector2(-10f, -6f);
    [SerializeField] private Vector2 jitterMax = new Vector2( 10f,  6f);

    private PlayerController2D player;
    private float spawnTimer;

    void Awake()
    {
        if (!mapPanel || !playerRect || !puffPrefab)
            Debug.LogWarning("[ExhaustEmitterUI] Assign MapPanel, PlayerRect, and PuffPrefab.");

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
        Vector2 dir = vel.sqrMagnitude > 0.001f ? -vel.normalized : Vector2.left;
        Vector2 right = vel.sqrMagnitude > 0.001f ? vel.normalized : Vector2.right;
        Vector2 up    = new Vector2(-right.y, right.x);

        Vector2 spawnPos = playerRect.anchoredPosition
                         + right * localOffset.x
                         + up    * localOffset.y;

        // Variations
        float speed   = puffSpeed * (1f + Random.Range(-speedVariance,  speedVariance));
        float life    = puffLife  * (1f + Random.Range(-lifeVariance,   lifeVariance));
        float s0      = startScale * (1f + Random.Range(-scaleVariance, scaleVariance));
        float s1      = endScale   * (1f + Random.Range(-scaleVariance, scaleVariance));
        float alpha   = Mathf.Clamp01(baseAlpha * (1f + Random.Range(-alphaVariance, alphaVariance)));
        float rotSpd  = Random.Range(rotateSpeedMin, rotateSpeedMax);
        Vector2 jitter= new Vector2(Random.Range(jitterMin.x, jitterMax.x),
                                    Random.Range(jitterMin.y, jitterMax.y));

        var puff = Instantiate(puffPrefab);
        puff.Play(mapPanel, spawnPos, dir, speed, life, s0, s1, alpha, rotSpd, jitter);
    }
}
