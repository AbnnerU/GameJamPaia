
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private LoadLevelData levelData;

    [SerializeField] private string loadingScene = "loading";

    public void LoadScene(string sceneName)
    {
        levelData.levelToLoad = sceneName;
        SceneManager.LoadScene(loadingScene);

        //SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    //private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    //{

    //}
}
