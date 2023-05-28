using Controllers;
using DG.Tweening;
using Pooling;
using UnityEngine;

public abstract class Block : PooledObject {
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float placingTime = .5f;
    [SerializeField] private float massScale = 3;
    [SerializeField] private Color highlightColor;

    private float initialPlacingTime;
    private Color initialColor;

    public Bounds Bounds => meshRenderer.bounds;
    public bool Highlighted {
        get => highlighted;
        set {
            highlighted = value;
            meshRenderer.material.color = highlighted ? highlightColor : initialColor;
        }
    }
    private bool highlighted;

    public string Grade => blockData.grade;
    public string Domain => blockData.domain;
    public string Cluster => blockData.cluster;
    public string StandardID => blockData.standardid;
    public string StandardDescription => blockData.standarddescription;
    public int StackIndex { get; private set; }

    private Tween placingTween;
    private Rigidbody rig;
    private BlocksDataController.BlockData blockData;

    protected virtual void Awake() {
        rig = GetComponent<Rigidbody>();
        rig.isKinematic = true;
        initialPlacingTime = placingTime;
        initialColor = meshRenderer.material.color;

        Ctx.Deps.EventsManager.PhysicsActivated += OnPhysicsActivated;
        Ctx.Deps.EventsManager.AnimationSkipped += OnAnimationSkipped;
        Ctx.Deps.EventsManager.GameRestarted += OnGameRestarted;
    }

    private void OnGameRestarted() {
        Clear();
    }

    private void OnAnimationSkipped() {
        placingTween?.Complete();
    }

    private void OnPhysicsActivated() {
        rig.isKinematic = false;
    }

    public void Init(BlocksDataController.BlockData blockData, Vector3 position, int stackIndex) {
        this.blockData = blockData;
        StackIndex = stackIndex;
        rig.mass = massScale * blockData.mastery;
        placingTween = transform.DOMove(position, initialPlacingTime).SetEase(Ease.InOutSine)
            .SetAutoKill(false)
            .OnKill(() => placingTween = null)
            .Play();
    }

    private void OnDestroy() {
        Ctx.Deps.EventsManager.PhysicsActivated -= OnPhysicsActivated;
        Ctx.Deps.EventsManager.AnimationSkipped -= OnAnimationSkipped;
        Ctx.Deps.EventsManager.GameRestarted -= OnGameRestarted;
    }

    public void Clear() {
        gameObject.SetActive(true);

        transform.rotation = Quaternion.identity;
        rig.isKinematic = true;

        blockData = null;
        initialPlacingTime = placingTime;
        Highlighted = false;
        placingTween?.Kill();
        ReturnToPool();
    }
}