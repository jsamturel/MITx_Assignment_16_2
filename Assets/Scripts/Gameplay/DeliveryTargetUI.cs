using UnityEngine;
using TMPro;
using System.Collections;

public class DeliveryTargetUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform homeRect;
    [SerializeField] private TMP_Text popupText;

    [Header("Settings")]
    [SerializeField] private float popupDuration = 1.25f;

    private bool delivered = false;
    private Coroutine popupCo;

    void Reset()
    {
        homeRect = GetComponent<RectTransform>();
        if (!popupText)
        {
            var tmp = GetComponentInChildren<TMP_Text>(true);
            if (tmp) popupText = tmp;
        }
    }

    public bool IsDelivered => delivered;

    public bool TryDeliver(Vector2 playerAnchoredPos, float distanceThreshold)
    {
        if (delivered || homeRect == null) return false;
        float dist = Vector2.Distance(playerAnchoredPos, homeRect.anchoredPosition);
        if (dist <= distanceThreshold)
        {
            delivered = true;
            ShowPopup();
            return true;
        }
        return false;
    }

    private void ShowPopup()
    {
        if (!popupText) return;
        popupText.gameObject.SetActive(true);
        if (popupCo != null) StopCoroutine(popupCo);
        popupCo = StartCoroutine(HidePopupAfter());
    }

    private IEnumerator HidePopupAfter()
    {
        yield return new WaitForSecondsRealtime(popupDuration);
        if (popupText) popupText.gameObject.SetActive(false);
        popupCo = null;
    }
}
