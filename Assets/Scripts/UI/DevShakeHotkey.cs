using UnityEngine;

public class DevShakeHotkey : MonoBehaviour
{
    [SerializeField] private ScreenShakeUI shaker;
    [SerializeField] private KeyCode key = KeyCode.H;
    [SerializeField, Range(0f,1f)] private float intensity = 0.3f;

    void Update()
    {
        if (Input.GetKeyDown(key) && shaker != null)
            shaker.Shake(intensity);
    }
}
