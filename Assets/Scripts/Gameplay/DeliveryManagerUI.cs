using UnityEngine;
using UnityEngine.InputSystem; // optional if using PlayerInput for Interact

public class DeliveryManagerUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform playerRect;          // PlayerScooter
    [SerializeField] private DeliveryTargetUI[] targets;        // Home1, Home2
    [SerializeField] private HUDControllerTMP hud;              // deliveries & score

    [Header("Interact")]
    [SerializeField] private bool usePlayerInput = false;       // set true if you wired an Interact action
    [SerializeField] private UnityEngine.InputSystem.PlayerInput playerInput;
    [SerializeField] private string interactActionName = "Interact";
    [SerializeField] private float interactRadius = 60f;

    [Header("Popup")]
    [SerializeField] private RectTransform popupParent;         // MapPanel
    [SerializeField] private PopupTextUI popupPrefab;           // PopupTextUI.prefab
    [SerializeField] private Vector2 popupOffset = new Vector2(0f, 36f);
    [SerializeField] private string popupMessage = "Delivery made";
    [SerializeField] private float popupLifetime = 1.0f;

    [Header("Scoring")]
    [SerializeField] private int scorePerDelivery = 50;
    [SerializeField] private int totalDeliveries = 2;           // for HUD init

    private InputAction interactAction; // cached if using PlayerInput

    void Awake()
    {
        if (hud) hud.SetTotalDeliveries(totalDeliveries);

        if (usePlayerInput && playerInput != null && !string.IsNullOrEmpty(interactActionName))
            interactAction = playerInput.actions[interactActionName];
    }

    void Update()
    {
        if (playerRect == null || targets == null) return;

        // 1) keep prompts updated
        Vector2 p = playerRect.anchoredPosition;
        foreach (var t in targets)
            if (t) t.UpdatePrompt(p);

        // 2) interact check
        bool pressed = false;
        if (usePlayerInput && interactAction != null)
        {
            pressed = interactAction.WasPressedThisFrame();
        }
        else
        {
            // keyboard fallback (Enter)
            pressed = Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame;
        }

        if (pressed)
            TryDeliverNearest(p);
    }

    private void TryDeliverNearest(Vector2 playerPos)
    {
        DeliveryTargetUI best = null;
        float bestDist = float.MaxValue;

        foreach (var t in targets)
        {
            if (t == null || t.Delivered) continue;
            float d = Vector2.Distance(playerPos, t.Rect.anchoredPosition);
            if (d < bestDist)
            {
                bestDist = d;
                best = t;
            }
        }

        if (best != null && bestDist <= interactRadius)
        {
            if (best.TryDeliver())
            {
                // HUD updates
                if (hud != null)
                {
                    hud.AddDelivery(1);
                    hud.AddScore(scorePerDelivery);
                }

                // Popup
                if (popupPrefab != null && popupParent != null)
                {
                    var pop = Instantiate(popupPrefab);
                    pop.Show(popupParent, best.Rect.anchoredPosition + popupOffset,
                             popupMessage, popupLifetime, null);
                }
            }
        }
    }
}
