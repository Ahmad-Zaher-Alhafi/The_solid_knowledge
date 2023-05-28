using Cinemachine;
using UnityEngine;

namespace Controllers {
    public class CameraController : MonoBehaviour, IController {
        [SerializeField] private CameraFocusObject objectToLookAt;
        [SerializeField] private CinemachineFreeLook freeLookCamera;
        [SerializeField] private float zoom = 5;

        [SerializeField] private float zoomSpeed = 3;
        [SerializeField] private Vector2 zoomClamp;

        public Camera Camera => Camera.main;
        public Vector2 HeightClamp => heightClamp;
        private Vector2 heightClamp;

        private int shownStackIndex;
        private int numOfStacks;
        private float zoomValue;

        private void Start() {
            Ctx.Deps.EventsManager.StacksSpawned += OnStacksSpawned;
        }

        private void OnStacksSpawned() {
            numOfStacks = Ctx.Deps.BlocksSpawnController.StacksCount;
            FocusOnStack(0);
        }

        private void Update() {
            if (numOfStacks == 0) return;


            if (Input.GetKeyUp(KeyCode.D)) {
                int nextStackIndex = (shownStackIndex + 1) % numOfStacks;
                FocusOnStack(nextStackIndex);
            }

            if (Input.GetKeyUp(KeyCode.A)) {
                int nextStackIndex = (numOfStacks + (shownStackIndex - 1)) % numOfStacks;
                FocusOnStack(nextStackIndex);
            }

            if (Input.GetAxis("Mouse ScrollWheel") != 0) {
                zoomValue = -Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
                zoomValue = Mathf.Clamp(zoom + zoomValue, zoomClamp.x, zoomClamp.y);
            }

            zoom = Mathf.Lerp(zoom, zoomValue, 5 * Time.deltaTime);

            freeLookCamera.m_Orbits[0].m_Radius = zoom;
            freeLookCamera.m_Orbits[1].m_Radius = zoom;
            freeLookCamera.m_Orbits[2].m_Radius = zoom;
        }

        private void FocusOnStack(int stackIndex) {
            shownStackIndex = stackIndex;

            Vector3 objectToLookAtPoint = Ctx.Deps.BlocksSpawnController.GetStackCenterPoint(stackIndex);

            objectToLookAt.UpdateTargetPoint(objectToLookAtPoint);
            heightClamp.x = -objectToLookAtPoint.x / 2;
            heightClamp.y = objectToLookAtPoint.y * 2;

            freeLookCamera.m_Orbits[0].m_Height = Mathf.Lerp(freeLookCamera.m_Orbits[0].m_Height, heightClamp.x, 5 * Time.deltaTime);
            freeLookCamera.m_Orbits[1].m_Height = Mathf.Lerp(freeLookCamera.m_Orbits[1].m_Height, objectToLookAtPoint.y, 5 * Time.deltaTime);
            freeLookCamera.m_Orbits[2].m_Height = Mathf.Lerp(freeLookCamera.m_Orbits[2].m_Height, heightClamp.y, 5 * Time.deltaTime);
        }

        public void BlockDetailShown(Vector3 detailPanelPosition, int stackIndex) {
            shownStackIndex = stackIndex;
            objectToLookAt.UpdateTargetPoint(detailPanelPosition);
        }

        public void BlockDetailClosed() {
            Vector3 objectToLookAtPoint = Ctx.Deps.BlocksSpawnController.GetStackCenterPoint(shownStackIndex);
            objectToLookAt.UpdateTargetPoint(objectToLookAtPoint);
        }

        private void OnDestroy() {
            Ctx.Deps.EventsManager.StacksSpawned -= OnStacksSpawned;
        }

        public void Restart() {
            Clear();
        }

        private void Clear() {
            objectToLookAt.Clear();
            
            freeLookCamera.m_Orbits[0].m_Radius = zoom;
            freeLookCamera.m_Orbits[1].m_Radius = zoom;
            freeLookCamera.m_Orbits[2].m_Radius = zoom;
            
            numOfStacks = 0;
            shownStackIndex = 0;
            zoomValue = zoom;
        }
    }
}