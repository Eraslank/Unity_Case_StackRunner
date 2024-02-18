using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : Component
{
    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {
        _instance = null;
    }

    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var objs = FindObjectsOfType(typeof(T)) as T[];
                if (objs.Length > 0)
                    _instance = objs[0];
                if (objs.Length > 1)
                {
                    Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                }
            }
            return _instance;
        }
    }
}


public class MonoBehaviourSingletonPersistent<T> : MonoBehaviour where T : Component
{
    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {
        Instance = null;
    }
    public static T Instance { get; private set; }

    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}