using UnityEngine;

namespace Pooling {
    public class PooledObject : MonoBehaviour {
        [System.NonSerialized]
        private ObjectPool poolInstanceForPrefab;

        public T GetPooledInstance<T>(Transform parent) where T : PooledObject {
            if (!poolInstanceForPrefab) {
                poolInstanceForPrefab = ObjectPool.GetPool(this);
            }

            return (T) poolInstanceForPrefab.GetObject(parent);
        }

        public ObjectPool Pool { get; set; }

        protected void ReturnToPool() {
            if (Pool) {
                Pool.AddObject(this);
            } else {
                Destroy(gameObject);
            }
        }
    }
}