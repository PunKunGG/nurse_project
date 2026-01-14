using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class InstabilityManager : MonoBehaviour
{
    [Header("Progress")]
    [SerializeField] private int totalObstacles = 6;
    private int fixedCount = 0;

    [Header("UI")]
    [SerializeField] private TMP_Text progressText;

    [Header("Knowledge Popup")]
    [SerializeField] private CanvasGroup knowledgePanel;
    [SerializeField] private TMP_Text knowledgeText;
    [SerializeField] private float knowledgeAutoHideSeconds = 2.5f;

    [Header("Win UI")]
    [SerializeField] private CanvasGroup winPanel;
    [SerializeField] private TMP_Text winText;

    private float hideAtTime = -1f;

    private void Awake()
    {
        SetCanvasGroupVisible(knowledgePanel, false);
        SetCanvasGroupVisible(winPanel, false);
        UpdateProgressUI();
    }

    private void Update()
    {
        if (knowledgePanel != null && knowledgePanel.alpha > 0f && hideAtTime > 0f && Time.time >= hideAtTime)
        {
            SetCanvasGroupVisible(knowledgePanel, false);
            hideAtTime = -1f;
        }
    }

    public void OnObstacleFixed(string knowledgeMessage)
    {
        fixedCount = Mathf.Clamp(fixedCount + 1, 0, totalObstacles);
        UpdateProgressUI();

        if (!string.IsNullOrWhiteSpace(knowledgeMessage))
            ShowKnowledge(knowledgeMessage);

        if (fixedCount >= totalObstacles)
            Win();
    }

    private void UpdateProgressUI()
    {
        if (progressText != null)
            progressText.text = $"{fixedCount}/{totalObstacles}";
    }

    private void ShowKnowledge(string msg)
    {
        if (knowledgePanel == null || knowledgeText == null) return;

        knowledgeText.text = msg;
        SetCanvasGroupVisible(knowledgePanel, true);
        hideAtTime = Time.time + Mathf.Max(0.5f, knowledgeAutoHideSeconds);
    }

    private void Win()
    {
        // Success wording based on your doc
        // Keep it short for prototype; you can paste full Thai later.
        if (winText != null)
        {
            winText.text =
                "Excellent! You've helped reduce the risk of falls in the elderly\n" +
                "by creating an environment based on instability nursing principles.";
        }

        SetCanvasGroupVisible(winPanel, true);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private static void SetCanvasGroupVisible(CanvasGroup cg, bool visible)
    {
        if (cg == null) return;
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }
}
