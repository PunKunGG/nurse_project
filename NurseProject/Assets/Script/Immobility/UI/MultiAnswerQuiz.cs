using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MultiAnswerQuiz : MonoBehaviour
{
    [Header("Manager Reference")]
    public ImmobilityStageManager stageManager; // ลาก Manager มาใส่

    [Header("Buttons Configuration")]
    // ลากปุ่มที่เป็น "คำตอบที่ถูก" มาใส่ใน List นี้ให้ครบ
    public List<Button> correctButtons; 
    
    // ลากปุ่มที่เป็น "คำตอบที่ผิด" มาใส่ใน List นี้
    public List<Button> wrongButtons;

    [Header("Settings")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    private int correctFoundCount = 0;

    void Start()
    {
        // 1. ตั้งค่าปุ่มที่ถูก
        foreach (Button btn in correctButtons)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnCorrectClicked(btn));
        }

        // 2. ตั้งค่าปุ่มที่ผิด
        foreach (Button btn in wrongButtons)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnWrongClicked(btn));
        }
    }

    // เมื่อเริ่มเปิดหน้านี้ (ทุกครั้งที่ Panel ถูก Active) ให้รีเซ็ตค่า
    void OnEnable()
    {
        correctFoundCount = 0;
        ResetButtonColors();
    }

    void OnCorrectClicked(Button btn)
    {
        // เปลี่ยนสีข้อความข้างในเป็นสีเขียว
        TextMeshProUGUI txt = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (txt) txt.color = correctColor;

        // ล็อคปุ่มไม่ให้กดซ้ำ
        btn.interactable = false;

        // นับคะแนน
        correctFoundCount++;

        // เช็คว่าหาเจอครบทุกข้อหรือยัง?
        if (correctFoundCount >= correctButtons.Count)
        {
            Debug.Log("Found all correct answers!");
            Invoke(nameof(FinishQuiz), 1.0f); // รอ 1 วินาทีแล้วค่อยจบ
        }
    }

    void OnWrongClicked(Button btn)
    {
        // เปลี่ยนสีข้อความข้างในเป็นสีแดง
        TextMeshProUGUI txt = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (txt) txt.color = wrongColor;

        // (Optional) จะล็อคปุ่มผิดด้วยก็ได้ถ้าไม่อยากให้กดซ้ำ
        // btn.interactable = false; 
    }

    void FinishQuiz()
    {
        // แจ้ง Manager ว่าจบส่วนนี้แล้ว (ไปหน้า Result)
        if (stageManager)
        {
            stageManager.OnTurnOverAnsweredCorrect();
        }
    }

    // ฟังก์ชันช่วยรีเซ็ตสีปุ่ม (เผื่อเล่นใหม่)
    void ResetButtonColors()
    {
        foreach (Button btn in correctButtons)
        {
            btn.interactable = true;
            TextMeshProUGUI txt = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (txt) txt.color = Color.black; // หรือสีตั้งต้นของคุณ
        }
        foreach (Button btn in wrongButtons)
        {
            btn.interactable = true;
            TextMeshProUGUI txt = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (txt) txt.color = Color.black;
        }
    }
}