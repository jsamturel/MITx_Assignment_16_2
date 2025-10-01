using UnityEngine;
using TMPro;

public class DeliveryTargetUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform homeRect;      // auto if null
    [SerializeField] private TMP_Text promptText;         // "Press Enter to deliver"

    [Header("Prompt")]
    [SerializeField] private string promptMessage = "Press Enter to deliver";
    [SerializeField] private float promptRadius = 80f;

    [Header("State (read-only at runtime)")]
    [SerializeField] private bool delivered = false;

    public bool Delivered => delivered;
    public RectTransform Rect => homeRect ? homeRect : (homeRect = GetComponent<RectTransform>());

    void Reset()
    {
        homeRect = GetComponent<RectTransform>();
        if (!promptText)
        {
            foreach (var t in GetComponentsInChildren<TMP_Text>(true))
            {
                if (t.name.ToLower().Contains("prompt")) { promptText = t; break; }
            }
        }
    }

    void Awake()
    {
        if (promptText)
        {
            promptText.gameObject.SetActive(false);
            promptText.text = promptMessage;
        }
    }

    /// <summary>
    /// Show/hide the prompt based on distance, unless already delivered.
    /// Call this each frame from your manager with the player's anchoredPosition.
    /// </summary>
    public void UpdatePrompt(Vector2 playerAnchoredPos)
    {
        if (!promptText) return;

        if (delivered)
        {
            if (promptText.gameObject.activeSelf) promptText.gameObject.SetActive(false);
            return;
        }

        float dist = Vector2.Distance(playerAnchoredPos, Rect.anchoredPosition);
        bool show = dist <= promptRadius;

        if (show)
        {
            if (promptText.text != promptMessage) promptText.text = promptMessage;
            if (!promptText.gameObject.activeSelf) promptText.gameObject.SetActive(true);
        }
        else
        {
            if (promptText.gameObject.activeSelf) promptText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Immediately marks delivered and hides the prompt. Returns true if we changed state.
    /// Use this if your manager already checked distance.
    /// </summary>
    public bool TryDeliver()
    {
        if (delivered) return false;
        MarkDelivered();
        return true;
    }

    /// <summary>
    /// Checks distance and, if close enough, marks delivered and hides the prompt.
    /// Returns true if delivery happened this call.
    /// </summary>
    public bool TryDeliver(Vector2 playerAnchoredPos, float interactRadius)
    {
        if (delivered) return false;

        float dist = Vector2.Distance(playerAnchoredPos, Rect.anchoredPosition);
        if (dist <= interactRadius)
        {
            MarkDelivered();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Sets delivered = true and hides the prompt (if present).
    /// </summary>
    public void MarkDelivered()
    {
        delivered = true;
        if (promptText && promptText.gameObject.activeSelf)
            promptText.gameObject.SetActive(false);
    }
}
