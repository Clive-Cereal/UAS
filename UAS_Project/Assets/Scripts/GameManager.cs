using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    [HideInInspector] public static GameState currentState = GameState.Init;
    public static GameState currentGameState => currentState;
    public static string targetScene;
    public static GameState targetState;
    [SerializeField] private GameObject pauseMenuUI;


    private void Awake() 
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        Initialise();
        if(Input.GetKeyDown(KeyCode.Escape) && currentState == GameState.Playing)
        {
            pauseMenuUI.SetActive(!pauseMenuUI.activeSelf);
            if(pauseMenuUI.activeSelf)
            {
                currentState = GameState.Paused;
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                currentState = GameState.Playing;
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

//---------------------------------------------------------------------

    void Initialise()
    {
        if(currentState == GameState.Init)
        {
            SceneLoader("00_Start", GameState.Menu);
        }
    }

    public void SceneLoader(string sceneName, GameState stateName) //To use this : eg. GameManager.Instance.SceneLoader("desiredscenename", GameState.desiredstate);
    {
        targetScene = sceneName;
        targetState = stateName;

        SceneManager.LoadScene("_Loading");
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1f;
        SceneLoader("00_Start", GameState.Menu);
    }
//-------------------FOR UI BUTTONS------------------------------------

    public void Button_StartGame()
    {
        SceneLoader("01_Main", GameState.Playing);
    }

//---------------------------------------------------------------------
    public void ExitGame()
    {
        Application.Quit();
    }
}
