using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class InsomniaQuestionData
{
    public string title;        // หัวข้อ 
    [TextArea(3, 5)]
    public string storyText;    // เนื้อหาคำถาม
    public string answer1;      // ตัวเลือก 1
    public string answer2;      // ตัวเลือก 2
    public int correctAnswer;   // 1 หรือ 2
}

public class InsomniaQuizManager : MonoBehaviour
{
    [Header("UI References (Question)")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI storyText;
    public TextMeshProUGUI pageIndicatorText;
    public TextMeshProUGUI btn1Text;
    public TextMeshProUGUI btn2Text;

    [Header("UI References (Result)")]
    public UniversalResultUI resultUI; // ลาก UniversalResultUI มาใส่
    public int passScore = 3;          // เกณฑ์ผ่าน (เช่น 3 เต็ม 4)

    [Header("Question Pool")]
    public List<InsomniaQuestionData> allQuestions;

    private List<InsomniaQuestionData> selectedQuestions = new List<InsomniaQuestionData>();
    private int currentQuestionIndex = 0;
    private int score = 0;

    void Start()
    {
        // เริ่มเกมมาให้ Setup เลย
        SetupQuiz();
    }

    void SetupQuiz()
    {
        // 1. สุ่มเลือกคำถาม 4 ข้อจาก Pool
        List<InsomniaQuestionData> tempPool = new List<InsomniaQuestionData>(allQuestions);
        selectedQuestions.Clear();
        
        int questionCount = Mathf.Min(4, tempPool.Count); // ป้องกัน Error ถ้าน้อยกว่า 4 ข้อ
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
        if (currentQuestionIndex < selectedQuestions.Count)
        {
            InsomniaQuestionData q = selectedQuestions[currentQuestionIndex];
            
            // อัปเดต UI
            if(titleText) titleText.text = q.title;
            if(storyText) storyText.text = q.storyText;
            if(btn1Text) btn1Text.text = q.answer1;
            if(btn2Text) btn2Text.text = q.answer2;
            
            if(pageIndicatorText) 
                pageIndicatorText.text = $"{currentQuestionIndex + 1}/{selectedQuestions.Count}";
        }
        else
        {
            ShowResult();
        }
    }

    // ฟังก์ชันผูกกับปุ่มใน Unity (Btn 1, Btn 2)
    public void OnAnswerSelected(int choice)
    {
        if (choice == selectedQuestions[currentQuestionIndex].correctAnswer)
        {
            score++;
            Debug.Log("Insomnia: Correct Answer");
        }
        else
        {
            Debug.Log("Insomnia: Wrong Answer");
        }

        currentQuestionIndex++;
        
        // เช็คว่าหมดข้อสอบหรือยัง
        if (currentQuestionIndex < selectedQuestions.Count)
        {
            DisplayQuestion();
        }
        else
        {
            ShowResult();
        }
    }

    void ShowResult()
    {
        bool isPassed = (score >= passScore);
        
        // เรียกใช้ Universal Result Panel
        if (resultUI)
        {
            resultUI.ShowResult(isPassed);
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