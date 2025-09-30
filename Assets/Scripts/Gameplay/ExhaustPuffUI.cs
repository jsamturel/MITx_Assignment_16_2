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
    private float baseAlpha = 1f;

    // Extra juice
    private float rotateSpeedDeg;
    private Vector2 jitterPerSec;   // small lateral wander
    private float startAngle;

    public void Play(
        RectTransform parent,
        Vector2 startPos,
        Vector2 dir,
        float speed,
        float duration,
        float s0 = 0.8f,
        float s1 = 1.6f,
        float alpha = 1f,
        float rotateSpeedDegPerSec = 30f,
        Vector2 jitterPerSecond = default
    )
    {
        if (!rt) rt = GetComponent<RectTransform>();
        if (!img) img = GetComponent<Image>();

        transform.SetParent(parent, false);
        rt.anchoredPosition = startPos;

        dir = dir.sqrMagnitude > 0.001f ? dir.normalized : Vector2.left;
        velocity = dir * speed;

        maxLife = Mathf.Max(0.1f, duration);
        life = 0f;

        startScale = s0;
        endScale = s1;

        baseAlpha = Mathf.Clamp01(alpha);
        var c = img.color; c.a = baseAlpha; img.color = c;

        // Randomize a bit for variety
        startAngle = Random.Range(0f, 360f);
        rotateSpeedDeg = rotateSpeedDegPerSec;
        jitterPerSec = jitterPerSecond;

        rt.localScale = Vector3.one * startScale;
        rt.localEulerAngles = new Vector3(0, 0, startAngle);

        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!rt) return;

        life += Time.deltaTime;
        float t = Mathf.Clamp01(life / maxLife);

        // Easing (ease-out for movement/scale, smoother fade)
        float ease = 1f - Mathf.Pow(1f - t, 2f); // quadratic ease-out

        // Move + slight jitter
        rt.anchoredPosition += (velocity + jitterPerSec) * Time.deltaTime;

        // Scale up softly
        float s = Mathf.Lerp(startScale, endScale, ease);
        rt.localScale = Vector3.one * s;

        // Spin a bit for organic feel
        if (Mathf.Abs(rotateSpeedDeg) > 0.01f)
        {
            rt.Rotate(0f, 0f, rotateSpeedDeg * Time.deltaTime);
        }

        // Fade out (ease)
        if (img)
        {
            var c = img.color;
            c.a = baseAlpha * (1f - ease);
            img.color = c;
        }

        if (life >= maxLife)
            gameObject.SetActive(false);
    }
}
