using UnityEngine;

public class MapPanelController : MonoBehaviour
{
    [SerializeField] private GameObject mapPanel;

    void Start()
    {
        if (mapPanel) mapPanel.SetActive(true); // visible in intro wireframe
    }

    public void ShowMap(bool show)
    {
        if (mapPanel) mapPanel.SetActive(show);
    }
}
