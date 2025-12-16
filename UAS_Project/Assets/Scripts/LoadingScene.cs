using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScene : MonoBehaviour
{
    private void Start()
    {
        GameManager.currentState = GameState.Loading;
        StartCoroutine(LoadSceneWithLoadingScreen());
    }

    private IEnumerator LoadSceneWithLoadingScreen()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(GameManager.targetScene);

        op.allowSceneActivation = false;

        float timer = 0f;

        while (timer < 2f) //This makes fake loading time for set amount
        {
            timer += Time.deltaTime;
            yield return null;
        }

        op.allowSceneActivation = true; //Then the scene is activated here

        while (!op.isDone) yield return null;

        GameManager.currentState = GameManager.targetState;
    }
}
