using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Controllers {
    public class BlocksSpawnController : MonoBehaviour, IController {
        [Header("Objects")]
        [SerializeField] private List<Block> blocksPrefabs;
        [SerializeField] private Stack stackPrefab;
        [SerializeField] private Transform stacksHolder;
        [SerializeField] private BlocksDataController blocksDataController;

        [Header("Positioning")]
        [SerializeField] private int numOfColumnsPerStack = 3;
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 offsetBetweenStacks;
        [SerializeField] private Vector3 offsetBetweenBlocks;

        [Header("Visuals")]
        [SerializeField] private float timeBetweenBlocks;

        public int StacksCount => stacks.Count;

        private readonly Dictionary<string, List<BlocksDataController.BlockData>> stacksData = new();
        private readonly List<Stack> stacks = new();

        private Vector3 blockSize;
        private Coroutine spawnStacksRoutine;
        private Vector3 initialStartPosition;
        private float initialTimeBetweenBlocks;

        private void Start() {
            Ctx.Deps.EventsManager.AnimationSkipped += OnAnimationSkipped;
            Ctx.Deps.EventsManager.PhysicsActivated += OnPhysicsActivated;
        }

        private void OnPhysicsActivated() {
            stacks.ForEach(stack => stack.RemoveGlassBlocks());
        }

        private void OnAnimationSkipped() {
            initialTimeBetweenBlocks = 0;
        }

        private void InitBlocks(List<BlocksDataController.BlockData> fetchedBlocksData) {
            foreach (BlocksDataController.BlockData blockData in fetchedBlocksData) {
                if (!stacksData.ContainsKey(blockData.grade)) {
                    stacksData.Add(blockData.grade, new List<BlocksDataController.BlockData>());
                }

                stacksData[blockData.grade].Add(blockData);
            }

            SortBlocks();
            spawnStacksRoutine = StartCoroutine(SpawnStacks());
        }

        private void SortBlocks() {
            foreach (string grade in stacksData.Keys.ToList()) {
                stacksData[grade] = stacksData[grade].OrderBy(data => data.domain).ThenBy(data => data.cluster).ThenBy(data => data.standardid).ToList();
            }
        }

        private IEnumerator SpawnStacks() {
            int stackIndex = 0;

            EnsureBlockSize(blocksPrefabs.First().transform.localScale);

            foreach (string grade in stacksData.Keys) {
                int columnNumber = 0;
                int rowNumber = 0;
                int angel = 0;

                Vector3 stackOffset = offsetBetweenStacks * stackIndex;
                Stack stack = SpawnStack(stackOffset, stackIndex, grade);

                foreach (BlocksDataController.BlockData gradeBlockData in stacksData[grade]) {
                    Vector3 blockOffset = angel == 0
                        ? new Vector3((blockSize.x + offsetBetweenBlocks.x) * columnNumber, blockSize.y * rowNumber)
                        : new Vector3(0, blockSize.y * rowNumber, (blockSize.x + offsetBetweenBlocks.x) * columnNumber);

                    Vector3 blockPlacingPosition = initialStartPosition + blockOffset + stackOffset;

                    SpawnBlock(gradeBlockData, blockPlacingPosition, angel, stack);

                    columnNumber++;
                    rowNumber += columnNumber / numOfColumnsPerStack;
                    columnNumber %= numOfColumnsPerStack;

                    if (columnNumber == 0) {
                        if (angel == 0) {
                            angel = 90;
                            initialStartPosition.z += blockSize.x;
                        } else {
                            angel = 0;
                            initialStartPosition.z -= blockSize.x;
                        }
                    }

                    yield return new WaitForSeconds(initialTimeBetweenBlocks);
                }


                stackIndex++;
            }

            Ctx.Deps.EventsManager.TriggerStacksSpawned();
        }

        private Stack SpawnStack(Vector3 position, int stackIndex, string grade) {
            Stack stack = stackPrefab.GetPooledInstance<Stack>(stacksHolder);
            stack.Init(stackIndex, grade);
            stack.transform.position = position;
            stacks.Add(stack);
            return stack;
        }

        private void SpawnBlock(BlocksDataController.BlockData blockData, Vector3 position, float angel, Stack stack) {
            Block block = blocksPrefabs[blockData.mastery].GetPooledInstance<Block>(stack.transform);
            block.transform.position = position + Vector3.up * 2;
            block.transform.localScale = blockSize;
            block.transform.eulerAngles = new Vector3(0, angel, 0);
            block.Init(blockData, position, stack.Index);

            stack.AddBlock(block);
        }

        /// <summary>
        /// Ensure that the length of the block equals to the total width of  the columns blocks + the space between them
        /// </summary>
        /// <param name="currentBlockSize"></param>
        private void EnsureBlockSize(Vector3 currentBlockSize) {
            blockSize = currentBlockSize;
            float totalWidth = blockSize.x * numOfColumnsPerStack;
            float totalSpaces = offsetBetweenBlocks.x * (numOfColumnsPerStack - 1);

            blockSize.z = totalWidth + totalSpaces;
        }

        public Vector3 GetStackCenterPoint(int stackIndex) {
            return stacks.Single(stack => stack.Index == stackIndex).CenterPoint;
        }

        private void OnDestroy() {
            Ctx.Deps.EventsManager.AnimationSkipped -= OnAnimationSkipped;
            Ctx.Deps.EventsManager.PhysicsActivated -= OnPhysicsActivated;
        }

        public void Restart() {
            Clear();
            blocksDataController.FetchData(InitBlocks);
        }

        private void Clear() {
            if (spawnStacksRoutine != null) {
                StopCoroutine(spawnStacksRoutine);
            }

            initialTimeBetweenBlocks = timeBetweenBlocks;
            initialStartPosition = startPosition;
            stacksData.Clear();
            stacks.ForEach(stack => stack.Clear());
            stacks.Clear();
        }
    }
}