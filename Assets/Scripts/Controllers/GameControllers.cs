using System;
using UnityEngine;

namespace Controllers {
    public class GameControllers : MonoBehaviour {
        private IController BlocksDataController;
        private IController BlocksSpawnController;
        private IController CameraController;

        private void OnEnable() {
            CameraController = GetComponentInChildren<CameraController>();
            BlocksDataController = GetComponentInChildren<BlocksDataController>();
            BlocksSpawnController = GetComponentInChildren<BlocksSpawnController>();
            Ctx.Deps.EventsManager.GameRestarted += RestartControllers;
        }

        private void OnDisable() {
            Ctx.Deps.EventsManager.GameRestarted -= RestartControllers;
        }

        private void RestartControllers() {
            BlocksDataController.Restart();
            BlocksSpawnController.Restart();
            CameraController.Restart();
        }
    }
}