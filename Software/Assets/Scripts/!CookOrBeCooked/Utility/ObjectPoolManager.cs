using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookOrBeCooked.Utility;
using Unity.VisualScripting;
using UnityEngine;

namespace CookOrBeCooked.Utility.ObjectPool
{   
    /// <summary>
    /// Manager for object pooling in the scene.
    /// </summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        // Object pool map, whereby the keys are the prefabs of the objects that are to be pooled and the value being the pools themselves
        private Dictionary<GameObject, List<GameObject>> _objectPools = new Dictionary<GameObject, List<GameObject>>();

        protected override void Awake()
        {
            base.Awake();

            InitialisePools();
            PrintPoolsInfo(false);
        }

        public GameObject GetGameObjectFromPool(IObjectPoolUser objectPoolUser, GameObject prefab)
        {
            if (!_objectPools.ContainsKey(prefab))
            {
                Debug.LogWarning($"Object Pool for {prefab} was not initialised. Expanding pool now");
                IObjectPoolUser[] _users = { objectPoolUser };
                InitialisePools(_users);
            }

            if (_objectPools.ContainsKey(prefab))
            {
                var pool = _objectPools[prefab];
                for (int i = 0; i < pool.Count; i++)
                {
                    if (pool[i] == null)
                        Debug.LogWarning("An object from the object pool was destroyed from outside of the object pool manager");
                    else if (!pool[i].activeInHierarchy)
                    {
                        return pool[i];
                    }
                }
                if (objectPoolUser.IsPoolExpandable)
                {
                    // Expand the pool.   // NOTE: may need to use InitialisePools and recursively expand the pool.
                    GameObject newGameObject = Object.Instantiate(prefab, objectPoolUser.PoolParent == null ? transform : objectPoolUser.PoolParent);
                    newGameObject.SetActive(false);
                    _objectPools[prefab].Add(newGameObject);
                    Debug.LogFormat("Expanded Pool for {0} to {1}", prefab.name, _objectPools[prefab].Count);
                    return newGameObject;
                }
            }

            return null;
        }

        public GameObject GetGameObjectFromPool(IObjectPoolUser objectPoolUser, GameObject prefab, Transform parent, bool setActive = false, bool worldPositionStays = false)
        {
            GameObject objectFromPool = GetGameObjectFromPool(objectPoolUser, prefab);
            if (objectFromPool != null)
            {
                objectFromPool.transform.SetParent(parent != null ? parent : transform, worldPositionStays);
                objectFromPool.SetActive(setActive);
            }
            return objectFromPool;
        }

        public GameObject GetGameObjectFromPool(IObjectPoolUser objectPoolUser, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, bool setActive = false)
        {
            GameObject objectFromPool = GetGameObjectFromPool(objectPoolUser, prefab);
            if (objectFromPool != null)
            {
                objectFromPool.transform.SetParent(parent != null ? parent : transform, false);
                objectFromPool.transform.position = position;
                objectFromPool.transform.rotation = rotation;
                objectFromPool.SetActive(setActive);
            }
            return objectFromPool;
        }

        private void InitialisePools()
        {
            // Get list of Object Pool Users
            IObjectPoolUser[] objectPoolUsers = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IObjectPoolUser>().ToArray();

            // Initialise pool
            InitialisePools(objectPoolUsers);
        }

        private void InitialisePoolsRecursive(GameObject obj)
        {
            // Get list of Object Pool Users
            IObjectPoolUser[] objectPoolUsers = obj.GetComponentsInChildren<MonoBehaviour>(true).OfType<IObjectPoolUser>().ToArray();

            // Initialise pool
            if (objectPoolUsers.Length > 0)
                InitialisePools(objectPoolUsers);
        }

        private void InitialisePools(IObjectPoolUser[] objectPoolUsers)
        {
            // Initialise pool
            foreach (IObjectPoolUser objectPoolUser in objectPoolUsers)
            {
                for (int i = 0; i < objectPoolUser.ObjectsToPool.Length; i++)
                {
                    // Check if the key is null. Not allowed to pool null object reference
                    if (objectPoolUser.ObjectsToPool[i] == null)
                    {
                        Debug.LogWarning("Attempted to pool null object from " + objectPoolUser);
                        continue;
                    }

                    // Check if key exists. Else, add key
                    if (!_objectPools.ContainsKey(objectPoolUser.ObjectsToPool[i]))
                    {
                        _objectPools.Add(objectPoolUser.ObjectsToPool[i], new List<GameObject>());
                    }

                    // Add object quantities to pool
                    for (int j = 0; j < objectPoolUser.PoolQuantitiesRequired[i]; j++)
                    {
                        GameObject newGameObject = Object.Instantiate(objectPoolUser.ObjectsToPool[i], objectPoolUser.PoolParent == null ? transform : objectPoolUser.PoolParent);
                        newGameObject.SetActive(false);
                        _objectPools[objectPoolUser.ObjectsToPool[i]].Add(newGameObject);

                        // Recursively initialise pool
                        InitialisePoolsRecursive(newGameObject);
                    }
                }
            }
        }

        public void PrintPoolsInfo(bool printBreakdown)
        {
            Debug.LogFormat("Number of Pools:{0}", _objectPools.Count);
            if (printBreakdown)
            {
                foreach (var keyvalue in _objectPools)
                {
                    Debug.LogFormat("{0} has pool size of: {1}", keyvalue.Key.name, keyvalue.Value.Count);
                }
            }
        }

        public void ReturnToPool(GameObject objectToReturn, Transform parent = null)
        {
            // Default to ObjectPoolManager's child
            if (parent == null)
                parent = transform;

            if (objectToReturn.transform.parent != parent)
            {
                objectToReturn.transform.SetParent(parent, false);
            }

            if (objectToReturn.activeSelf)
                objectToReturn.SetActive(false);
        }

        public void DestroyPooledObject(GameObject prefab, GameObject objectToDestroy)
        {
            _objectPools[prefab].Remove(objectToDestroy);
            Destroy(objectToDestroy);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var pool in _objectPools.Values)
            {
                foreach (GameObject gameObject in pool)
                {
                    Destroy(gameObject);
                }
            }
        }    
    }
}
