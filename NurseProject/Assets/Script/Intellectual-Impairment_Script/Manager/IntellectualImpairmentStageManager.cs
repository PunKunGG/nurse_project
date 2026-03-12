using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro; // อย่าลืมบรรทัดนี้ เพราะเราจะใช้ TextMeshPro

public class IntellectualImpairmentStageManager : MonoBehaviour
{
    [Header("Phase 1: Cutscene")]
    // public GameObject cutscenePanel;  // Moved to EnvironmentCutsceneHandler
    // public VideoPlayer videoPlayer;   // Moved to EnvironmentCutsceneHandler  
    
    [Header("Phase 2: Quiz")]
    public GameObject quizPanel;     
    public float panelSlideDuration = 0.5f; // ระยะเวลาอนิเมชันเลื่อนขึ้น

    [Header("Phase 3: Gameplay & UI")]
    public GameObject environmentParent;
    public TextMeshProUGUI objectiveText; // ข้อความภารกิจบนหน้าจอ*
    public TextMeshProUGUI objectiveCountText; // จำนวนภารกิจ (e.g., 1/3)

    [Header("Result System")] // แสดงผลลัพธ์ตอนจบด่าน
    public UniversalResultUI resultUI; // ลาก UniversalResultPanel มาใส่ตรงนี้
    public KnowledgePopupUI knowledgePopup;
    
    [Header("Game Logic")]
    public int totalHazards = 0;     
    public int fixedHazards = 0;    

    public EnvironmentCutsceneHandler cutsceneHandler; // Assign this in Inspector

    private Vector2 quizPanelOriginalPos;

    void Start()
    {
        // เก็บตำแหน่งเดิมของ QuizPanel ไว้ใช้อนิเมชัน
        if (quizPanel)
        {
            RectTransform rect = quizPanel.GetComponent<RectTransform>();
            if (rect != null) quizPanelOriginalPos = rect.anchoredPosition;
        }

        // เริ่มมา: ซ่อน Objective ก่อน (ซ่อนตัวแม่ที่เป็น Image ด้วย)
        if(objectiveText && objectiveText.transform.parent != null) 
            objectiveText.transform.parent.gameObject.SetActive(false);
        
        if(objectiveCountText && objectiveCountText.transform.parent != null && objectiveCountText.transform.parent != objectiveText.transform.parent)
             objectiveCountText.transform.parent.gameObject.SetActive(false);
        
        // Auto-assign resultUI if missing
        if (!resultUI)
        {
            resultUI = FindFirstObjectByType<UniversalResultUI>();
            if (resultUI) Debug.Log("System: Auto-assigned UniversalResultUI to Manager.");
        }

        if(resultUI) resultUI.gameObject.SetActive(false); // ซ่อนหน้าผลไว้ก่อน
        
        // Setup Handler Events if not set in Inspector
        if (cutsceneHandler)
        {
            cutsceneHandler.onCutsceneFinished.RemoveListener(OnCutsceneFinishedCallback);
            cutsceneHandler.onCutsceneFinished.AddListener(OnCutsceneFinishedCallback);
            cutsceneHandler.PlayCutscene();
        }
        else
        {
            Debug.LogError("EnvironmentCutsceneHandler is missing!");
            // Fallback: Just start quiz?
             if (quizPanel) quizPanel.SetActive(true);
        }
        
        if(quizPanel) quizPanel.SetActive(false);
    }

    // --- PHASE 1: CUTSCENE (Delegated) ---
    // ฟังก์ชันนี้เอาไว้ให้ปุ่ม Replay เรียกใช้
    public void ReplayCutscene()
    {
        Debug.Log("Replaying Cutscene...");
        if(quizPanel) quizPanel.SetActive(false); // ปิด Quiz
        
        // ซ่อน Objective Text ตอนเล่นวิดีโอซ้ำ
        if(objectiveText && objectiveText.transform.parent != null) 
            objectiveText.transform.parent.gameObject.SetActive(false);
        
        if(objectiveCountText && objectiveCountText.transform.parent != null && objectiveCountText.transform.parent != objectiveText.transform.parent)
            objectiveCountText.transform.parent.gameObject.SetActive(false);

        if (cutsceneHandler) cutsceneHandler.PlayCutscene();
    }
    
    // ฟังก์ชันสำหรับปุ่ม Skip (เรียกจาก UI Button)
    public void SkipCutscene()
    {
        Debug.Log("Skipping Cutscene...");
        if (cutsceneHandler) cutsceneHandler.StopCutscene();
    }

    // Callback from Handler
    void OnCutsceneFinishedCallback()
    {
        Debug.Log("Cutscene Finished Callback Received");
        if (quizPanel) 
        {
            quizPanel.SetActive(true);
            StartCoroutine(SlideInPanel(quizPanel));
        }          
    }

    private System.Collections.IEnumerator SlideInPanel(GameObject panel)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();
        if (rect == null) yield break;

        // ดึงตำแหน่งปลายทางจากที่เซฟไว้ตอน Start
        Vector2 targetPosition = quizPanelOriginalPos;
        
        // ตั้งค่าจุดเริ่มต้นให้อยู่ต่ำลงไปจากหน้าจอ (1000 pixels)
        Vector2 startPosition = targetPosition - new Vector2(0, 1000f); 
        rect.anchoredPosition = startPosition;

        float time = 0;
        while (time < panelSlideDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / panelSlideDuration);
            
            // Ease out cubic
            float ease = 1f - Mathf.Pow(1f - t, 3f);

            rect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, ease);
            yield return null;
        }

        rect.anchoredPosition = targetPosition;
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
        if(objectiveText && objectiveText.transform.parent != null) 
        {
            objectiveText.transform.parent.gameObject.SetActive(true);
            
            if(objectiveCountText && objectiveCountText.transform.parent != null)
                objectiveCountText.transform.parent.gameObject.SetActive(true);

            UpdateObjectiveUI();
        }
    }

    public void OnHazardFixed()
    {
        fixedHazards++;
        Debug.Log($"Hazard Fixed: {fixedHazards}/{totalHazards}");
        UpdateObjectiveUI();

        CheckLevelComplete();
    }

    private void CheckLevelComplete()
    {
        // เช็คเงื่อนไขชนะเกม
        if (fixedHazards >= totalHazards)
        {
            Debug.Log("LEVEL COMPLETE! Showing Result UI...");
            
            // **เรียกหน้า Result แสดงผลว่าผ่าน**
            if(resultUI) 
            {
                resultUI.ShowResult(
                    true, 
                    "<color=green>ภารกิจสำเร็จ!</color>",
                    "ยินดีด้วย คุณมีความรู้ความเข้าใจที่ถูกต้องเกี่ยวกับการจัดสิ่งแวดล้อม",
                    "ไปด่านต่อไป >>"
                );
            }
            else
            {
                Debug.LogError("UniversalResultUI is missing! Cannot show result.");
            }
            
            // (Optional) ซ่อน Objective Text (และกรอบ) เพื่อความสวยงาม
            if(objectiveText && objectiveText.transform.parent != null) 
                objectiveText.transform.parent.gameObject.SetActive(false);

            if(objectiveCountText && objectiveCountText.transform.parent != null && objectiveCountText.transform.parent != objectiveText.transform.parent)
                objectiveCountText.transform.parent.gameObject.SetActive(false);
        }
    }

    // Dev Tool
    [ContextMenu("Force Complete Level")]
    public void ForceComplete()
    {
        fixedHazards = totalHazards;
        CheckLevelComplete();
    }

    // ฟังก์ชันช่วยเขียน Text
    void UpdateObjectiveUI()
    {
        if(objectiveText)
        {
            objectiveText.text = "จากภาพ มีปัจจัยกระตุ้นใดบ้าง ที่ส่งผลให้ผู้ป่วยเกิดภาวะสับสนเฉียบพลัน:";
        }

        if(objectiveCountText)
        {
            objectiveCountText.text = $"{fixedHazards}/{totalHazards}";
        }
    }
}