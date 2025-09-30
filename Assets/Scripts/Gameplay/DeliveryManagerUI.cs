using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class DeliveryManagerUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform mapPanel;
    [SerializeField] private RectTransform playerRect;
    [SerializeField] private DeliveryTargetUI[] targets;
    [SerializeField] private HUDControllerTMP hud;

    [Header("Settings")]
    [SerializeField] private int scorePerDelivery = 50;
    [SerializeField] private float interactRadius = 40f;

    [Header("Input")]
    [SerializeField] private bool usePlayerInput = true;
#if ENABLE_INPUT_SYSTEM
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string interactActionName = "Interact";
    [SerializeField] private InputActionReference interact;
#endif

    void Awake()
    {
        if (hud == null) hud = FindObjectOfType<HUDControllerTMP>();
    }

    void OnEnable()
    {
#if ENABLE_INPUT_SYSTEM
        if (!usePlayerInput && interact) interact.action.Enable();
#endif
    }

    void OnDisable()
    {
#if ENABLE_INPUT_SYSTEM
        if (!usePlayerInput && interact) interact.action.Disable();
#endif
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        bool interactPressed = false;
        if (usePlayerInput && playerInput != null)
        {
            var action = playerInput.actions[interactActionName];
            interactPressed = action != null && action.WasPressedThisFrame();
        }
        else if (interact != null)
        {
            interactPressed = interact.action.WasPressedThisFrame();
        }
        else
        {
            interactPressed = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
        }

        if (interactPressed && playerRect != null && targets != null)
        {
            Vector2 p = playerRect.anchoredPosition;
            foreach (var t in targets)
            {
                if (t != null && t.TryDeliver(p, interactRadius))
                {
                    if (hud != null)
                    {
                        hud.AddDelivery(1);
                        hud.AddScore(scorePerDelivery);
                    }
                }
            }
        }
#else
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Vector2 p = playerRect.anchoredPosition;
            foreach (var t in targets)
            {
                if (t != null && t.TryDeliver(p, interactRadius))
                {
                    if (hud != null)
                    {
                        hud.AddDelivery(1);
                        hud.AddScore(scorePerDelivery);
                    }
                }
            }
        }
#endif
    }
}
