using System.Collections;
using Pooling;
using UnityEngine;

public class Projectile : PooledObject {
    [SerializeField] private float secondsToDestroy = 5;

    private Coroutine destroyRoutine;
    private Rigidbody rig;

    private void Awake() {
        destroyRoutine = StartCoroutine(ReturnToPoolDelayed());
        Ctx.Deps.EventsManager.GameRestarted += OnGameRestarted;
        rig = GetComponent<Rigidbody>();
    }

    public void Fly(Vector3 force) {
        rig.AddForce(force, ForceMode.Impulse);
    }

    private void OnGameRestarted() {
        StopCoroutine(destroyRoutine);
        Clear();
    }

    private IEnumerator ReturnToPoolDelayed() {
        yield return new WaitForSeconds(secondsToDestroy);
        Clear();
    }

    private void Clear() {
        rig.velocity = Vector3.zero;
        ReturnToPool();
    }
}