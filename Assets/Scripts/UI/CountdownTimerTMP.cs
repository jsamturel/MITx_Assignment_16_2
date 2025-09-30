using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;

public class CountdownTimerTMP : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text penaltyText;

    [Header("Timing")]
    [SerializeField] private int startSeconds = 60;
    [SerializeField] private int penaltySeconds = 0;
    [SerializeField] private bool autoStart = true;

    [Header("Events")]
    [Tooltip("Invoked exactly once when the timer reaches 0.")]
    [SerializeField] private UnityEvent onTimerFinished;

    private float remaining;
    private bool running;
    private bool firedEvent = false;

    // Legacy Action for code subscribers (optional)
    public Action OnTimerEnded;

    void Awake()
    {
        remaining = startSeconds;
        UpdateUI();
    }

    void Start()
    {
        if (autoStart) running = true;
    }

    void Update()
    {
        if (!running) return;

        // Use scaled time so Pause stops the timer
        remaining -= Time.deltaTime;

        if (remaining <= 0f)
        {
            remaining = 0f;
            running = false;

            if (!firedEvent)
            {
                firedEvent = true; // ensure it only fires once
                OnTimerEnded?.Invoke();
                onTimerFinished?.Invoke();
            }
        }

        UpdateUI();
    }

    public void AddPenalty(int seconds)
    {
        penaltySeconds += seconds;
        remaining = Mathf.Max(0f, remaining - seconds);
        UpdateUI();
    }

    public void StartTimer() => running = true;
    public void StopTimer()  => running = false;

    public void ResetTimer(int newStartSeconds)
    {
        startSeconds   = newStartSeconds;
        penaltySeconds = 0;
        remaining      = startSeconds;
        running        = false;
        firedEvent     = false;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (timerText)   timerText.text   = $"Time: {Mathf.CeilToInt(remaining):00}";
        if (penaltyText) penaltyText.text = $"Penalty: {penaltySeconds}s";
    }
}
