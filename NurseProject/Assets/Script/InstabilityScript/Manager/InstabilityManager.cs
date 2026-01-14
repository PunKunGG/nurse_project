// InstabilityManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class InstabilityManager : MonoBehaviour
{
    [Header("Progress")]
    [SerializeField] private int totalHazards = 6;
    [SerializeField] private TMP_Text progressText;
    private int fixedCount = 0;

    [Header("Knowledge Popup (Animated)")]
    [SerializeField] private KnowledgePopupUI knowledgePopup;

    [Header("Win UI")]
    [SerializeField] private CanvasGroup winPanel;
    [SerializeField] private TMP_Text winText;

    [Header("Next Scene (Optional)")]
    [SerializeField] private string nextSceneName = ""; // e.g. "Immobility"

    private bool pendingWin = false;

    private void Awake()
    {
        UpdateProgressUI();

        SetCanvasGroupVisible(winPanel, false);
    }

    /// <summary>
    /// Called by each obstacle when fixed.
    /// </summary>
    public void OnObstacleFixed(string knowledgeMessage)
    {
        fixedCount = Mathf.Clamp(fixedCount + 1, 0, totalHazards);
        UpdateProgressUI();

        if (knowledgePopup != null && !string.IsNullOrWhiteSpace(knowledgeMessage))
            knowledgePopup.Show(knowledgeMessage);

        if (fixedCount >= totalHazards)
        {
    // If a knowledge popup is showing, wait for it to close
            if (knowledgePopup != null)
            {
                pendingWin = true;
                knowledgePopup.OnClosed += HandleKnowledgeClosedForWin;
            }
            else
            {
                Win();
            }
        }

    }

    private void HandleKnowledgeClosedForWin()
    {
        // Prevent multiple calls
        knowledgePopup.OnClosed -= HandleKnowledgeClosedForWin;

        if (!pendingWin) return;
        pendingWin = false;

        Win();
    }


    private void UpdateProgressUI()
    {
        if (progressText != null)
            progressText.text = $"{fixedCount}/{totalHazards}";
    }

    private void Win()
    {
        // Keep text consistent with your doc (can paste longer version later).
        if (winText != null)
        {
            winText.text =
                "Excellent! You've helped reduce the risk of falls in the elderly\n" +
                "by creating an environment based on instability nursing principles.";
        }

        SetCanvasGroupVisible(winPanel, true);
    }

    // Hook this to WinPanel "Next" button if you want
    public void LoadNextScene()
    {
        if (string.IsNullOrWhiteSpace(nextSceneName)) return;
        SceneManager.LoadScene(nextSceneName);
    }

    private static void SetCanvasGroupVisible(CanvasGroup cg, bool visible)
    {
        if (cg == null) return;
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }
}

