using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DetailsPanel : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private TextMeshProUGUI domainText;
    [SerializeField] private TextMeshProUGUI clusterText;
    [SerializeField] private TextMeshProUGUI standardID;
    [SerializeField] private TextMeshProUGUI standardDescriptionText;

    [SerializeField] private float punchValue = .01f;

    private Tween showTween;

    private void Start() {
        Ctx.Deps.EventsManager.GameRestarted += OnGameRestarted;
    }
    
    private void OnGameRestarted() {
        HideDetails();
    }

    public void ShowDetails(Vector3 blockPosition, int stackIndex, string grade, string domain, string cluster, string standardId, string standardDescription) {
        gameObject.SetActive(true);

        gradeText.text = grade + ": ";
        domainText.text = domain;
        clusterText.text = cluster;
        standardID.text = standardId + ": ";
        standardDescriptionText.text = standardDescription;


        RectTransform rectTransform = (RectTransform) transform;


        Vector3 stackCenterPoint = Ctx.Deps.BlocksSpawnController.GetStackCenterPoint(stackIndex);

        transform.position = new Vector3(stackCenterPoint.x, blockPosition.y, blockPosition.z);

        Vector2 panelMinPoint = rectTransform.TransformPoint(rectTransform.rect.min);
        Vector2 panelMaxPoint = rectTransform.TransformPoint(rectTransform.rect.max);
        Vector2 panelSize = panelMaxPoint - panelMinPoint;

        float stackMinYPoint = stackCenterPoint.y - stackCenterPoint.y / 2;

        float difference = Mathf.Abs(stackMinYPoint - panelMinPoint.y);

        // Make sure the panel won't go under the ground
        float yPosition = panelMinPoint.y < stackMinYPoint ? transform.position.y + difference : blockPosition.y;

        Vector3 placingPosition = new Vector3(stackCenterPoint.x + panelSize.x - 1, yPosition, stackCenterPoint.z);
        transform.position = placingPosition;

        showTween = transform.DOPunchScale(Vector3.one * punchValue, .15f).SetAutoKill(false).OnKill(() => showTween = null);
        showTween.Restart();
    }

    public void HideDetails() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        Ctx.Deps.EventsManager.GameRestarted -= OnGameRestarted;
        showTween?.Kill();
    }
}