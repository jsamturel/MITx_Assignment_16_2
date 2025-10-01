using UnityEngine;
using TMPro;
using UnityEngine.Events;

[System.Serializable] public class IntEvent : UnityEvent<int> { }

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

    [Header("Events")]
    public UnityEvent onDelivered;     // e.g., HUDControllerTMP.AddDelivery()
    public IntEvent onAddScore;        // e.g., HUDControllerTMP.AddScore(int)

    public bool Delivered => delivered;
    public RectTransform Rect => homeRect ? homeRect : (homeRect = GetComponent<RectTransform>());

    void Reset()
    {
        homeRect = GetComponent<RectTransform>();
        if (!promptText)
        {
            foreach (var t in GetComponentsInChildren<TMP_Text>(true))
                if (t.name.ToLower().Contains("prompt")) { promptText = t; break; }
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

    public bool TryDeliver()
    {
        if (delivered) return false;
        MarkDelivered();
        return true;
    }

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

    public void MarkDelivered()
    {
        if (delivered) return;
        delivered = true;

        if (promptText && promptText.gameObject.activeSelf)
            promptText.gameObject.SetActive(false);

        // Fire events
        onDelivered?.Invoke();
        onAddScore?.Invoke(50); // default 50; you can change per-home in Inspector
    }
}
