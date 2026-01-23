using UnityEngine;

public class ImmobilityStageManager : MonoBehaviour
{
    // กำหนดสถานะต่างๆ ของด่านนี้ เพื่อให้รู้ว่าตอนนี้ผู้เล่นอยู่ขั้นตอนไหน
    public enum StageState
    {
        Idle,               // เริ่มต้น ยังไม่ได้ทำอะไร
        PatientTurned,      // พลิกตัวคนไข้แล้ว (รอตรวจแผล)
        GradeQuestion,      // กำลังตอบคำถามเรื่องระดับแผล (Single Choice)
        CongratsAfterGrade, // ตอบถูก แสดงความยินดีสั้นๆ
        PillowPlacement,    // ช่วงลากหมอนไปวาง (Drag & Drop)
        KnowledgeSheet,     // [NEW] ช่วงอ่านใบความรู้ (แทรกเข้ามาก่อนคำถามท้าย)
        TurnOverQuestion,   // กำลังตอบคำถามหลังพลิกตัว (Multi-Select)
        Complete            // จบด่าน
    }

    [Header("State Checking")]
    public StageState state = StageState.Idle; // ดูสถานะปัจจุบันได้ใน Inspector

    [Header("Result System")] 
    public UniversalResultUI resultUI; // [NEW] ลาก Universal_ResultPanel มาใส่ตรงนี้ (หน้าจอเขียวๆ ตอนชนะ)

    [Header("UI Panels")]
    [SerializeField] private GameObject panelGradeQuestion; // คำถามแรก (ระดับแผล)
    [SerializeField] private GameObject panelCongrats;      // ข้อความชมเชย
    [SerializeField] private GameObject panelTurnOver;      // คำถามสุดท้าย (Multi-select)
    
    [Header("New Features")] 
    [SerializeField] private GameObject panelKnowledge;     // [NEW] ใบความรู้ (ใส่ Panel ที่มีปุ่ม "เข้าใจแล้ว")

    [Header("Patient Phase Objects")]
    [SerializeField] private GameObject patientDummy;       // ตัวคนไข้
    [SerializeField] private SpriteRenderer patientRenderer; // ตัว render รูปคนไข้ (เอาไว้เปลี่ยนรูป)
    [SerializeField] private Sprite lateralSprite;          // รูปตอนนอนตะแคง
    [SerializeField] private GameObject woundClickArea;     // จุดที่ให้คลิกดูแผล (Collider)
    
    [Header("Pillow Objects")]
    [SerializeField] private GameObject pillow;             // ตัวหมอน (Draggable)
    [SerializeField] private GameObject pillowDropZone;     // จุดวางหมอน (DropZone)

    [Header("Timing settings")]
    [SerializeField] private float congratsAutoCloseSeconds = 1.0f; // เวลาหน่วงปิดหน้าชมเชย

    // ตัวแปรเช็คว่า UI กำลังบังจออยู่ไหม (เผื่อใช้ป้องกันการกดทะลุ)
    public bool IsUIBlockingInput { get; private set; }

    private void Awake()
    {
        SetAllOff(); // ปิดทุกอย่างก่อนเพื่อความชัวร์
        state = StageState.Idle;
    }

    private void Start()
    {
        // ซ่อนหน้า Result ไว้ก่อน
        if (resultUI) resultUI.gameObject.SetActive(false);

        // เริ่มเกม: ให้เห็นคนไข้ แต่ซ่อนแผลและหมอนไว้
        if (patientDummy) patientDummy.SetActive(true);
        if (woundClickArea) woundClickArea.SetActive(false);
    }

    // ฟังก์ชันสำหรับรีเซ็ตหน้าจอ (ปิด UI ทั้งหมด)
    private void SetAllOff()
    {
        if (panelGradeQuestion) panelGradeQuestion.SetActive(false);
        if (panelCongrats) panelCongrats.SetActive(false);
        if (panelTurnOver) panelTurnOver.SetActive(false);
        if (panelKnowledge) panelKnowledge.SetActive(false); // [NEW] ปิดใบความรู้

        if (pillow) pillow.SetActive(false);
        if (pillowDropZone) pillowDropZone.SetActive(false);
        if (woundClickArea) woundClickArea.SetActive(false);

        IsUIBlockingInput = false;
    }

    // --- STEP 1: พลิกตัวคนไข้ ---
    // เรียกโดย: การคลิกที่ตัวคนไข้ (ผ่าน Event Trigger หรือ Collider)
    public void OnPatientClicked()
    {
        if (state != StageState.Idle) return; // กดได้แค่ตอนเริ่มเกมเท่านั้น

        // เปลี่ยนรูปเป็นนอนตะแคง
        if (patientRenderer && lateralSprite) 
        {
            patientRenderer.sprite = lateralSprite;
        }

        // เปิด Collider แผล ให้กดต่อได้
        if (woundClickArea) woundClickArea.SetActive(true);

        // อัปเดตสถานะ
        state = StageState.PatientTurned;
        Debug.Log("Patient Turned: Ready to inspect wound.");
    }

    // --- STEP 2: ตรวจแผล ---
    // เรียกโดย: การคลิกที่แผล (WoundClickArea)
    public void OnWoundClicked()
    {
        if (state != StageState.PatientTurned) return; // ต้องพลิกตัวก่อนถึงจะกดแผลได้

        StartGradeQuestion();
    }

    // เริ่มคำถามแรก (ระดับแผล)
    public void StartGradeQuestion()
    {
        state = StageState.GradeQuestion;
        IsUIBlockingInput = true;

        if (panelGradeQuestion) panelGradeQuestion.SetActive(true);
    }

    // เมื่อตอบคำถามแรกถูก (เรียกโดย script GradeQuestionUI)
    public void OnGradeAnsweredCorrect()
    {
        if (state != StageState.GradeQuestion) return;

        // ปิดคำถามแรก
        if (panelGradeQuestion) panelGradeQuestion.SetActive(false);

        // แสดงความยินดีชั่วคราว
        state = StageState.CongratsAfterGrade;
        if (panelCongrats) panelCongrats.SetActive(true);

        // รอสักครู่ แล้วค่อยไปขั้นตอนวางหมอน
        Invoke(nameof(AdvanceToPillowPlacement), congratsAutoCloseSeconds);
    }

    // เตรียมเข้าสู่ขั้นตอนวางหมอน
    private void AdvanceToPillowPlacement()
    {
        if (panelCongrats) panelCongrats.SetActive(false); // ปิดหน้ายินดี

        state = StageState.PillowPlacement;
        IsUIBlockingInput = false;

        // เปิดหมอนและจุดวางหมอนขึ้นมา
        if (pillow) 
        {
            pillow.SetActive(true);
            // สั่งปลดล็อคหมอนให้ลากได้ (ถ้า Script Draggable มีตัวแปร isLocked)
            var dragScript = pillow.GetComponent<Draggable2D>();
            if (dragScript) dragScript.isLocked = false;
        }
        if (pillowDropZone) pillowDropZone.SetActive(true);
    }

    // --- STEP 3: วางหมอน ---
    // เรียกโดย: Script DropZone2D เมื่อวางหมอนถูก
    public void OnPillowPlacedCorrect()
    {
        if (state != StageState.PillowPlacement) return;

        // [NEW] แทนที่จะไปคำถามเลย เราเปลี่ยนไปหน้า "ใบความรู้" ก่อน
        state = StageState.KnowledgeSheet;
        IsUIBlockingInput = true;

        // เปิดใบความรู้ขึ้นมา
        if (panelKnowledge) panelKnowledge.SetActive(true); 
        
        Debug.Log("Pillow Placed -> Show Knowledge Sheet");
    }

    // --- STEP 4: อ่านใบความรู้ ---
    // [NEW] เรียกโดย: ปุ่ม "ถัดไป/เข้าใจแล้ว" บน Panel_Knowledge
    public void OnKnowledgeReadConfirm()
    {
        if (state != StageState.KnowledgeSheet) return;

        // ปิดใบความรู้
        if (panelKnowledge) panelKnowledge.SetActive(false);

        // เข้าสู่ Quiz สุดท้ายจริงๆ
        state = StageState.TurnOverQuestion;
        if (panelTurnOver) panelTurnOver.SetActive(true);
        
        Debug.Log("Knowledge Read -> Start Quiz");
    }
    
    // --- STEP 5: คำถามสุดท้าย (Multi-Select) ---
    // เรียกโดย: Script TurnOverQuestion เมื่อตอบครบทุกข้อ
    public void OnTurnOverAnsweredCorrect()
    {
        if (state != StageState.TurnOverQuestion) return;

        // ปิดหน้าคำถาม
        if (panelTurnOver) panelTurnOver.SetActive(false);
        IsUIBlockingInput = false;

        // จบเกม
        state = StageState.Complete;
        Debug.Log("Immobility stage complete.");

        // [NEW] เรียกหน้า Result รวม (สีเขียว)
        if (resultUI) resultUI.ShowResult(true);
    }
}