using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {

    protected static T m_Instance;

    public static T Instance {
        get {
            if (m_Instance == null) {
                m_Instance = (T)FindObjectOfType(typeof(T));
                if (m_Instance == null) {
                    CreateInstance();
                }
            }
            return m_Instance;
        }
    }

    private static void CreateInstance() {
        var go = new GameObject(typeof(T).Name);
        m_Instance = go.AddComponent<T>();

#if UNITY_EDITOR
        if (Application.isPlaying) {
            GameObject.DontDestroyOnLoad(go);
        } else {
            go.hideFlags = HideFlags.HideAndDontSave;
            EditorApplication.playModeStateChanged += (state) => {
                if (m_Instance != null) {
                    GameObject.DestroyImmediate(go);
                    m_Instance = null;
                }
            };
        }
#else
        GameObject.DontDestroyOnLoad(go);
#endif
    }
}

public class Singleton<T> where T : class, new() {

    protected static T m_Instance;
    private static object m_Lock = new object();

    public static T Instance {
        get {
            if (m_Instance == null) {
                lock (m_Lock) {
                    if (m_Instance == null) {
                        m_Instance = new T();
                    }
                }
            }
            return m_Instance;
        }
    }

    public virtual void OnEnable() { }


    public virtual void OnDisable() {
        m_Instance = null;
    }
}
