
using Cysharp.Threading.Tasks;
using SnakeGame.Debuging;
using SnakeGame.GameUtilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame.ObsoleteClasses
{
    /// <summary>
    /// Is just used to pool snake segments components.
    /// </summary>
    [DisallowMultipleComponent]
    [Obsolete]
    public class SnakePool : MonoBehaviour
    {
        [SerializeField] private List<SegmentPool> Segments = new();

        private readonly Dictionary<int, Queue<Component>> poolDictionary = new();

        // Start is called before the first frame update
        //void Start()
        //{
        //    for (int i = 0; i < Segments.Count; i++)
        //    {
        //        CreateSegment(Segments[i].PrefabToUse, Segments[i].PoolSize);
        //    }
        //}

        public async UniTask Init()
        {
            for (int i = 0; i < Segments.Count; i++)
            {
                CreateSegment(Segments[i].PrefabToUse, Segments[i].PoolSize, Segments[i].ComponentType);
            }
            await UniTask.Delay(1000);
        }

        private void CreateSegment(GameObject prefabToUse, int poolSize, string componentType)
        {
            int poolKey = prefabToUse.GetInstanceID();
            // Gets the name of the prefab
            string prefabName = prefabToUse.name;

            // Creates the parent gameobject to attached the child gameobjects to
            GameObject parentGameObject = new(prefabName + " Anchor");
            parentGameObject.transform.SetParent(transform);

            if (!poolDictionary.ContainsKey(poolKey))
            {
                poolDictionary.Add(poolKey, new Queue<Component>());

                for (int i = 0; i < poolSize; i++)
                {
                    GameObject newObject = Instantiate(prefabToUse, parentGameObject.transform);

                    newObject.SetActive(false);

                    poolDictionary[poolKey].Enqueue(newObject.GetComponent(Type.GetType(componentType)));
                }
            }
        }

        public Component ReuseSegment(GameObject prefabToUse, Vector3 position, Quaternion rotation = default)
        {
            int poolKey = prefabToUse.GetInstanceID();

            if (poolDictionary.ContainsKey(poolKey))
            {
                //Get the object from the pool queue
                Component componentToReuse = GetComponentFromPool(poolKey);

                ResetObject(position, rotation, componentToReuse, prefabToUse);

                return componentToReuse;
            }
            else
            {
                this.LogError($"No Object Pool For {prefabToUse}");
                return null;
            }
        }

        /// <summary>
        /// Gets A Gameobject From The Pool using The 'poolKey'
        /// </summary>
        private Component GetComponentFromPool(int poolKey)
        {
            Component componentToReuse = poolDictionary[poolKey].Dequeue();
            poolDictionary[poolKey].Enqueue(componentToReuse);

            if (componentToReuse.gameObject.activeSelf == true)
                componentToReuse.gameObject.SetActive(false);

            return componentToReuse;
        }

        /// <summary>
        /// Resets The GameObject
        /// </summary>
        private void ResetObject(Vector3 position, Quaternion rotation, Component componentToReuse, GameObject prefabToUse)
        {
            componentToReuse.transform.SetPositionAndRotation(position, rotation);
            componentToReuse.gameObject.transform.localScale = prefabToUse.transform.localScale;
        }

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(Segments), Segments);
        }
#endif
        #endregion

        [Serializable]
        private struct SegmentPool
        {
            [Tooltip("The pool size this object will have, the number of gameobjects created in the pool" +
                " will be defined by the pool size")]
            public int PoolSize;
            [Tooltip("The prefab to instantiate in the pool")]
            public GameObject PrefabToUse;
            public string ComponentType;
        }
    }
}
