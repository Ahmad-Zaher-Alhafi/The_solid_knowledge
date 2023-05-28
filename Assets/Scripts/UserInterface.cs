using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {
    [SerializeField] private Button activatePhysicsButton;
    [SerializeField] private Button skipAnimationButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private GameObject helpPanel;
    
    private void Start() {
        Ctx.Deps.EventsManager.StacksSpawned += OnStacksSpawned;
        Clear();
    }
    
    private void OnStacksSpawned() {
        skipAnimationButton.interactable = false;
        restartButton.interactable = true;
        activatePhysicsButton.interactable = true;

        if (PlayerPrefs.GetString("HelpShown") != "True") {
            OpenHelpClicked();
            PlayerPrefs.SetString("HelpShown","True");
        }
    }

    public void ActivatePhysicsClicked() {
        Ctx.Deps.GameController.ActivatePhysics();
        activatePhysicsButton.interactable = false;
    }

    public void SkipAnimationClicked() {
        Ctx.Deps.GameController.SkipAnimation();
        skipAnimationButton.interactable = false;
    }
    
    public void RestartClicked() {
        Clear();
        Ctx.Deps.GameController.Restart();
    }

    public void OpenHelpClicked() {
        helpPanel.SetActive(true);
    }
    
    public void CloseHelpClicked() {
        helpPanel.SetActive(false);
    }

    private void OnDestroy() {
        Ctx.Deps.EventsManager.StacksSpawned -= OnStacksSpawned;
    }

    private void Clear() {
        skipAnimationButton.interactable = true;
        activatePhysicsButton.interactable = false;
        restartButton.interactable = false;
    }
}