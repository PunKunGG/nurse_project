using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro; // อย่าลืมบรรทัดนี้ เพราะเราจะใช้ TextMeshPro

public class EnvironmentStageManager : MonoBehaviour
{
    [Header("Phase 1: Cutscene")]
    public GameObject cutscenePanel; 
    public VideoPlayer videoPlayer;  
    
    [Header("Phase 2: Quiz")]
    public GameObject quizPanel;     

    [Header("Phase 3: Gameplay & UI")]
    public GameObject environmentParent;
    public TextMeshProUGUI objectiveText; // ข้อความภารกิจบนหน้าจอ*

    [Header("Result System")] // แสดงผลลัพธ์ตอนจบด่าน
    public UniversalResultUI resultUI; // ลาก UniversalResultPanel มาใส่ตรงนี้
    
    [Header("Game Logic")]
    public int totalHazards = 0;     
    public int fixedHazards = 0;    

    void Start()
    {
        // เริ่มมา: ซ่อน Objective ก่อน
        if(objectiveText) objectiveText.gameObject.SetActive(false);
        if(resultUI) resultUI.gameObject.SetActive(false); // ซ่อนหน้าผลไว้ก่อน
        // เริ่ม Cutscene
        PlayCutscene();
        
        if(quizPanel) quizPanel.SetActive(false);
    }

    // --- PHASE 1: CUTSCENE ---
    // --- PHASE 1: CUTSCENE ---
    void PlayCutscene()
    {
        if (cutscenePanel) cutscenePanel.SetActive(true);
        
        // *สำคัญ: Reset วิดีโอให้เริ่มใหม่เสมอ*
        if (videoPlayer)
        {
            videoPlayer.time = 0; 
            videoPlayer.Play();
            videoPlayer.loopPointReached -= OnVideoFinished; // กัน error จากการ add ซ้ำ
            videoPlayer.loopPointReached += OnVideoFinished; 
        }
        else
        {
            Debug.LogWarning("Video Player not assigned. Skipping cutscene.");
            OnVideoFinished(null);
        }
    }

    // ฟังก์ชันนี้เอาไว้ให้ปุ่ม Replay เรียกใช้
    public void ReplayCutscene()
    {
        Debug.Log("Replaying Cutscene...");
        if(quizPanel) quizPanel.SetActive(false); // ปิด Quiz
        PlayCutscene(); // เล่นวิดีโอใหม่
    }
    
    // ฟังก์ชันสำหรับปุ่ม Skip (เรียกจาก UI Button)
    public void SkipCutscene()
    {
        Debug.Log("Skipping Cutscene...");
        if (videoPlayer) videoPlayer.Stop(); // หยุดวิดีโอถ้าเล่นอยู่
        OnVideoFinished(videoPlayer);
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video Finished (or Skipped)");
        if (cutscenePanel) cutscenePanel.SetActive(false); 
        if (quizPanel) quizPanel.SetActive(true);          
    }

    // --- PHASE 2: QUIZ ---
    public void OnQuizCompleted()
    {
        Debug.Log("Quiz Done! Let's head to the next part.");
        if (quizPanel) quizPanel.SetActive(false);
        
        StartGameplay();
    }

    // --- PHASE 3: Interact ---
    void StartGameplay()
    {
        // โชว์ Objective เมื่อเริ่มหาของ
        if(objectiveText) 
        {
            objectiveText.gameObject.SetActive(true);
            UpdateObjectiveUI();
        }
    }

    public void OnHazardFixed()
    {
        fixedHazards++;
        UpdateObjectiveUI();

        // เช็คเงื่อนไขชนะเกม
        if (fixedHazards >= totalHazards)
        {
            Debug.Log("LEVEL COMPLETE!");
            
            // **เรียกหน้า Result แสดงผลว่าผ่าน**
            if(resultUI) resultUI.ShowResult(true);
            
            // (Optional) ซ่อน Objective Text เพื่อความสวยงาม
            if(objectiveText) objectiveText.gameObject.SetActive(false);
        }
    }

    // ฟังก์ชันช่วยเขียน Text
    void UpdateObjectiveUI()
    {
        if(objectiveText)
        {
            // ตัวอย่างข้อความ: "จัดการสิ่งแวดล้อม: 1/3"
            objectiveText.text = $"จากภาพ มีปัจจัยกระตุ้นใดบ้าง ที่ส่งผลให้ผู้ป่วยเกิดภาวะสับสนเฉียบพลัน: {fixedHazards}/{totalHazards}";
        }
    }
}