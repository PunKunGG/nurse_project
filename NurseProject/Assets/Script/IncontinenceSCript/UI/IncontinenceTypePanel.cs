// IncontinenceTypePanel.cs — Stage 2: เลือกประเภท Incontinence (Single choice)
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel Stage 2: แสดง 7 ปุ่มให้ผู้เล่นเลือกประเภท Incontinence
/// ผูกกับ IncontinenceLevelController
/// </summary>
public class IncontinenceTypePanel : MonoBehaviour
{
    [Header("Buttons (7 ปุ่ม — เรียงตาม enum IncontinenceType)")]
    [Tooltip("ลำดับ: Stress, Urge, Overflow, Functional, Mixed(S+U), Mixed(U+F), Mixed(S+F)")]
    [SerializeField] private Button[] typeButtons = new Button[7];

    [Header("Feedback")]
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Colors")]
    [SerializeField] private Color defaultColor  = Color.white;
    [SerializeField] private Color correctColor  = new Color(0.2f, 0.8f, 0.3f); // green
    [SerializeField] private Color wrongColor    = new Color(0.9f, 0.3f, 0.3f); // red

    // Reference ถูก set โดย LevelController
    private IncontinenceLevelController controller;
    private IncontinenceType correctType;
    private bool answered = false;
    private Image feedbackBg; // parent Image ของ feedbackText
    private string currentQuestion;

    /// <summary>ตั้งค่า panel สำหรับเคสใหม่</summary>
    public void Setup(IncontinenceLevelController ctrl, IncontinenceType correct, string typeQuestion = "จากตัวเลือกต่อไปนี้ เลือกประเภทของ Urinary Incontinence")
    {
        controller = ctrl;
        correctType = correct;
        answered = false;

        // Cache parent Image once
        if (feedbackBg == null && feedbackText)
            feedbackBg = feedbackText.transform.parent.GetComponent<Image>();

        currentQuestion = string.IsNullOrEmpty(typeQuestion) ? "จากตัวเลือกต่อไปนี้ เลือกประเภทของ Urinary Incontinence" : typeQuestion;

        // Reset UI (แสดงคำถามเป็นค่าขั้งต้นแทนที่จะเป็นค่าว่าง)
        SetFeedback(currentQuestion);
        ResetButtonColors();

        // Bind buttons
        for (int i = 0; i < typeButtons.Length; i++)
        {
            if (typeButtons[i] == null) continue;
            int index = i; // capture
            typeButtons[i].onClick.RemoveAllListeners();
            typeButtons[i].onClick.AddListener(() => OnTypeClicked(index));
            typeButtons[i].interactable = true;
        }
    }

    private void OnTypeClicked(int index)
    {
        if (answered) return; // กัน double-click ตอนกำลังเปลี่ยน stage

        IncontinenceType selected = (IncontinenceType)index;

        // Reset สีก่อน
        ResetButtonColors();

        if (selected == correctType)
        {
            // --- ถูก ---
            answered = true;
            if (typeButtons[index]) typeButtons[index].image.color = correctColor;
            SetFeedback("ถูกต้อง!");

            Debug.Log($"[TypePanel] Correct! Selected: {selected}");

            // แจ้ง Controller (รอ 1 วิ เพื่อให้ผู้เล่นเห็น feedback)
            if (controller) Invoke(nameof(NotifyCorrect), 1.0f);
        }
        else
        {
            // --- ผิด ---
            if (typeButtons[index]) typeButtons[index].image.color = wrongColor;
            SetFeedback("ยังไม่ถูกต้อง ลองทบทวนลักษณะอาการอีกครั้ง");

            // รีเซ็ตข้อความช่วยเหลือให้กลับมาเป็นคำถามหลังจากผ่านไป 2 วินาที (ถ้าให้กลับ)
            // หรือแค่ทิ้งไว้สักพัก (ในที่นี้ ปล่อยให้มันค้างไว้ก่อน หรือจะคงไว้ แต่ feedback เป็น single choice อยู่แล้ว)

            Debug.Log($"[TypePanel] Wrong! Selected: {selected}, Correct: {correctType}");
        }
    }

    private void NotifyCorrect()
    {
        if (controller) controller.OnTypeAnsweredCorrectly();
    }

    /// <summary>ตั้งค่า feedback text พร้อมเปิด/ปิด parent Image GameObject</summary>
    private void SetFeedback(string msg)
    {
        if (feedbackText) feedbackText.text = msg;
        if (feedbackBg)
        {
            // ถ้ามีข้อความ, เปิด Background
            // ถัาไม่มีอะไรเลย ก็ปิดไป
            feedbackBg.gameObject.SetActive(!string.IsNullOrEmpty(msg));
        }
    }

    private void ResetButtonColors()
    {
        foreach (var btn in typeButtons)
        {
            if (btn && btn.image) btn.image.color = defaultColor;
        }
    }
}
