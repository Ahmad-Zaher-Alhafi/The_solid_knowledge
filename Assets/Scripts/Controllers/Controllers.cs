using UnityEngine;

namespace Controllers {
    public interface IControllers {
        BlocksSpawnController BlocksSpawnController { get; }
        CameraController CameraController { get; }
        IEventManager EventsManager { get; }
        GameController GameController { get; }
        DetailsPanel DetailsPanel { get; }
    }

    public class Controllers : MonoBehaviour, IControllers {
        public BlocksSpawnController BlocksSpawnController { get; private set; }
        public CameraController CameraController { get; private set; }
        public IEventManager EventsManager { get; private set; }
        public GameController GameController { get; private set; }
        public DetailsPanel DetailsPanel { get; private set; }

        private void Awake() {
            BlocksSpawnController = GetComponentInChildren<BlocksSpawnController>();
            CameraController = GetComponentInChildren<CameraController>();
            EventsManager = GetComponentInChildren<IEventManager>();
            GameController = GetComponentInChildren<GameController>();
            DetailsPanel = FindObjectOfType<DetailsPanel>();

            Ctx.SetContext(this);
        }
    }
}