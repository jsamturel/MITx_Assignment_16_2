using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Moves a UI Image (player scooter) within a Map panel (RectTransform) using Input System.
/// - Move (Vector2 arrows/left stick) for direction
/// - Accelerate/Brake scale speed up/down (W/RT, S/LT)
/// - Honk plays AudioSource when pressed (Space / X|Square)
/// </summary>
public class PlayerController2D : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform mapPanel;
    [SerializeField] private RectTransform playerRect;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 350f;
    [SerializeField] private float acceleration = 600f;
    [SerializeField] private float brakeDecel = 800f;
    [SerializeField] private float baseSpeedScalar = 0.6f;

    [Header("Audio")]
    [SerializeField] private AudioSource honkAudio;

    [Header("Input System")]
    [SerializeField] private bool usePlayerInput = true;
#if ENABLE_INPUT_SYSTEM
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string accelerateActionName = "Accelerate";
    [SerializeField] private string brakeActionName = "Brake";
    [SerializeField] private string honkActionName = "Honk";

    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference accelerate;
    [SerializeField] private InputActionReference brake;
    [SerializeField] private InputActionReference honk;
#endif

    private Vector2 dirInput;
    private float speedScalar;
    private Vector2 velocity;

    void Awake()
    {
        if (!mapPanel || !playerRect)
            Debug.LogError("[PlayerController2D] Assign Map Panel and Player Rect.");
        speedScalar = baseSpeedScalar;
    }

    void OnEnable()
    {
#if ENABLE_INPUT_SYSTEM
        if (!usePlayerInput)
        {
            move?.action.Enable();
            accelerate?.action.Enable();
            brake?.action.Enable();
            honk?.action.Enable();
        }
#endif
    }

    void OnDisable()
    {
#if ENABLE_INPUT_SYSTEM
        if (!usePlayerInput)
        {
            move?.action.Disable();
            accelerate?.action.Disable();
            brake?.action.Disable();
            honk?.action.Disable();
        }
#endif
    }

    void Update()
    {
        ReadInput();
        UpdateMovement(Time.unscaledDeltaTime);
    }

    private void ReadInput()
    {
#if ENABLE_INPUT_SYSTEM
        if (usePlayerInput && playerInput != null)
        {
            var moveAction = playerInput.actions[moveActionName];
            var accelAction = playerInput.actions[accelerateActionName];
            var brakeAction = playerInput.actions[brakeActionName];
            var honkAction = playerInput.actions[honkActionName];

            dirInput = moveAction.ReadValue<Vector2>();
            bool accelHeld = accelAction.IsPressed();
            bool brakeHeld = brakeAction.IsPressed();

            float target = baseSpeedScalar;
            if (accelHeld) target = 1f;
            if (brakeHeld) target = 0.2f;
            speedScalar = Mathf.MoveTowards(speedScalar, target, (accelHeld ? acceleration : brakeDecel) * Time.unscaledDeltaTime / 1000f);

            if (honkAction.WasPressedThisFrame() && honkAudio)
                honkAudio.Play();
        }
        else
        {
            dirInput = move ? move.action.ReadValue<Vector2>() : Vector2.zero;
            bool accelHeld = accelerate ? accelerate.action.IsPressed() : false;
            bool brakeHeld = brake ? brake.action.IsPressed() : false;
            float target = baseSpeedScalar;
            if (accelHeld) target = 1f;
            if (brakeHeld) target = 0.2f;
            speedScalar = Mathf.MoveTowards(speedScalar, target, (accelHeld ? acceleration : brakeDecel) * Time.unscaledDeltaTime / 1000f);

            if (honk != null && honk.action.WasPressedThisFrame() && honkAudio)
                honkAudio.Play();
        }
#else
        // Legacy fallback without the Input System:
        dirInput = new Vector2(
            (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0),
            (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) - (Input.GetKey(KeyCode.DownArrow) ? 1 : 0)
        );
        dirInput = Vector2.ClampMagnitude(dirInput, 1f);

        float target = baseSpeedScalar;
        if (Input.GetKey(KeyCode.W)) target = 1f;
        if (Input.GetKey(KeyCode.S)) target = 0.2f;
        speedScalar = Mathf.MoveTowards(speedScalar, target, acceleration * Time.unscaledDeltaTime / 1000f);

        if (Input.GetKeyDown(KeyCode.Space) && honkAudio) honkAudio.Play();
#endif
    }

    private void UpdateMovement(float dt)
    {
        Vector2 targetVel = dirInput.normalized * (maxSpeed * Mathf.Clamp01(speedScalar));
        velocity = Vector2.MoveTowards(velocity, targetVel, acceleration * dt);

        Vector2 pos = playerRect.anchoredPosition;
        pos += velocity * dt;

        Rect rect = GetLocalRect(mapPanel);
        Vector2 half = playerRect.rect.size * 0.5f;
        float minX = rect.xMin + half.x;
        float maxX = rect.xMax - half.x;
        float minY = rect.yMin + half.y;
        float maxY = rect.yMax - half.y;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        playerRect.anchoredPosition = pos;
    }

    private Rect GetLocalRect(RectTransform rt)
    {
        Vector2 size = rt.rect.size;
        return new Rect(-size * 0.5f, size);
    }

    public float CurrentSpeed01 => Mathf.InverseLerp(0f, maxSpeed, velocity.magnitude);
}