using UnityEngine;

public class MonoBehaviourSingletonPersistent<T> : MonoBehaviour
    where T : Component
{
    public static T Instance { get; private set; }

    public virtual void Awake()
    {
        if (Instance == null) 
        {
            Instance = this as T;
            DontDestroyOnLoad(this);
            OnInstanceSet();
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnInstanceSet()
    {
    }
}

// Some known limitations with MonoBehaviourSingletonPersistent class:
//      1) Due to this object being persistent between scenes, when switching between
//      them, all MonoBehaviour methods will be invoked on the new object before it
//      is destroyed. 
//
//      Usually this is fine, but if you assign values to some static
//      variables (I have no clue why you would ever do that in a singleton,
//      but whatever) in the awake method, keep this comment in mind.
//      2) It's a singleton. It sucks.

public class MonoBehaviourGetterSingleton<T> : MonoBehaviour
    where T : Component
{
    private static T _instance;
    public static T Instance
    {
        get {
            if (_instance == null) 
            {
                var objs = FindObjectsOfType(typeof(T)) as T[];
                if (objs.Length > 0)
                    _instance = objs[0];
                if (objs.Length > 1) 
                {
                    Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                }
                if (_instance == null) 
                {
                    GameObject obj = new GameObject();
                    //obj.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    public static bool HasInstance()
    {
        return _instance != null;
    }
}