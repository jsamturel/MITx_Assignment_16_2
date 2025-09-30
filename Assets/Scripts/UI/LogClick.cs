using UnityEngine;

public class LogClick : MonoBehaviour
{
    public void LogIt(string tag)
    {
        Debug.Log($"[UI] Click: {tag}");
    }
}
