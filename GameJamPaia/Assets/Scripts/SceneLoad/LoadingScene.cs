using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private LoadLevelData levelData;

    [SerializeField] private bool playOnStart=true;

    private void Start()
    {
        if (playOnStart)
            LoadLevel();
        
        
    }

    public void LoadLevel()
    {
        StartCoroutine(LoadLevelAsync());
    }

    public void LoadLevelWhitDelay(float delayValue)
    {
        StartCoroutine(LoadLevelAsyncWhitDelay(delayValue));
    }

 
     IEnumerator LoadLevelAsync()
    {

        AsyncOperation operation = SceneManager.LoadSceneAsync(levelData.levelToLoad);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            yield return null;
        }
    }

    IEnumerator LoadLevelAsyncWhitDelay(float delay)
    {
        print("Delay");
        yield return new WaitForSeconds(delay);
     
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelData.levelToLoad);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            yield return null;
        }
    }
}
