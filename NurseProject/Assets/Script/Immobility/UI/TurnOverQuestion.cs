using UnityEngine;
using UnityEngine.UI;
using TMPro; // ต้องใช้สำหรับเปลี่ยนสีตัวอักษร
using System.Collections.Generic; // ต้องใช้สำหรับ List

public class TurnOverQuestion : MonoBehaviour
{
    [Header("Manager Reference")]
    [SerializeField] private ImmobilityStageManager stageManager;

    [Header("Answer Buttons Configuration")]
    // ใส่ปุ่มที่เป็นคำตอบ "ถูก" ทั้งหมดลงใน List นี้
    [SerializeField] private List<Button> correctButtons; 
    
    // ใส่ปุ่มที่เป็นคำตอบ "ผิด" ทั้งหมดลงใน List นี้
    [SerializeField] private List<Button> wrongButtons;

    [Header("Color Settings")]
    [SerializeField] private Color correctTextColor = Color.green; // สีเมื่อตอบถูก
    [SerializeField] private Color wrongTextColor = Color.red;     // สีเมื่อตอบผิด
    [SerializeField] private Color defaultTextColor = Color.black; // สีปกติ (เอาไว้รีเซ็ต)

    private int correctFoundCount = 0; // ตัวนับว่าเจอกี่ข้อแล้ว

    private void Start()
    {
        // วนลูปตั้งค่าปุ่ม "ถูก" ทุกปุ่ม
        foreach (Button btn in correctButtons)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnCorrectClicked(btn));
        }

        // วนลูปตั้งค่าปุ่ม "ผิด" ทุกปุ่ม
        foreach (Button btn in wrongButtons)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnWrongClicked(btn));
        }
    }

    private void OnEnable()
    {
        // ทุกครั้งที่หน้านี้เปิดขึ้นมา ให้รีเซ็ตค่าเสมอ
        ResetQuiz();
    }

    // --- Logic เมื่อกดปุ่มถูก ---
    private void OnCorrectClicked(Button btn)
    {
        // 1. เปลี่ยนสีตัวอักษรเป็นเขียว
        ChangeButtonTextColor(btn, correctTextColor);

        // 2. ล็อคปุ่มไม่ให้กดซ้ำ
        btn.interactable = false;

        // 3. บวกคะแนน
        correctFoundCount++;

        // 4. เช็คว่าครบหรือยัง?
        if (correctFoundCount >= correctButtons.Count)
        {
            Debug.Log("ครบแล้ว! ไปหน้าสรุปผล");
            Invoke(nameof(CompleteStage), 1.0f); // รอ 1 วิ แล้วค่อยจบ
        }
    }

    // --- Logic เมื่อกดปุ่มผิด ---
    private void OnWrongClicked(Button btn)
    {
        // เปลี่ยนสีตัวอักษรเป็นแดง
        ChangeButtonTextColor(btn, wrongTextColor);
        
        // (เลือกได้) จะล็อคปุ่มด้วยไหม? ถ้าไม่ล็อคก็ลบบรรทัดล่างทิ้ง
        btn.interactable = false; 
    }

    // ฟังก์ชันจบด่าน
    private void CompleteStage()
    {
        if (stageManager)
        {
            stageManager.OnTurnOverAnsweredCorrect();
        }
    }

    // ฟังก์ชันช่วยเปลี่ยนสี (รองรับ TextMeshPro)
    private void ChangeButtonTextColor(Button btn, Color color)
    {
        TextMeshProUGUI txt = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (txt)
        {
            txt.color = color;
        }
    }

    // ฟังก์ชันรีเซ็ต (เผื่อผู้เล่นกด Replay หรือเริ่มใหม่)
    private void ResetQuiz()
    {
        correctFoundCount = 0;

        // รีเซ็ตปุ่มถูก
        foreach (Button btn in correctButtons)
        {
            btn.interactable = true;
            ChangeButtonTextColor(btn, defaultTextColor);
        }

        // รีเซ็ตปุ่มผิด
        foreach (Button btn in wrongButtons)
        {
            btn.interactable = true;
            ChangeButtonTextColor(btn, defaultTextColor);
        }
    }
}