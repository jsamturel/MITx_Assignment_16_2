using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PauseInputRelay : MonoBehaviour
{
    [SerializeField] private PauseController pauseController;
#if ENABLE_INPUT_SYSTEM
    [SerializeField] private PlayerInput playerInput;      // drag your UIManager's PlayerInput
    [SerializeField] private string pauseActionName = "Pause";
    private InputAction _pause;
#endif

    void Awake()
    {
        if (!pauseController)
            pauseController = GetComponent<PauseController>(); // ok if both live on UIManager
    }

#if ENABLE_INPUT_SYSTEM
    void OnEnable()
    {
        if (playerInput != null)
        {
            _pause = playerInput.actions[pauseActionName];
            if (_pause != null) _pause.performed += OnPausePerformed;
        }
    }

    void OnDisable()
    {
        if (_pause != null) _pause.performed -= OnPausePerformed;
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        if (pauseController != null) pauseController.TogglePause();
    }
#else
    void Update()
    {
        // Legacy fallback if not using the Input System package
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            if (pauseController != null) pauseController.TogglePause();
    }
#endif
}
