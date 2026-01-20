using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GradeQuestionUI : MonoBehaviour
{
    [Header("Wiring")]
    [SerializeField] private ImmobilityStageManager stageManager;

    [Header("Buttons")]
    [SerializeField] private Button btn1;
    [SerializeField] private Button btn2;
    [SerializeField] private Button btn3;

    [Header("Correct Answer (1/2/3)")]
    [SerializeField] private int correctOption = 2;

    [Header("Optional Feedback")]
    [SerializeField] private TextMeshProUGUI feedbackText; // or TMP_Text if you use TextMeshPro

    private void Awake()
    {
        if (btn1) btn1.onClick.AddListener(() => Choose(1));
        if (btn2) btn2.onClick.AddListener(() => Choose(2));
        if (btn3) btn3.onClick.AddListener(() => Choose(3));
    }

    private void OnEnable()
    {
        if (feedbackText) feedbackText.text = "";
    }

    private void Choose(int option)
    {
        // รีเซ็ตสีปุ่มทั้งหมดก่อน (เผื่อกดผิดไปแล้วปุ่มแดงค้าง)
        ResetButtonColors(); 

        if (option == correctOption)
        {
            if (feedbackText) feedbackText.text = "Correct!";
            if (feedbackText) feedbackText.color = Color.green;
            
            // เปลี่ยนปุ่มที่กดเป็นสีเขียว
            SetButtonColor(option, Color.green);

            stageManager.OnGradeAnsweredCorrect();
        }
        else
        {
            if (feedbackText) feedbackText.text = "Wrong. Try again.";
            if (feedbackText) feedbackText.color = Color.red;

            // เปลี่ยนปุ่มที่กดเป็นสีแดง
            SetButtonColor(option, Color.red);
        }
    }

    private void SetButtonColor(int option, Color color)
    {
        Button targetBtn = null;
        if (option == 1) targetBtn = btn1;
        else if (option == 2) targetBtn = btn2;
        else if (option == 3) targetBtn = btn3;

        if (targetBtn) targetBtn.image.color = color;
    }

    private void ResetButtonColors()
    {
        if(btn1) btn1.image.color = Color.white;
        if(btn2) btn2.image.color = Color.white;
        if(btn3) btn3.image.color = Color.white;
    }
}
