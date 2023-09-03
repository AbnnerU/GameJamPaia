using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManagerBase<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected void Setup(T instance)
    {
        if (Instance == null)
        {
            Instance = instance;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != instance)
        {
            Destroy(gameObject);
        }
    }
}
