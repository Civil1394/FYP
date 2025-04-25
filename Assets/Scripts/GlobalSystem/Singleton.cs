using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private class MonoSingletonInternal : SingletonBase<MonoSingletonInternal>
    {
        private T _monoInstance;
        public T MonoInstance
        {
            get => _monoInstance;
            set => _monoInstance = value;
        }
    }

    private static readonly object _lock = new object();
    
    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                var internal_instance = MonoSingletonInternal.Instance;
                if (internal_instance.MonoInstance == null)
                {
                    internal_instance.MonoInstance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError($"[Singleton_{typeof(T)}] Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
                        return internal_instance.MonoInstance;
                    }

                    if (internal_instance.MonoInstance == null)
                    {
                        GameObject singleton = new GameObject();
                        internal_instance.MonoInstance = singleton.AddComponent<T>();
                        singleton.name = $"(singleton) {typeof(T)}";

                        DontDestroyOnLoad(singleton);

                        Debug.Log($"[Singleton] An instance of {typeof(T)} is needed in the scene, so '{singleton}' was created with DontDestroyOnLoad.");
                    }
                    else
                    {
                        Debug.Log($"[Singleton] Using instance already created: {internal_instance.MonoInstance.gameObject.name}");
                    }
                }

                return internal_instance.MonoInstance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (MonoSingletonInternal.Instance.MonoInstance == null)
        {
            MonoSingletonInternal.Instance.MonoInstance = this as T;
            //DontDestroyOnLoad(gameObject);
        }
        else if (MonoSingletonInternal.Instance.MonoInstance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (MonoSingletonInternal.Instance.MonoInstance == this)
        {
            MonoSingletonInternal.Instance.MonoInstance = null;
        }
    }
}