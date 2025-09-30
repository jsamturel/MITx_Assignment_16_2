using UnityEngine;
using TMPro;

public class HUDControllerTMP : MonoBehaviour
{
    [SerializeField] private TMP_Text deliveriesText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private int totalDeliveries = 2;

    private int delivered = 0;
    private int score = 0;

    void Start() => Refresh();

    public void AddDelivery(int amount = 1)
    {
        delivered += amount;
        delivered = Mathf.Clamp(delivered, 0, totalDeliveries);
        Refresh();
    }

    public void SetTotalDeliveries(int total)
    {
        totalDeliveries = Mathf.Max(0, total);
        Refresh();
    }

    public void AddScore(int amount)
    {
        score += amount;
        Refresh();
    }

    private void Refresh()
    {
        if (deliveriesText) deliveriesText.text = $"Deliveries: {delivered}/{totalDeliveries}";
        if (scoreText) scoreText.text = $"Score: {score:000}";
    }
}
