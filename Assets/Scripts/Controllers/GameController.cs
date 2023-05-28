using UnityEngine;

namespace Controllers {
    public class GameController : MonoBehaviour {
        private void Start() {
            Restart();
        }

        public void ActivatePhysics() {
            Ctx.Deps.EventsManager.TriggerPhysicsActivated();
        }

        public void SkipAnimation() {
            Ctx.Deps.EventsManager.TriggerAnimationSkipped();
        }

        public void Restart() {
            Ctx.Deps.EventsManager.TriggerGameRestarted();
        }
    }
}