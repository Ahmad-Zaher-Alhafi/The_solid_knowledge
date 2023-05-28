using System;
using UnityEngine;

public class Shooter : MonoBehaviour {
    [SerializeField] private Transform projectilesHolder;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float shootForce;

    private Ray mouseRay;
    private bool physicsActivated;

    private void Start() {
        Ctx.Deps.EventsManager.PhysicsActivated += OnPhysicsActivated;
        Ctx.Deps.EventsManager.GameRestarted += OnGameRestarted;
    }

    private void OnGameRestarted() {
        physicsActivated = false;
    }

    private void OnPhysicsActivated() {
        physicsActivated = true;
    }

    private void Update() {
        if (!physicsActivated) return;

        mouseRay = Ctx.Deps.CameraController.Camera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonUp(0)) {
            Shoot();
        }
    }

    private void Shoot() {
        if (!Physics.Raycast(mouseRay, out RaycastHit raycastHit, Mathf.Infinity, ~LayerMask.GetMask("Projectile"))) return;

        Projectile projectile = projectilePrefab.GetPooledInstance<Projectile>(projectilesHolder);
        projectile.transform.position = Ctx.Deps.CameraController.Camera.transform.position + Ctx.Deps.CameraController.Camera.transform.forward;
        Vector3 direction = (raycastHit.point - projectile.transform.position).normalized;
        projectile.Fly(direction * shootForce);
    }

    private void OnDestroy() {
        Ctx.Deps.EventsManager.PhysicsActivated -= OnPhysicsActivated;
        Ctx.Deps.EventsManager.GameRestarted -= OnGameRestarted;
    }
}