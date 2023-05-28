using System;
using UnityEngine;

public class CameraFocusObject : MonoBehaviour {
    [SerializeField] private float objectToLookAtSpeed = 10;
    [SerializeField] private float upDownSpeed = 5;

    private Vector3? targetPoint;
    private Vector3 initialPosition;

    private void Awake() {
        initialPosition = transform.position;
    }

    public void UpdateTargetPoint(Vector3 point) {
        targetPoint = point;
    }

    private void Update() {
        if (targetPoint == null) return;

        if (Mathf.Abs(targetPoint.Value.x - transform.position.x) > .1f) {
            transform.Translate((targetPoint.Value - transform.position).normalized * objectToLookAtSpeed * Time.deltaTime);
            if (Mathf.Abs(targetPoint.Value.x - transform.position.x) <= .1f) {
                transform.position = targetPoint.Value;
            }
        }

        if (Input.GetKey(KeyCode.W)) {
            Vector3 newValue = transform.position + Vector3.up * upDownSpeed * Time.deltaTime;
            newValue.y = Mathf.Clamp(newValue.y, Ctx.Deps.CameraController.HeightClamp.x, Ctx.Deps.CameraController.HeightClamp.y);
            transform.position = newValue;
        }

        if (Input.GetKey(KeyCode.S)) {
            Vector3 newValue = transform.position - Vector3.up * upDownSpeed * Time.deltaTime;
            newValue.y = Mathf.Clamp(newValue.y, Ctx.Deps.CameraController.HeightClamp.x, Ctx.Deps.CameraController.HeightClamp.y);
            transform.position = newValue;
        }
    }

    public void Clear() {
        targetPoint = initialPosition;
    }
}