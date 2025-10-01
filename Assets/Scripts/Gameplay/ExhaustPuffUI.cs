using UnityEngine;
using UnityEngine.UI;

public class ExhaustPuffUI : MonoBehaviour
{
    [SerializeField] private Image img;
    private RectTransform rt;

    private float life;
    private float maxLife;
    private Vector2 velocity;
    private float startScale;
    private float endScale;
    private float baseAlpha;

    // Extra feel
    private float rotateSpeedDeg;
    private Vector2 jitterPerSec;
    private float startAngle;

    public void Play(
        RectTransform parent,
        Vector2 startPos,
        Vector2 dir,
        float speed,
        float duration,
        float s0 = 0.9f,
        float s1 = 1.8f,
        float alpha = 1f,
        float rotateSpeedDegPerSec = 30f,
        Vector2 jitterPerSecond = default,
        Sprite variant = null,
        float startAngleJitterDeg = 12f   // NEW: small visual angle jitter ±
    )
    {
        if (!rt) rt = GetComponent<RectTransform>();
        if (!img) img = GetComponent<Image>();

        transform.SetParent(parent, false);
        rt.anchoredPosition = startPos;

        if (variant != null) img.sprite = variant;

        dir = dir.sqrMagnitude > 0.001f ? dir.normalized : Vector2.left;
        velocity = dir * speed;
        maxLife = Mathf.Max(0.1f, duration);
        life = 0f;

        startScale = s0;
        endScale   = s1;
        baseAlpha  = Mathf.Clamp01(alpha);

        var c = img.color; c.a = baseAlpha; img.color = c;

        // Small initial visual rotation jitter
        startAngle     = Random.Range(-startAngleJitterDeg, startAngleJitterDeg);
        rotateSpeedDeg = rotateSpeedDegPerSec;
        jitterPerSec   = jitterPerSecond;

        rt.localScale       = Vector3.one * startScale;
        rt.localEulerAngles = new Vector3(0, 0, startAngle);

        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!rt) return;

        life += Time.deltaTime;
        float t = Mathf.Clamp01(life / maxLife);

        // Ease for scale, late fade for fuller look
        float scaleEase = 1f - Mathf.Pow(1f - t, 2f);    // ease-out
        float fade      = 1f - Mathf.SmoothStep(0.55f, 1f, t); // fade near the end

        rt.anchoredPosition += (velocity + jitterPerSec) * Time.deltaTime;

        float s = Mathf.Lerp(startScale, endScale, scaleEase);
        rt.localScale = Vector3.one * s;

        if (Mathf.Abs(rotateSpeedDeg) > 0.01f)
            rt.Rotate(0f, 0f, rotateSpeedDeg * Time.deltaTime);

        if (img)
        {
            var c = img.color;
            c.a = baseAlpha * Mathf.Clamp01(fade);
            img.color = c;
        }

        if (life >= maxLife)
            gameObject.SetActive(false);
    }
}
