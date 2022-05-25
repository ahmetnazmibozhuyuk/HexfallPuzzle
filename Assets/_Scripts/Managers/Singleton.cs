using UnityEngine;

namespace Hexfall.Managers
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        public static T instance { get; private set; }
        protected virtual void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            //DontDestroyOnLoad(gameObject);
            instance = this as T;
        }
    }
}