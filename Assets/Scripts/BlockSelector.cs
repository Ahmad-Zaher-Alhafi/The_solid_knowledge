using UnityEngine;

public class BlockSelector : MonoBehaviour {
    private Ray mouseRay;
    private Block selectedBlock;
    private Block highlightedBlock;
    private bool physicsActivated;
    private bool stacksSpawned;

    private void Start() {
        Ctx.Deps.EventsManager.PhysicsActivated += OnPhysicsActivated;
        Ctx.Deps.EventsManager.GameRestarted += OnGameRestarted;
        Ctx.Deps.EventsManager.StacksSpawned += OnStacksSpawned;
    }

    private void OnStacksSpawned() {
        stacksSpawned = true;
    }

    private void OnGameRestarted() {
        physicsActivated = false;
        stacksSpawned = false;
    }

    private void OnPhysicsActivated() {
        physicsActivated = true;
        DeselectBlock();
        if (highlightedBlock != null) {
            highlightedBlock.Highlighted = false;
            highlightedBlock = null;
        }
    }

    private void Update() {
        if (!stacksSpawned || physicsActivated) return;

        mouseRay = Ctx.Deps.CameraController.Camera.ScreenPointToRay(Input.mousePosition);
        HighlightBlock();

        if (Input.GetMouseButtonUp(0)) {
            SelectBlock();
        }

        if (Input.GetMouseButtonUp(1)) {
            DeselectBlock();
        }
    }

    private void HighlightBlock() {
        if (highlightedBlock != null && highlightedBlock != selectedBlock) {
            highlightedBlock.Highlighted = false;
            highlightedBlock = null;
        }

        if (!Physics.Raycast(mouseRay, out RaycastHit raycastHit, Mathf.Infinity)) return;
        if (raycastHit.collider.gameObject.layer != LayerMask.NameToLayer("Block")) return;

        highlightedBlock = raycastHit.collider.GetComponentInParent<Block>();
        highlightedBlock.Highlighted = true;
    }

    private void SelectBlock() {
        DeselectBlock();

        if (!Physics.Raycast(mouseRay, out RaycastHit raycastHit, Mathf.Infinity)) return;
        if (raycastHit.collider.gameObject.layer != LayerMask.NameToLayer("Block")) return;

        selectedBlock = raycastHit.collider.GetComponentInParent<Block>();
        selectedBlock.Highlighted = true;

        Ctx.Deps.DetailsPanel.ShowDetails(raycastHit.point, selectedBlock.StackIndex, selectedBlock.Grade, selectedBlock.Domain, selectedBlock.Cluster, selectedBlock.StandardID, selectedBlock.StandardDescription);
        Ctx.Deps.CameraController.BlockDetailShown(Ctx.Deps.DetailsPanel.transform.position, selectedBlock.StackIndex);
    }

    private void DeselectBlock() {
        if (selectedBlock == null) return;
        selectedBlock.Highlighted = false;
        selectedBlock = null;

        Ctx.Deps.DetailsPanel.HideDetails();
        Ctx.Deps.CameraController.BlockDetailClosed();
    }

    private void OnDestroy() {
        Ctx.Deps.EventsManager.PhysicsActivated -= OnPhysicsActivated;
        Ctx.Deps.EventsManager.GameRestarted -= OnGameRestarted;
        Ctx.Deps.EventsManager.StacksSpawned -= OnStacksSpawned;
    }
}