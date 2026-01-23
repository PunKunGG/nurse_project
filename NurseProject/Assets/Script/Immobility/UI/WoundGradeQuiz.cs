using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WoundGradeQuiz : MonoBehaviour
{
    [Header("Manager")]
    public ImmobilityStageManager stageManager;

    [Header("Buttons")]
    public Button correctButton; // ปุ่มคำตอบที่ถูก (เช่น "ระดับ 1")
    public Button[] wrongButtons; // ปุ่มคำตอบที่ผิดอื่นๆ

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText; // (Optional) ข้อความแจ้งเตือน

    void Start()
    {
        // ตั้งค่าปุ่มถูก
        if(correctButton) 
        {
            correctButton.onClick.RemoveAllListeners();
            correctButton.onClick.AddListener(OnCorrect);
        }

        // ตั้งค่าปุ่มผิด
        foreach(var btn in wrongButtons)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnWrong);
        }
    }

    void OnEnable()
    {
        if(feedbackText) feedbackText.text = ""; // ล้างข้อความเก่า
    }

    void OnCorrect()
    {
        Debug.Log("ตอบถูก! ไปวางหมอนต่อ");
        // แจ้ง Manager ว่าตอบถูกแล้ว ให้ไป Phase วางหมอน
        if(stageManager) stageManager.OnGradeAnsweredCorrect();
    }

    void OnWrong()
    {
        Debug.Log("ตอบผิด!");
        // แจ้งเตือน หรือ จะให้เกมจบเลยก็ได้
        if(feedbackText) 
        {
            feedbackText.text = "ยังไม่ถูกต้อง ลองพิจารณาลักษณะแผลอีกทีครับ";
            feedbackText.color = Color.red;
        }
        
        
    }
}