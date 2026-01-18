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
        if (option == correctOption)
        {
            if (feedbackText) feedbackText.text = "";
            stageManager.OnGradeAnsweredCorrect();
        }
        else
        {
            if (feedbackText) feedbackText.text = "Wrong. Try again.";
        }
    }
}
