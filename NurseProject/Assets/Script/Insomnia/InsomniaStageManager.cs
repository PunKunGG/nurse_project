using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class InsomniaQuestionData
{
    public string title;        // หัวข้อ 
    [TextArea(2, 4)]
    public string questionText; // คำถาม (Question)
    public string answer1;      // ตัวเลือก 1
    public string answer2;      // ตัวเลือก 2
    public int correctAnswer;   // 1 หรือ 2
}

public class InsomniaStageManager : MonoBehaviour
{
    [Header("UI References (Question)")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI pageIndicatorText;
    public TextMeshProUGUI btn1Text;
    public TextMeshProUGUI btn2Text;

    [Header("Buttons (Auto-wired if empty)")]
    public Button btn1;
    public Button btn2;

    [Header("Button Colors")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color correctColor = new Color(0.2f, 0.8f, 0.3f); // green
    [SerializeField] private Color wrongColor   = new Color(0.9f, 0.3f, 0.3f); // red
    [SerializeField] private float feedbackDelay = 1.0f; // วินาทีที่แสดงสีก่อนไปข้อถัดไป

    [Header("UI References (Result)")]
    public UniversalResultUI resultUI;
    public int passScore = 4;

    [Header("Result Text (Configurable)")]
    public string winTitle = "แบบทดสอบผ่านแล้ว!";
    [TextArea(2,4)]
    public string winMessage = "คุณทำคะแนนได้ {0} คะแนน ซึ่งผ่านเกณฑ์ครับ";
    public string winButtonText = "ไปด่านต่อไป >>";

    public string failTitle = "ยังไม่ผ่านเกณฑ์";
    [TextArea(2,4)]
    public string failMessage = "คุณทำได้ {0} คะแนน ลองทบทวนเนื้อหา Insomnia ใหม่อีกครั้งนะครับ";
    public string failButtonText = "ลองอีกครั้ง";

    [Header("Question Pool")]
    public List<InsomniaQuestionData> allQuestions;

    private List<InsomniaQuestionData> selectedQuestions = new List<InsomniaQuestionData>();
    private int currentQuestionIndex = 0;
    private int score = 0;
    private bool waitingForFeedback = false;

    private readonly Vector2[] btnPositions = { new Vector2(450, -400), new Vector2(-450, -400) };

    void Start()
    {
        SetupButtons();
        SetupQuiz();
    }

    void SetupButtons()
    {
        if (btn1 == null && btn1Text != null) btn1 = btn1Text.GetComponentInParent<Button>();
        if (btn2 == null && btn2Text != null) btn2 = btn2Text.GetComponentInParent<Button>();

        if (btn1 != null)
        {
            btn1.onClick.RemoveAllListeners();
            btn1.onClick.AddListener(() => OnAnswerSelected(1));
        }
        else
        {
            Debug.LogWarning("InsomniaStageManager: Btn1 not found!");
        }

        if (btn2 != null)
        {
            btn2.onClick.RemoveAllListeners();
            btn2.onClick.AddListener(() => OnAnswerSelected(2));
        }
        else
        {
            Debug.LogWarning("InsomniaStageManager: Btn2 not found!");
        }
    }

    void SetupQuiz()
    {
        List<InsomniaQuestionData> tempPool = new List<InsomniaQuestionData>(allQuestions);
        selectedQuestions.Clear();

        int questionCount = Mathf.Min(10, tempPool.Count);
        for (int i = 0; i < questionCount; i++)
        {
            int randomIndex = Random.Range(0, tempPool.Count);
            selectedQuestions.Add(tempPool[randomIndex]);
            tempPool.RemoveAt(randomIndex);
        }

        currentQuestionIndex = 0;
        score = 0;
        DisplayQuestion();
    }

    void DisplayQuestion()
    {
        RandomizeButtonPositions();

        if (currentQuestionIndex < selectedQuestions.Count)
        {
            InsomniaQuestionData q = selectedQuestions[currentQuestionIndex];

            if(titleText) titleText.text = q.title;
            if(questionText) questionText.text = q.questionText;
            if(btn1Text) btn1Text.text = q.answer1;
            if(btn2Text) btn2Text.text = q.answer2;

            if(pageIndicatorText) 
                pageIndicatorText.text = $"{currentQuestionIndex + 1}/{selectedQuestions.Count}";

            // Reset button colors
            ResetButtonColors();

            // Re-enable buttons
            if (btn1) btn1.interactable = true;
            if (btn2) btn2.interactable = true;
        }
        else
        {
            ShowResult();
        }
    }

    public void OnAnswerSelected(int choice)
    {
        if (waitingForFeedback) return;
        if (currentQuestionIndex >= selectedQuestions.Count) return;

        bool isCorrect = (choice == selectedQuestions[currentQuestionIndex].correctAnswer);

        if (isCorrect)
        {
            score++;
            Debug.Log("Insomnia: Correct Answer");
        }
        else
        {
            Debug.Log("Insomnia: Wrong Answer");
        }

        // Show color feedback on both buttons
        ShowButtonFeedback(choice, isCorrect);

        // Disable buttons while showing feedback
        if (btn1) btn1.interactable = false;
        if (btn2) btn2.interactable = false;

        // Advance after delay
        StartCoroutine(AdvanceAfterDelay());
    }

    private void ShowButtonFeedback(int chosenAnswer, bool isCorrect)
    {
        int correctAns = selectedQuestions[currentQuestionIndex].correctAnswer;

        // สีปุ่มที่เลือก
        Button chosenBtn = (chosenAnswer == 1) ? btn1 : btn2;
        if (chosenBtn && chosenBtn.image)
            chosenBtn.image.color = isCorrect ? correctColor : wrongColor;

        // ถ้าตอบผิด แสดงสีเขียวที่ปุ่มที่ถูกด้วย
        if (!isCorrect)
        {
            Button correctBtn = (correctAns == 1) ? btn1 : btn2;
            if (correctBtn && correctBtn.image)
                correctBtn.image.color = correctColor;
        }
    }

    private IEnumerator AdvanceAfterDelay()
    {
        waitingForFeedback = true;
        yield return new WaitForSeconds(feedbackDelay);
        waitingForFeedback = false;

        currentQuestionIndex++;

        if (currentQuestionIndex < selectedQuestions.Count)
        {
            DisplayQuestion();
        }
        else
        {
            ShowResult();
        }
    }

    private void ResetButtonColors()
    {
        if (btn1 && btn1.image) btn1.image.color = defaultColor;
        if (btn2 && btn2.image) btn2.image.color = defaultColor;
    }

    private void RandomizeButtonPositions()
    {
        if (btn1 == null || btn2 == null) return;

        RectTransform rt1 = btn1.GetComponent<RectTransform>();
        RectTransform rt2 = btn2.GetComponent<RectTransform>();

        if (rt1 == null || rt2 == null) return;

        // Shuffle index
        if (Random.value > 0.5f)
        {
            rt1.anchoredPosition = btnPositions[0];
            rt2.anchoredPosition = btnPositions[1];
        }
        else
        {
            rt1.anchoredPosition = btnPositions[1];
            rt2.anchoredPosition = btnPositions[0];
        }
    }

    void ShowResult()
    {
        bool isPassed = (score >= passScore);
        
        if (resultUI)
        {
            if (isPassed)
            {
                resultUI.ShowResult(
                    true,
                    winTitle,
                    string.Format(winMessage, score),
                    winButtonText
                );
            }
            else
            {
                resultUI.ShowResult(
                    false,
                    failTitle,
                    string.Format(failMessage, score),
                    failButtonText
                );
            }
        }
    }

    // --- ส่วนสำหรับ Debugger เรียกใช้ ---
    
    public void ForceAnswer(bool isCorrect)
    {
        if (isCorrect) score++;
        currentQuestionIndex++;

        if (currentQuestionIndex < selectedQuestions.Count) DisplayQuestion();
        else ShowResult();
    }

    public void ForceEndGame(bool forceWin)
    {
        if (forceWin) score = selectedQuestions.Count;
        else score = 0;
        ShowResult();
    }
    
    // Getters
    public int GetCurrentQuestionNum() => currentQuestionIndex + 1;
    public int GetScore() => score;
}