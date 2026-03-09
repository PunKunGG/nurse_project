// IncontinenceManagementPanel.cs — Stage 3: เลือกการพยาบาล (Multiple selection + Submit)
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Panel Stage 3: แสดง 10 ปุ่ม toggle ให้ผู้เล่นเลือกการพยาบาล + ปุ่ม Submit
/// ต้องเลือกครบทุกข้อที่ถูก ห้ามเกิน ห้ามขาด
/// </summary>
public class IncontinenceManagementPanel : MonoBehaviour
{
    [Header("Toggle Buttons (10 ปุ่ม — เรียงตาม enum ManagementOption)")]
    [Tooltip("ลำดับ: Kegel, Knack, BladderTrain, Prompted, TimedVoiding, CIC, CoreGait, BedMobility, EnvMod, MedReview")]
    [SerializeField] private Button[] managementButtons = new Button[10];

    [Header("Submit")]
    [SerializeField] private Button submitButton;

    [Header("Feedback")]
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Colors")]
    [SerializeField] private Color defaultColor   = Color.white;
    [SerializeField] private Color selectedColor   = new Color(0.4f, 0.7f, 1f);  // light blue
    [SerializeField] private Color correctColor    = new Color(0.2f, 0.8f, 0.3f); // green
    [SerializeField] private Color wrongColor      = new Color(0.9f, 0.3f, 0.3f); // red

    [Header("Hint System")]
    [SerializeField] private Button hintButton;
    [SerializeField] private VisualNovelIntro hintNovel;
    private string[] currentHintDialogue;

    // State
    private IncontinenceLevelController controller;
    private HashSet<ManagementOption> correctSet = new HashSet<ManagementOption>();
    private HashSet<ManagementOption> selectedSet = new HashSet<ManagementOption>();
    private string currentQuestion;
    private bool submitted = false;

    /// <summary>ตั้งค่า panel สำหรับเคสใหม่</summary>
    public void Setup(IncontinenceLevelController ctrl, List<ManagementOption> correctOptions, string[] hintDialogue, string managementQuestion = "จากตัวเลือกต่อไปนี้ เลือกวิธีการพยาบาลและการฝึกที่เหมาะสมกับผู้ป่วย")
    {
        controller = ctrl;
        submitted = false;
        currentHintDialogue = hintDialogue;

        // เก็บเฉลย
        correctSet.Clear();
        foreach (var opt in correctOptions) correctSet.Add(opt);
        selectedSet.Clear();

        currentQuestion = string.IsNullOrEmpty(managementQuestion) ? "จากตัวเลือกต่อไปนี้ เลือกวิธีการพยาบาลและการฝึกที่เหมาะสมกับผู้ป่วย" : managementQuestion;

        // Reset UI
        if (feedbackText) 
        {
            feedbackText.text = currentQuestion;
            if (feedbackText.transform.parent != null)
            {
                feedbackText.transform.parent.gameObject.SetActive(true);
            }
        }
        ResetAllButtons();

        // Bind hint button
        if (hintButton)
        {
            hintButton.onClick.RemoveAllListeners();
            hintButton.onClick.AddListener(ShowHint);
            
            // Show button only if there is a hint
            hintButton.gameObject.SetActive(currentHintDialogue != null && currentHintDialogue.Length > 0);
        }

        // Hide the visual novel panel initially
        if (hintNovel != null)
        {
            hintNovel.gameObject.SetActive(false);
        }

        // Bind toggle buttons
        for (int i = 0; i < managementButtons.Length; i++)
        {
            if (managementButtons[i] == null) continue;
            int index = i;
            managementButtons[i].onClick.RemoveAllListeners();
            managementButtons[i].onClick.AddListener(() => OnToggleClicked(index));
            managementButtons[i].interactable = true;
        }

        // Bind submit
        if (submitButton)
        {
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(OnSubmit);
            submitButton.interactable = true;
        }
    }

    private void OnToggleClicked(int index)
    {
        if (submitted) return;

        ManagementOption option = (ManagementOption)index;

        // Toggle selection
        if (selectedSet.Contains(option))
        {
            selectedSet.Remove(option);
            if (managementButtons[index]) managementButtons[index].image.color = defaultColor;
        }
        else
        {
            selectedSet.Add(option);
            if (managementButtons[index]) managementButtons[index].image.color = selectedColor;
        }

        // Clear feedback on new selection, restore the question text
        if (feedbackText) 
        {
            feedbackText.text = currentQuestion;
            if (feedbackText.transform.parent != null)
            {
                feedbackText.transform.parent.gameObject.SetActive(true);
            }
        }
    }

    private void ShowHint()
    {
        if (hintNovel != null && currentHintDialogue != null && currentHintDialogue.Length > 0)
        {
            hintNovel.gameObject.SetActive(true);
            hintNovel.StartIntro(currentHintDialogue);
        }
        else
        {
            Debug.LogWarning("[ManagementPanel] Hint requested but no Novel/Dialogue available.");
        }
    }

    private void OnSubmit()
    {
        if (submitted) return;

        // Validate: selectedSet must exactly equal correctSet
        bool isCorrect = selectedSet.SetEquals(correctSet);

        if (isCorrect)
        {
            submitted = true;

            // แสดงสีเขียวเฉพาะที่เลือกถูก
            HighlightCorrectAnswers();
            if (feedbackText) 
            {
                if (feedbackText.transform.parent != null)
                {
                    feedbackText.transform.parent.gameObject.SetActive(true);
                }
                feedbackText.text = "ถูกต้องทั้งหมด!";
            }

            Debug.Log("[ManagementPanel] All correct!");

            // แจ้ง Controller (รอ 1 วิ)
            Invoke(nameof(NotifyCorrect), 1.0f);
        }
        else
        {
            // หาว่าขาดหรือเกินอะไร
            int missing = 0;
            int extra = 0;
            foreach (var c in correctSet)
                if (!selectedSet.Contains(c)) missing++;
            foreach (var s in selectedSet)
                if (!correctSet.Contains(s)) extra++;

            string msg = "ยังไม่ถูกต้อง — ";
            if (missing > 0) msg += $"ยังขาดอีก {missing} ข้อ ";
            if (extra > 0)   msg += $"เลือกผิด {extra} ข้อ";
            
            if (feedbackText) 
            {
                if (feedbackText.transform.parent != null)
                {
                    feedbackText.transform.parent.gameObject.SetActive(true);
                }
                feedbackText.text = msg;
            }

            Debug.Log($"[ManagementPanel] Wrong! Missing: {missing}, Extra: {extra}");
        }
    }

    private void NotifyCorrect()
    {
        if (controller) controller.OnManagementAnsweredCorrectly();
    }

    private void HighlightCorrectAnswers()
    {
        for (int i = 0; i < managementButtons.Length; i++)
        {
            if (managementButtons[i] == null) continue;
            ManagementOption opt = (ManagementOption)i;

            if (correctSet.Contains(opt))
                managementButtons[i].image.color = correctColor;
            else if (selectedSet.Contains(opt))
                managementButtons[i].image.color = wrongColor;
            else
                managementButtons[i].image.color = defaultColor;
        }
    }

    private void ResetAllButtons()
    {
        foreach (var btn in managementButtons)
        {
            if (btn && btn.image) btn.image.color = defaultColor;
        }
    }
}
