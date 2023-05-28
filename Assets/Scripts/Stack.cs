using System.Collections.Generic;
using System.Linq;
using Blocks;
using Pooling;
using TMPro;
using UnityEngine;

public class Stack : PooledObject {
    [SerializeField] private TextMeshPro stackName;
    [SerializeField] private GameObject tag;

    public int Index { get; private set; }
    public string Grade { get; private set; }

    public Vector3 CenterPoint {
        get {
            float stackCentralX = blocks.Average(block => block.Bounds.center.x);
            float stackCentralY = blocks.Average(block => block.Bounds.center.y);
            float stackCentralZ = blocks.Average(block => block.Bounds.center.z);
            return new Vector3(stackCentralX, stackCentralY, stackCentralZ);
        }
    }

    private readonly List<Block> blocks = new();

    private void Awake() {
        tag.SetActive(false);
        Ctx.Deps.EventsManager.StacksSpawned += OnStacksSpawned;
    }
    private void OnStacksSpawned() {
        Vector3 stackCenterPoint = Ctx.Deps.BlocksSpawnController.GetStackCenterPoint(Index);
        tag.SetActive(true);
        tag.transform.position = new Vector3(stackCenterPoint.x + 2, .5f, stackCenterPoint.z - 2);
    }

    public void Init(int index, string grade) {
        Index = index;
        Grade = grade;

        name = grade + " Stack";
        stackName.text = grade;
    }

    public void AddBlock(Block block) {
        blocks.Add(block);
    }

    public void RemoveGlassBlocks() {
        blocks.OfType<GlassBlock>().ToList().ForEach(block => block.Clear());
    }

    private void OnDestroy() {
        Ctx.Deps.EventsManager.StacksSpawned -= OnStacksSpawned;
    }

    public void Clear() {
        Index = default;
        Grade = default;
        blocks.ForEach(block => block.Clear());
        blocks.Clear();
        tag.SetActive(false);
        ReturnToPool();
    }
}