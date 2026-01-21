using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // ถ้าใช้ TextMeshPro

public class SymptomQuizUI : MonoBehaviour
{
    [Header("Managers")]
    public EnvironmentStageManager stageManager; // ลาก Manager มาใส่

    [Header("Question UI")]
    public TextMeshProUGUI questionText; // ใส่ "อาการของผู้ป่วยบ่งบอกถึงภาวะใด"

    [Header("Buttons Setup")]
    // ลากปุ่มที่เป็นคำตอบที่ "ถูก" มาใส่ช่องนี้
    public Button correctButton; 
    
    // ลากปุ่มที่ "ผิด" ทั้งหมด 3 ปุ่ม มาใส่ List นี้
    public Button[] wrongButtons;

    void Start()
    {
        // 1. ตั้งค่าปุ่มที่ถูก
        if (correctButton)
        {
            correctButton.onClick.RemoveAllListeners();
            correctButton.onClick.AddListener(() => StartCoroutine(HandleCorrect(correctButton)));
        }

        // 2. ตั้งค่าปุ่มที่ผิดทั้งหมด
        foreach (var btn in wrongButtons)
        {
            if (btn)
            {
                Button tempBtn = btn; // ต้องสร้าง temp variable สำหรับ loop
                tempBtn.onClick.RemoveAllListeners();
                tempBtn.onClick.AddListener(() => StartCoroutine(HandleWrong(tempBtn)));
            }
        }
    }

    // --- Logic เมื่อตอบถูก ---
    IEnumerator HandleCorrect(Button btn)
    {
        // เปลี่ยนสีเขียว
        btn.image.color = Color.green; 
        Debug.Log("Correct Answer!");

        // รอ 0.5 - 1 วินาที ให้ผู้เล่นเห็นสีเขียว
        yield return new WaitForSeconds(0.8f);

        // แจ้ง Manager ว่าผ่านด่าน Quiz แล้ว -> ไปเริ่มเกมหาของ
        if(stageManager) stageManager.OnQuizCompleted();
    }

    // --- Logic เมื่อตอบผิด ---
    IEnumerator HandleWrong(Button btn)
    {
        // เปลี่ยนสีแดง
        btn.image.color = Color.red;
        Debug.Log("Wrong Answer.");

        // รอ 0.5 วินาที
        yield return new WaitForSeconds(0.5f);

        // ซ่อนปุ่มนั้นไปเลย (หายไป)
        btn.gameObject.SetActive(false);
    }
}