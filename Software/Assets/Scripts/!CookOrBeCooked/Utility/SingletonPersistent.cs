using UnityEngine;
using System;

namespace CookOrBeCooked.Utility
{
    // Custom abstract class for creating singletons
    public abstract class SingletonPersistent<T> : MonoBehaviour where T : SingletonPersistent<T>
    {
        private static T _singleton = null;
        private static bool _singletonWasCreated = false;

        protected static T Instance
        {
            get
            {
                if (!_singleton && !_singletonWasCreated)
                {
                    // Create persistent game object that contains singleton. Set active to false first
                    // so as to set _singleton reference so that Awake() function would work properly
                    GameObject persistentGO = new GameObject(typeof(T).FullName + " [Persistent Object]");
                    persistentGO.SetActive(false);
                    _singleton = persistentGO.AddComponent<T>();
                    persistentGO.SetActive(true);

                    DontDestroyOnLoad(persistentGO);

                    // Initialise
                    _singleton.Init();

                    // Set creation flag so that when application is closing, and the Singleton is destroyed
                    // before subscribers can finish unsubscribing, a null reference would be returned rather than
                    // creating a new Singleton. The null reference is handled properly in functions.
                    _singletonWasCreated = true;
                }

                return _singleton;
            }
        }

        /// <summary>
        /// Check if this component is the same one as singleton. Happens when the scene has a game object with a event manager script.
        /// </summary>
        void Awake()
        {
            if (_singleton != this)
            {
                Debug.LogError($"Game object that contains {typeof(T).FullName} script exist in scene. Note that there's no need for it, the game object {this.gameObject.name} is disabled");
                this.gameObject.SetActive(false);
            }
        }

        public abstract void Init();
    }
}