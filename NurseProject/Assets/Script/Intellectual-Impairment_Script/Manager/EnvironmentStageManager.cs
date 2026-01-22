using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video; // ต้องใช้สำหรับ VideoPlayer

public class EnvironmentStageManager : MonoBehaviour
{
    [Header("Phase 1: Cutscene")]
    public GameObject cutscenePanel; // Panel ที่มี RawImage
    public VideoPlayer videoPlayer;  // ตัวเล่นวิดีโอ
    
    [Header("Phase 2: Quiz")]
    public GameObject quizPanel;     // Panel คำถาม

    [Header("Phase 3: Gameplay")]
    public GameObject environmentParent; // Parent ที่เก็บของ Interactive ทั้งหมด
    public int totalHazards = 0;     // จำนวนของที่ต้องแก้ (ระบบจะนับเอง)
    public int fixedHazards = 0;     // จำนวนของที่แก้ไปแล้ว

    void Start()
    {
        // เริ่มมา: เปิด Cutscene, ปิด Quiz, ปิดการกดของในฉาก
        PlayCutscene();
        if(quizPanel) quizPanel.SetActive(false);
        // ล็อคของในฉากก่อน (เดี๋ยวเขียนเพิ่ม)
    }

    // step 1: CUTSCENE 
    void PlayCutscene()
    {
        if (cutscenePanel) cutscenePanel.SetActive(true);
        if (videoPlayer)
        {
            videoPlayer.Play();
            videoPlayer.loopPointReached += OnVideoFinished; // ผูก Event เมื่อเล่นจบ
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video Finished");
        if (cutscenePanel) cutscenePanel.SetActive(false); // ปิดหน้าจอวิดีโอ
        if (quizPanel) quizPanel.SetActive(true);          // เปิดคำถาม
    }

    // step 2: QUIZ (เรียกจาก Script Quiz UI) 
    public void OnQuizCompleted()
    {
        Debug.Log("Quiz Correct! Start Gameplay.");
        if (quizPanel) quizPanel.SetActive(false);
        
        // เริ่ม Phase 3: ค้นหาสิ่งผิดปกติ
        // (ตรงนี้คุณอาจจะเปิดเสียง หรือขึ้น UI บอกว่า "จงจัดการสิ่งแวดล้อม")
    }

    //  step 3: จับผิดสิ่งแวดล้อม
    // ฟังก์ชันนี้ให้ Object ในฉากเรียกเมื่อถูกแก้ไข
    public void OnHazardFixed()
    {
        fixedHazards++;
        Debug.Log($"Fixed: {fixedHazards}/{totalHazards}");

        if (fixedHazards >= totalHazards)
        {
            Debug.Log("LEVEL COMPLETE! All hazards fixed.");
            // เรียกหน้าสรุปคะแนน หรือ จบเกมตรงนี้
        }
    }
}