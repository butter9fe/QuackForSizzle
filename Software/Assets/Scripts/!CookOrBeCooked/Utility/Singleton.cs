using UnityEngine;
using System;

namespace CookOrBeCooked.Utility
{
    // Custom abstract class for creating singletons
    [DefaultExecutionOrder(-99)]
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }

        private void InitialiseSingleton()
        {
            // Singleton instance
            if (Instance == null)
                Instance = (T)this;
            else
            {
                Debug.Log(typeof(T) + " Instance already assigned to " + Instance.name + ", deleting duplicate instance in " + gameObject.name);
                Destroy(gameObject);
                return;
            }
        }

        protected virtual void Awake()
        {
            InitialiseSingleton();
        }

        protected virtual void OnDestroy()
        {
            // Clear Singleton instance
            if (Instance == this)
                Instance = null;
        }
    }
}