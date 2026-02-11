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
    [SerializeField] private Button btn4;

    [Header("Optional Feedback")]
    [SerializeField] private TextMeshProUGUI feedbackText;

    private int correctOption; // runtime 1..4

    [Header("Wound Data")]
    [SerializeField] private WoundGradeInfo[] woundData; // Index 0 = Grade 1, etc.

    [Header("UI Elements")]
    [SerializeField] private Image magnifiedImageDisplay;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [System.Serializable]
    public struct WoundGradeInfo
    {
        public Sprite magnifiedImage;
        [TextArea]
        public string description;
    }

    private void Awake()
    {
        if (btn1) btn1.onClick.AddListener(() => Choose(1));
        if (btn2) btn2.onClick.AddListener(() => Choose(2));
        if (btn3) btn3.onClick.AddListener(() => Choose(3));
        if (btn4) btn4.onClick.AddListener(() => Choose(4));
    }

    private void OnEnable()
    {
        if (feedbackText) feedbackText.text = "";
        ResetButtonColors();

        if (!stageManager)
        {
            Debug.LogError("GradeQuestionUI: stageManager not assigned.", this);
            correctOption = 0;
            return;
        }

        correctOption = stageManager.CurrentWoundGrade; // 1..4
        if (correctOption < 1 || correctOption > 4)
        {
            Debug.LogWarning($"GradeQuestionUI: invalid wound grade = {correctOption}.", this);
        }

        UpdateWoundInfoUI(correctOption);
    }

    private void UpdateWoundInfoUI(int grade)
    {
        int dataIndex = grade - 1; // 0-based index for array

        if (woundData != null && dataIndex >= 0 && dataIndex < woundData.Length)
        {
            var info = woundData[dataIndex];

            if (magnifiedImageDisplay)
            {
                magnifiedImageDisplay.sprite = info.magnifiedImage;
                magnifiedImageDisplay.gameObject.SetActive(info.magnifiedImage != null);
            }

            if (descriptionText)
            {
                descriptionText.text = info.description;
            }
        }
        else
        {
            // Clear or hide if data is missing
            if (magnifiedImageDisplay) magnifiedImageDisplay.gameObject.SetActive(false);
            if (descriptionText) descriptionText.text = "";
        }
    }

    private void Choose(int option)
    {
        ResetButtonColors();

        if (option == correctOption)
        {
            if (feedbackText)
            {
                feedbackText.text = "Correct!";
                feedbackText.color = Color.green;
            }

            SetButtonColor(option, Color.green);

            if (stageManager) stageManager.OnGradeAnsweredCorrect();
        }
        else
        {
            if (feedbackText)
            {
                feedbackText.text = "Wrong. Try again.";
                feedbackText.color = Color.red;
            }

            SetButtonColor(option, Color.red);
        }
    }

    private void SetButtonColor(int option, Color color)
    {
        Button targetBtn = null;
        if (option == 1) targetBtn = btn1;
        else if (option == 2) targetBtn = btn2;
        else if (option == 3) targetBtn = btn3;
        else if (option == 4) targetBtn = btn4;

        if (targetBtn) targetBtn.image.color = color;
    }

    private void ResetButtonColors()
    {
        if (btn1) btn1.image.color = Color.white;
        if (btn2) btn2.image.color = Color.white;
        if (btn3) btn3.image.color = Color.white;
        if (btn4) btn4.image.color = Color.white;
    }
}
