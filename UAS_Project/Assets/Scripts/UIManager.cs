using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMPro.TextMeshProUGUI titleText;
    [SerializeField] private TMPro.TextMeshProUGUI bodyText;
    [SerializeField] private GameObject pauseMenuUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        pauseMenuUI.SetActive(false);
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

    void Update()
    {
        if (GameManager.currentState == GameState.Playing)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("[UIManager] Escape pressed while Playing. State: " + GameManager.currentState);
                PauseGame();
            }
        }
        else if (GameManager.currentState == GameState.Paused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("[UIManager] Escape pressed while Paused. State: " + GameManager.currentState);
                ResumeGame();
            }
        }
    }

    public void PauseGame()
    {
        Debug.Log("[UIManager] PauseGame called. Enabling pause UI.");
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameManager.currentState = GameState.Paused;
        if (Player.Instance != null) Player.Instance.moveable = false;
    }

    public void ResumeGame()
    {
        Debug.Log("[UIManager] ResumeGame called. Hiding pause UI.");
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.currentState = GameState.Playing;
        if (Player.Instance != null) Player.Instance.moveable = true;
    }
}
