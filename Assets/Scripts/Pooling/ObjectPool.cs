using System.Collections.Generic;
using UnityEngine;

namespace Pooling {
    public class ObjectPool : MonoBehaviour {
        PooledObject prefab;

        private readonly List<PooledObject> availableObjects = new();

        public static ObjectPool GetPool(PooledObject prefab) {
            GameObject obj;
            ObjectPool pool;

            obj = new GameObject(prefab.name + " Pool");
            pool = obj.AddComponent<ObjectPool>();
            pool.prefab = prefab;
            return pool;
        }

        public PooledObject GetObject(Transform parent) {
            PooledObject obj;
            int lastAvailableIndex = availableObjects.Count - 1;
            if (lastAvailableIndex >= 0) {
                obj = availableObjects[lastAvailableIndex];
                availableObjects.RemoveAt(lastAvailableIndex);
                obj.transform.SetParent(parent);
                obj.gameObject.SetActive(true);
            } else {
                obj = Instantiate(prefab, parent, false);
                obj.Pool = this;
            }

            return obj;
        }

        public void AddObject(PooledObject obj) {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(transform);
            availableObjects.Add(obj);
        }
    }
}