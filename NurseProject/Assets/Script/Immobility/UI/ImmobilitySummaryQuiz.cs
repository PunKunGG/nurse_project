using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ImmobilitySummaryQuiz : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button[] answerButtons; // 10 buttons
    // [SerializeField] private Button submitButton; // Removed
    // [SerializeField] private Button closeButton;  // Removed


    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private float wrongFeedbackDuration = 1.0f;

    [SerializeField] private ImmobilityStageManager stageManager;

    private readonly Vector2[] availablePositions = new Vector2[]
    {
        new Vector2(-240, -47), new Vector2(40, -47), new Vector2(-540, -47), 
        new Vector2(40, -250), new Vector2(340, -47), new Vector2(340, -250), 
        new Vector2(640, -47), new Vector2(640, -250), new Vector2(-240, -250), 
        new Vector2(-540, -250)
    };

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

        
        correctFoundCount = 0;

        RandomizeButtonPositions();

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

    private void RandomizeButtonPositions()
    {
        if (answerButtons == null || answerButtons.Length == 0) return;

        // Create a copy of positions to shuffle
        List<Vector2> shuffledPositions = new List<Vector2>(availablePositions);

        // Simple Fisher-Yates shuffle
        for (int i = 0; i < shuffledPositions.Count; i++)
        {
            int rnd = Random.Range(i, shuffledPositions.Count);
            Vector2 temp = shuffledPositions[i];
            shuffledPositions[i] = shuffledPositions[rnd];
            shuffledPositions[rnd] = temp;
        }

        // Assign shuffled positions to buttons
        for (int i = 0; i < answerButtons.Length && i < shuffledPositions.Count; i++)
        {
            if (answerButtons[i])
            {
                RectTransform rt = answerButtons[i].GetComponent<RectTransform>();
                if (rt) rt.anchoredPosition = shuffledPositions[i];
            }
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
