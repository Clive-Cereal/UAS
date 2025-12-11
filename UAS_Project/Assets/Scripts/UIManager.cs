using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMPro.TextMeshProUGUI titleText;
    [SerializeField] private TMPro.TextMeshProUGUI bodyText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowInfo(InfoEntry entry)
    {
        if (entry == null) return;

        titleText.text = entry.title;
        bodyText.text = entry.description;

        panel.SetActive(true);
    }

    public void HideInfo()
    {
        panel.SetActive(false);
    }
}
