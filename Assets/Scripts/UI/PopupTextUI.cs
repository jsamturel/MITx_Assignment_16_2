using UnityEngine;
using TMPro;
using System.Collections;

public class PopupTextUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TMP_Text label; // auto if left null
    [SerializeField] private RectTransform rt;

    [Header("Anim")]
    [SerializeField] private float lifetime = 1.0f;   // seconds
    [SerializeField] private float risePixels = 40f;  // moves up slightly
    [SerializeField] private float startScale = 0.9f;
    [SerializeField] private float endScale = 1.0f;

    private Vector2 startPos;

    void Awake()
    {
        if (!rt) rt = GetComponent<RectTransform>();
        if (!label) label = GetComponent<TMP_Text>();
    }

    public void Show(RectTransform parent, Vector2 anchoredPos, string message,
                     float? overrideLifetime = null, float? overrideRise = null)
    {
        transform.SetParent(parent, false);
        startPos = anchoredPos;
        rt.anchoredPosition = anchoredPos;
        if (label) label.text = message;
        if (overrideLifetime.HasValue) lifetime = overrideLifetime.Value;
        if (overrideRise.HasValue)     risePixels = overrideRise.Value;
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        float t = 0f;
        Color c = label ? label.color : Color.white;
        float startA = c.a;

        while (t < lifetime)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / lifetime);
            // ease out
            float ease = 1f - Mathf.Pow(1f - u, 2f);

            // position/scale
            rt.anchoredPosition = startPos + new Vector2(0f, ease * risePixels);
            float s = Mathf.Lerp(startScale, endScale, ease);
            rt.localScale = new Vector3(s, s, 1f);

            // fade
            if (label)
            {
                c.a = Mathf.Lerp(startA, 0f, ease);
                label.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
