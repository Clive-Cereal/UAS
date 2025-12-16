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


    private void Awake() 
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] Scene loaded: {scene.name}. Target: {targetScene}. Setting state to targetState ({targetState}).");
        if (!string.IsNullOrEmpty(targetScene) && scene.name == targetScene)
        {
            currentState = targetState;
        }
    }

    void Update()
    {
        Initialise();
        Debug.Log("[GameManager] Current State: " + currentState);
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
