using UnityEngine;
using TMPro;

public class LoadingIndicator : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI loadingText;

    private int activeLoads = 0;

    private void Awake()
    {
        if (loadingText != null)
            loadingText.gameObject.SetActive(false);
    }

    public void ShowLoading(string zoneName)
    {
        activeLoads++;
        if (loadingText != null)
        {
            loadingText.text = $"Loading {zoneName}...";
            loadingText.gameObject.SetActive(true);
        }
    }

    public void HideLoading()
    {
        activeLoads = Mathf.Max(0, activeLoads - 1);
        if (activeLoads == 0 && loadingText != null)
            loadingText.gameObject.SetActive(false);
    }
}
