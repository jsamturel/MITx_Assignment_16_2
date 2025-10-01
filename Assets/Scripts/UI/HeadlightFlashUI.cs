using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeadlightFlashUI : MonoBehaviour
{
    [SerializeField] private Image img;                // Headlight Image
    [SerializeField, Range(0f,1f)] private float maxAlpha = 0.55f;
    [SerializeField] private float holdTime = 0.08f;   // bright time
    [SerializeField] private float fadeTime = 0.22f;   // fade out
    [SerializeField] private float scalePunch = 1.12f; // quick zoom
    [SerializeField] private float scaleReturnTime = 0.16f;

    private RectTransform rt;
    private Coroutine routine;
    private Color baseColor;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        if (!img) img = GetComponent<Image>();
        baseColor = img.color;
        baseColor.a = 0f;
        img.color = baseColor;
    }

    public void Flash()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // pop alpha up
        var c = img.color;
        c.a = maxAlpha;
        img.color = c;

        // scale punch
        Vector3 start = Vector3.one;
        Vector3 peak  = Vector3.one * scalePunch;
        float t = 0f;
        while (t < scaleReturnTime)
        {
            t += Time.deltaTime;
            float k = 1f - Mathf.Clamp01(t / scaleReturnTime);
            rt.localScale = Vector3.Lerp(start, peak, k); // quick punch then return
            yield return null;
        }
        rt.localScale = Vector3.one;

        // hold bright
        yield return new WaitForSeconds(holdTime);

        // fade out
        t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fadeTime);
            c.a = Mathf.Lerp(maxAlpha, 0f, k);
            img.color = c;
            yield return null;
        }
        c.a = 0f;
        img.color = c;
        routine = null;
    }
}
