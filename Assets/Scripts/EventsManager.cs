using System;
using UnityEngine;

public interface IEventManager {
    event Action PhysicsActivated;
    void TriggerPhysicsActivated();

    event Action AnimationSkipped;
    void TriggerAnimationSkipped();

    event Action StacksSpawned;
    void TriggerStacksSpawned();

    event Action GameRestarted;
    void TriggerGameRestarted();
}

public class EventsManager : MonoBehaviour, IEventManager {
    public event Action PhysicsActivated;
    public void TriggerPhysicsActivated() {
        PhysicsActivated?.Invoke();
    }

    public event Action AnimationSkipped;
    public void TriggerAnimationSkipped() {
        AnimationSkipped?.Invoke();
    }

    public event Action StacksSpawned;
    public void TriggerStacksSpawned() {
        StacksSpawned?.Invoke();
    }

    public event Action GameRestarted;
    public void TriggerGameRestarted() {
        GameRestarted?.Invoke();
    }
}