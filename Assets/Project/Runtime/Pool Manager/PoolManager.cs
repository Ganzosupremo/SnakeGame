using SnakeGame.Debuging;
using SnakeGame.GameUtilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SnakeGame
{
    [DisallowMultipleComponent]
    public class PoolManager : SingletonMonoBehaviour<PoolManager>
    {
        #region Tooltip
        [Tooltip("This array is populated with the prefabs that are gonna be added to the pool, also specify the number of gameobjects to be created for each prefab")]
        #endregion
        public PoolObject[] poolArray = null;
        private Transform objectPoolTransform;

        private readonly Dictionary<int, Queue<Component>> poolDictionary = new();
        private readonly Dictionary<int, int> _dynamicPoolDictionary = new();

        [Serializable]
        public struct PoolObject
        {
            public string name;

            [Tooltip("The pool size this object will have, the number of gameobjects created in the pool" +
                " will be defined by the pool size")]
            public int PoolSize;
            
            [Tooltip("Number of additional objects to create dynamically when needed.")]
            public int DynamicPoolSize;

            [Tooltip("The prefab to instantiate in the pool")]
            public GameObject prefabToUse;
            
            [Tooltip("Type exactly the type of component that the prefab is," +
                "otherwise it won't work. If the component it's on a namespace " +
                "also include the namespace.")]
            public string componentType;
        }

        protected override void Awake()
        {
            base.Awake();
            // This singleton gameobject will be the parent pool gameobject
            objectPoolTransform = this.gameObject.transform;

            for (int i = 0; i < poolArray.Length; i++)
            {
                CreatePool(poolArray[i].prefabToUse, poolArray[i].DynamicPoolSize, poolArray[i].PoolSize, poolArray[i].componentType);
            }
        }

        /// <summary>
        /// Creates The Object Pool With The Specified Prefabs, The PoolSize And the Type Of Component For Each One
        /// </summary>
        private void CreatePool(GameObject prefabToUse, int dynamicSize, int poolSize, string componentType)
        {
            int poolKey = prefabToUse.GetInstanceID();
            // Gets the name of the prefab
            string prefabName = prefabToUse.name;

            // Creates the parent gameobject to attach the child gameobjects to
            GameObject parentGameObject = new(prefabName + " Anchor");
            parentGameObject.transform.SetParent(objectPoolTransform);

            if (!poolDictionary.ContainsKey(poolKey))
            {
                poolDictionary.Add(poolKey, new Queue<Component>());

                // Create the initial static pool
                for (int i = 0; i < poolSize; i++)
                {
                    GameObject newObject = Instantiate(prefabToUse, parentGameObject.transform);
                    newObject.SetActive(false);

                    poolDictionary[poolKey].Enqueue(newObject.GetComponent(Type.GetType(componentType)));
                }

                // Store the dynamic pool size for later use
                _dynamicPoolDictionary.Add(poolKey, dynamicSize);
            }



            //int poolKey = prefabToUse.GetInstanceID();
            //// Gets the name of the prefab
            //string prefabName = prefabToUse.name; 

            //// Creates the parent gameobject to attached the child gameobjects to
            //GameObject parentGameObject = new(prefabName + " Anchor"); 
            //parentGameObject.transform.SetParent(objectPoolTransform);

            //if (!poolDictionary.ContainsKey(poolKey))
            //{
            //    poolDictionary.Add(poolKey, new Queue<Component>());

            //    for (int i = 0; i < poolSize; i++)
            //    {
            //        GameObject newObject = Instantiate(prefabToUse, parentGameObject.transform);

            //        newObject.SetActive(false);

            //        poolDictionary[poolKey].Enqueue(newObject.GetComponent(Type.GetType(componentType)));
            //    }
            //}
        }

        /// <summary>
        /// Reuses A Game Object Contained In The Pool.
        /// </summary>
        /// <param name="prefabToUse">The Prefab Containing The Component.</param>
        /// <param name="position">The World Position Where The Gameobject Should Appear When Enabled.</param>
        /// <param name="rotation">Should Be Set If The Component Needs A Rotation.</param>
        /// <returns></returns>
        public Component ReuseComponent(GameObject prefabToUse, Vector3 position, Quaternion rotation = default)
        {
            int poolKey = prefabToUse.GetInstanceID();

            if (poolDictionary.ContainsKey(poolKey))
            {
                // Get the object from the pool queue
                Component componentToReuse = GetComponentFromPool(poolKey);

                ResetObject(position, rotation, componentToReuse, prefabToUse);

                return componentToReuse;
            }
            else
            {
                // Check if the dynamic pool size is defined for this prefab
                foreach (PoolObject poolObject in poolArray)
                {
                    if (poolObject.prefabToUse == prefabToUse)
                    {
                        CreatePool(prefabToUse, poolObject.DynamicPoolSize, poolObject.DynamicPoolSize, poolObject.componentType);
                        return ReuseComponent(prefabToUse, position, rotation); // Retry the reuse after creating the dynamic pool.
                    }
                }

                this.LogError($"No Object Pool For {prefabToUse}");
                return null;
            }


            //int poolKey = prefabToUse.GetInstanceID();

            //if (!poolDictionary.ContainsKey(poolKey))
            //{
            //    ExpandPool(prefabToUse, 20); // You can adjust the count of new objects to create here.
            //}

            //if (poolDictionary.TryGetValue(poolKey, out Queue<Component> poolQueue))
            //{
            //    // Get the object from the pool queue
            //    Component componentToReuse = poolQueue.Dequeue();
            //    poolQueue.Enqueue(componentToReuse);

            //    ResetObject(position, rotation, componentToReuse, prefabToUse);

            //    return componentToReuse;
            //}
            //else
            //{
            //    this.LogError($"No Object Pool For {prefabToUse}");
            //    return null;
            //}

            //if (poolDictionary.ContainsKey(poolKey))
            //{
            //    //Get the object from the pool queue
            //    Component componentToReuse = GetComponentFromPool(poolKey);

            //    ResetObject(position, rotation, componentToReuse, prefabToUse);

            //    return componentToReuse;
            //}
            //else
            //{
            //    this.LogError($"No Object Pool For {prefabToUse}");
            //    return null;
            //}
        }

        /// <summary>
        /// Gets A Gameobject From The Pool using The 'poolKey'
        /// </summary>
        private Component GetComponentFromPool(int poolKey)
        {
            Component componentToReuse = poolDictionary[poolKey].Dequeue();
            poolDictionary[poolKey].Enqueue(componentToReuse);

            if (componentToReuse.gameObject.activeSelf == true)
            {
                componentToReuse.gameObject.SetActive(false);
            }

            return componentToReuse;
        }

        //private void ExpandPool(GameObject prefabToUse, int count)
        //{
        //    int poolKey = prefabToUse.GetInstanceID();
        //    string prefabName = prefabToUse.name;
        //    // Creates the parent gameobject to attached the child gameobjects to
        //    GameObject parentGameObject = new(prefabName + " Anchor");
        //    parentGameObject.transform.SetParent(objectPoolTransform);

        //    if (!poolDictionary.ContainsKey(poolKey))
        //    {
        //        poolDictionary.Add(poolKey, new Queue<Component>());
        //    }

        //    for (int i = 0; i < count; i++)
        //    {
        //        GameObject newObject = Instantiate(prefabToUse, parentGameObject.transform);
        //        newObject.SetActive(false);

        //        poolDictionary[poolKey].Enqueue(newObject.GetComponent(Type.GetType(poolArray[i].componentType)));
        //    }
        //}

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
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(poolArray), poolArray);
        }
#endif
        #endregion
    }
}
