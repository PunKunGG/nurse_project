using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ImmobilitySummaryQuiz : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button[] answerButtons; // 10 buttons
    // [SerializeField] private Button submitButton; // Removed
    // [SerializeField] private Button closeButton;  // Removed
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private float wrongFeedbackDuration = 1.0f;

    [Header("Wiring")]
    [SerializeField] private ImmobilityStageManager stageManager;

    // Based on user spec:
    // 0-indexed correct indices: 0, 1, 2, 5, 6, 7 (Total 6)
    private readonly int[] correctIndices = { 0, 1, 2, 3, 4, 5 };
    private int correctFoundCount = 0;

    private void Awake()
    {
        if (panelRoot) panelRoot.SetActive(false);
        // if (submitButton) submitButton.onClick.AddListener(OnSubmit);
        // if (closeButton) closeButton.onClick.AddListener(OnClose);
        
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (answerButtons == null) return;
        
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; // Local copy for closure
            if (answerButtons[i])
            {
                answerButtons[i].onClick.AddListener(() => OnAnswerButtonClicked(index));
            }
        }
    }

    public void Show()
    {
        if (panelRoot) panelRoot.SetActive(true);
        if (scoreText) scoreText.text = "Select all correct complications.";
        
        correctFoundCount = 0;

        // Reset state
        if (answerButtons != null)
        {
            foreach (var btn in answerButtons)
            {
                if (btn) 
                {
                    btn.interactable = true;
                    if(btn.image) btn.image.color = normalColor;
                }
            }
        }
    }

    private void OnAnswerButtonClicked(int index)
    {
        Button btn = answerButtons[index];
        if (!btn || !btn.interactable) return;

        bool isCorrect = IsCorrectIndex(index);

        if (isCorrect)
        {
            // Correct -> Turn Green, Disable click, Count up
            if (btn.image) btn.image.color = correctColor;
            btn.interactable = false;
            correctFoundCount++;
            
            CheckCompletion();
        }
        else
        {
            // Wrong -> Turn Red, Wait, Turn White
            StartCoroutine(WrongFeedbackRoutine(btn));
        }
    }

    private IEnumerator WrongFeedbackRoutine(Button btn)
    {
        if (btn.image) btn.image.color = wrongColor;
        btn.interactable = false; // Prevent spamming

        yield return new WaitForSeconds(wrongFeedbackDuration);

        if (btn)
        {
            if (btn.image) btn.image.color = normalColor;
            btn.interactable = true;
        }
    }

    private void CheckCompletion()
    {
        if (scoreText) scoreText.text = $"Found: {correctFoundCount} / {correctIndices.Length}";

        if (correctFoundCount >= correctIndices.Length)
        {
            Debug.Log("Quiz Completed! All correct answers found.");
            Invoke(nameof(FinishStage), 1.0f); // Small delay before closing
        }
    }

    private void FinishStage()
    {
        if (panelRoot) panelRoot.SetActive(false);
        if (stageManager)
        {
            stageManager.CompleteStage();
        }
    }

    private bool IsCorrectIndex(int index)
    {
        foreach (int c in correctIndices)
        {
            if (c == index) return true;
        }
        return false;
    }
}
