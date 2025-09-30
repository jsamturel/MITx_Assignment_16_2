using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform mapPanel;   // The container the scooter moves within
    [SerializeField] private RectTransform playerRect; // The scooter's RectTransform (UI Image)

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 350f;        // px/sec
    [SerializeField] private float acceleration = 600f;    // px/sec^2 (toward target vel)
    [SerializeField] private float brakeDecel = 800f;      // px/sec^2 (toward lower speed)
    [SerializeField] private float baseSpeedScalar = 0.6f; // idle forward factor (0..1)

    [Header("Audio")]
    [SerializeField] private AudioSource honkAudio;

    [Header("Input System (PlayerInput)")]
    [SerializeField] private bool usePlayerInput = true;
    [SerializeField] private PlayerInput playerInput;               // Assign the PlayerInput on UIManager (or same GameObject)
    [SerializeField] private string moveActionName = "Move";        // Vector2
    [SerializeField] private string accelerateActionName = "Accelerate"; // Button
    [SerializeField] private string brakeActionName = "Brake";           // Button
    [SerializeField] private string honkActionName = "Honk";             // Button

    [Header("Orientation")]
    [SerializeField] private bool rotateToVelocity = true;
    [SerializeField] private float maxTiltDegrees = 35f;   // clamp for readability
    [SerializeField] private float rotationSmooth = 12f;   // higher = snappier

    // ----- internal state -----
    private Vector2 dirInput;       // normalized move input (from arrows/left stick)
    private float speedScalar;      // 0..1 scaled by accel/brake
    private Vector2 velocity;       // current velocity in UI px/sec
    private bool lastAccelHeld;     // public getter exposes this

    void Awake()
    {
        if (!mapPanel || !playerRect)
            Debug.LogWarning("[PlayerController2D] Assign Map Panel and Player Rect.");

        speedScalar = baseSpeedScalar;
    }

    void Update()
    {
        ReadInput();                       // read actions (scaled time)
        UpdateMovement(Time.deltaTime);    // obeys pause (timeScale)
    }

    // ----------------- INPUT -----------------
    private void ReadInput()
    {
        if (!(usePlayerInput && playerInput != null))
            return; // nothing to read (explicitly no legacy fallback)

        // Actions by name (make sure they exist in the assigned InputActions asset)
        var moveAction  = playerInput.actions[moveActionName];
        var accelAction = playerInput.actions[accelerateActionName];
        var brakeAction = playerInput.actions[brakeActionName];
        var honkAction  = playerInput.actions[honkActionName];

        dirInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        dirInput = Vector2.ClampMagnitude(dirInput, 1f);

        bool accelHeld = accelAction != null && accelAction.IsPressed();
        bool brakeHeld = brakeAction != null && brakeAction.IsPressed();
        lastAccelHeld = accelHeld;

        // target speed scalar based on accel/brake
        float target = baseSpeedScalar;
        if (accelHeld) target = 1f;
        if (brakeHeld) target = 0.2f;

        // smooth the scalar toward target
        float rate = accelHeld ? acceleration : brakeDecel;
        speedScalar = Mathf.MoveTowards(speedScalar, target, rate * Time.deltaTime / 1000f);

        if (honkAction != null && honkAction.WasPressedThisFrame() && honkAudio)
            honkAudio.Play();
    }

    // --------------- MOVEMENT ----------------
    private void UpdateMovement(float dt)
    {
        // desired velocity from input * maxSpeed * scalar
        Vector2 targetVel = dirInput.normalized * (maxSpeed * Mathf.Clamp01(speedScalar));

        // move velocity toward target (simple accel model)
        velocity = Vector2.MoveTowards(velocity, targetVel, acceleration * dt);

        // integrate position
        Vector2 pos = playerRect.anchoredPosition;
        pos += velocity * dt;

        // clamp inside map panel rect
        Rect rect = GetLocalRect(mapPanel);
        Vector2 half = playerRect.rect.size * 0.5f;
        float minX = rect.xMin + half.x;
        float maxX = rect.xMax - half.x;
        float minY = rect.yMin + half.y;
        float maxY = rect.yMax - half.y;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        playerRect.anchoredPosition = pos;

        // optional: orient to velocity
        if (rotateToVelocity)
        {
            Vector2 v = velocity;
            float targetAngle = 0f;

            if (v.sqrMagnitude > 0.01f)
            {
                targetAngle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
                targetAngle = Mathf.Clamp(targetAngle, -maxTiltDegrees, maxTiltDegrees);
            }

            float currentZ = playerRect.localEulerAngles.z;
            if (currentZ > 180f) currentZ -= 360f; // map to -180..180 for lerp

            float newZ = Mathf.Lerp(currentZ, targetAngle, 1f - Mathf.Exp(-rotationSmooth * dt));
            playerRect.localEulerAngles = new Vector3(0f, 0f, newZ);
        }
    }

    private Rect GetLocalRect(RectTransform rt)
    {
        Vector2 size = rt.rect.size;
        return new Rect(-size * 0.5f, size); // anchoredPosition space
    }

    // --------------- PUBLIC GETTERS ----------------
    public Vector2 CurrentVelocity => velocity;
    public bool IsAccelerating => lastAccelHeld;
    public float CurrentSpeed01 => Mathf.InverseLerp(0f, maxSpeed, velocity.magnitude);
}
