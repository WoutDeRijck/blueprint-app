using UnityEngine;

namespace Misc
{

    /// <summary>
    /// Acts as a singleton, but replaces the instance when a new one is created instead of destroying it.
    /// </summary>
    public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }
        protected virtual void Awake() => Instance = this as T;

        protected virtual void OnApplicationQuit()
        {
            Instance = null;
            Destroy(gameObject);
        }

    }

    /// <summary>
    /// Basic singleton, destroys itself when created if there is already an instance.
    /// </summary>
    public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            base.Awake();
        }
    }

    /// <summary>
    /// Singleton but persistent across scenes.
    /// </summary>
    public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}