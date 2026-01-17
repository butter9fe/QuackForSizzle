
using UnityEngine;

namespace CookOrBeCooked.Utility.ObjectPool
{

    /// <summary>
    /// Interface to implement for MonoBehaviours to define the object pools for objects required at runtime.
    /// </summary>
    public interface IObjectPoolUser
    {
        /// <summary>
        /// List of objects that this user requires pooling for.
        /// </summary>
        public GameObject[] ObjectsToPool { get; }

        /// <summary>
        /// List of quantities this user needs for each respective pooled object.
        /// </summary>
        public int[] PoolQuantitiesRequired { get; }

        /// <summary>
        /// Parent Transform for this user's objects in the pool. If null, it will be parented to the ObjectPoolManager.
        /// </summary>
        public Transform PoolParent { get; }

        /// <summary>
        /// Whether the pool for this user's spawns should be expandabl e. Expansion occurs when there are no longer any items in the pool when requested.
        /// </summary>
        public bool IsPoolExpandable { get; }
    }
}
