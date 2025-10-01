using UnityEngine;

[RequireComponent(typeof(PlayerController2D))]
public class ScooterAudioController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerController2D player;  // auto if left null
    [SerializeField] private AudioSource engineSource;    // on PlayerScooter
    [SerializeField] private AudioSource ambientSource;   // on AudioManager

    [Header("Mix")]
    [SerializeField] private float engineMaxVol = 0.85f;
    [SerializeField] private float ambientMaxVol = 1.0f;
    [SerializeField] private float fadeUpPerSec = 3.0f;   // volume/sec
    [SerializeField] private float fadeDownPerSec = 3.5f; // volume/sec

    [Header("Engine feel")]
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.25f;

    // consider moving threshold down if you want “engine on” when barely nudging
    [SerializeField] private float moveThreshold = 0.05f; // 0..1, uses player.IsMoving check

    void Awake()
    {
        if (!player) player = GetComponent<PlayerController2D>();
        if (!engineSource)
        {
            engineSource = GetComponent<AudioSource>();
            if (!engineSource) Debug.LogWarning("[ScooterAudioController] Assign engine AudioSource.");
        }
        if (!ambientSource)
        {
            // Try to find one in scene named "AudioManager"
            var mgr = GameObject.Find("AudioManager");
            if (mgr) ambientSource = mgr.GetComponent<AudioSource>();
        }

        if (engineSource)
        {
            engineSource.loop = true;
            engineSource.volume = 0f;
            // don't play on awake; we’ll kick it when needed
        }
        if (ambientSource)
        {
            ambientSource.loop = true;
            if (!ambientSource.isPlaying) ambientSource.Play();
            ambientSource.volume = ambientMaxVol;
        }
    }

    void Update()
    {
        if (!player || !engineSource || !ambientSource) return;

        bool moving = player.IsMoving; // add IsMoving getter on PlayerController2D (below)
        float dt = Time.deltaTime;

        // Fade logic: crossfade engine/ambient
        float targetEngine = moving ? engineMaxVol : 0f;
        float targetAmbient = moving ? ambientMaxVol * 0.45f : ambientMaxVol;

        engineSource.volume = MoveToward(engineSource.volume, targetEngine,
            (targetEngine > engineSource.volume ? fadeUpPerSec : fadeDownPerSec) * dt);

        ambientSource.volume = MoveToward(ambientSource.volume, targetAmbient,
            (targetAmbient > ambientSource.volume ? fadeUpPerSec : fadeDownPerSec) * dt);

        // Start/stop engine playback as needed (avoid constant Play() calls)
        if (moving && !engineSource.isPlaying) engineSource.Play();
        if (!moving && engineSource.isPlaying && engineSource.volume <= 0.01f) engineSource.Stop();

        // Pitch with speed for nice feel
        float speed01 = player.CurrentSpeed01; // 0..1
        engineSource.pitch = Mathf.Lerp(minPitch, maxPitch, speed01);
    }

    private float MoveToward(float a, float b, float maxDelta)
    {
        if (Mathf.Approximately(a, b)) return b;
        if (a < b) return Mathf.Min(a + maxDelta, b);
        else       return Mathf.Max(a - maxDelta, b);
    }
}
