using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnStartSceneLoad : MonoBehaviour
{
    [SerializeField]private SceneLoadManager sceneLoadManager;
    [SerializeField] private string sceneToLoad;

    private void Start()
    {
        sceneLoadManager.LoadScene(sceneToLoad);
    }
}
