// ImmobilityStageManager.cs
using UnityEngine;

public class ImmobilityStageManager : MonoBehaviour
{
    // กำหนดสถานะขั้นตอนต่างๆ
    public enum StageState
    {
        Idle,               // Rest
        PatientTurned,      // Lateral
        InspectReady,       // Inspect + chosen wound active/clickable
        GradeQuestion,
        CongratsAfterGrade,
        PillowPlacement,
        TurnOverQuestion,
        Complete,
        Idle,               // เริ่มต้น (ท่านอนหงาย)
        PatientTurned,      // พลิกตัวแล้ว (ท่านอนตะแคง)
        InspectReady,       // [NEW] ซูมดูแผล + สุ่มแผลเสร็จแล้ว (พร้อมให้กดเลือก)
        GradeQuestion,      // กำลังตอบคำถามเรื่องแผล
        CongratsAfterGrade, // ชมเชยหลังตอบถูก
        PillowPlacement,    // ช่วงวางหมอน
        KnowledgeSheet,     // [FLOW] ช่วงอ่านใบความรู้
        TurnOverQuestion,   // ช่วงตอบคำถามสุดท้าย (Multi-Select)
        Complete            // จบด่าน
    }

    [Header("State")]
    public StageState state = StageState.Idle;
    public StageState state = StageState.Idle;

    [Header("Result System")]
    public UniversalResultUI resultUI; // [FLOW] หน้าจอผลลัพธ์ตอนจบ

    [Header("UI Panels")]
    [SerializeField] private GameObject panelGradeQuestion;
    [SerializeField] private GameObject panelCongrats;
    [SerializeField] private GameObject panelTurnOver;
    [SerializeField] private GameObject panelKnowledge; // [FLOW] ใบความรู้

    [Header("Patient Position GameObjects (Animator-based)")]
    [SerializeField] private GameObject restPositionGO;
    [SerializeField] private GameObject lateralPositionGO;
    [SerializeField] private GameObject inspectPositionGO;

    [Header("Wound Variants (Grade 1–4)")]
    [Tooltip("Index 0=Grade1, 1=Grade2, 2=Grade3, 3=Grade4")]
    [SerializeField] private GameObject[] woundVariants;

    [Header("Chosen Exam Trigger (Runtime)")]
    [SerializeField] private GameObject examTriggerGO; // runtime-selected wound GO

    [Header("Roll behavior")]
    [SerializeField] private bool rerollEveryEnterInspect = false;

    [Header("Patient Position GameObjects")]
    // ใช้ GameObject แทน SpriteRenderer เพื่อการจัดการ Animation/Prop ที่ซับซ้อนขึ้นได้
    [SerializeField] private GameObject restPositionGO;    // ท่านอนหงาย (เริ่มต้น/วางหมอน)
    [SerializeField] private GameObject lateralPositionGO; // ท่านอนตะแคง (ตอนพลิกตัว)
    [SerializeField] private GameObject inspectPositionGO; // ท่าซูมดูแผล (ตอน Inspect)

    [Header("Wound Variants (Random Logic)")]
    [Tooltip("ใส่ Prefab แผลต่างๆ เรียงตามลำดับ: Index 0=Grade1, 1=Grade2, ...")]
    [SerializeField] private GameObject[] woundVariants;
    
    // เก็บตัวแปรแผลที่สุ่มได้ในรอบนี้
    [SerializeField] private GameObject examTriggerGO; 
    
    [Header("Settings")]
    [SerializeField] private bool rerollEveryEnterInspect = false; // สุ่มแผลใหม่ทุกครั้งที่กดดูไหม?
    [SerializeField] private float congratsAutoCloseSeconds = 1.0f;

    [Header("Pillow Objects")]
    [SerializeField] private GameObject pillow;
    [SerializeField] private GameObject pillowDropZone;

    public bool IsUIBlockingInput { get; private set; }

    private int selectedWoundGrade = 0; // 1..4
    public int CurrentWoundGrade => selectedWoundGrade;

    // ตัวแปรเก็บเฉลย (เอาไว้ให้ GradeQuestionUI ดึงไปเช็ค)
    private int selectedWoundGrade = 0; 
    public int CurrentWoundGrade => selectedWoundGrade; // Property ให้คนอื่นอ่านค่าได้

    private void Awake()
    {
        // Turn off UI
        SetAllOff();
        
        // ตั้งค่าเริ่มต้น
        IsUIBlockingInput = false;
        selectedWoundGrade = 0;
        examTriggerGO = null;
        
        // ซ่อนแผลทั้งหมด และเริ่มที่ท่า Rest
        HideAllWounds();
        ApplyPositionVisual(StageState.Idle);

        state = StageState.Idle;
    }

    private void Start()
    {
        if (resultUI) resultUI.gameObject.SetActive(false);
    }

    private void SetAllOff()
    {
        // ปิด UI ทั้งหมด
        if (panelGradeQuestion) panelGradeQuestion.SetActive(false);
        if (panelCongrats) panelCongrats.SetActive(false);
        if (panelTurnOver) panelTurnOver.SetActive(false);
        if (panelKnowledge) panelKnowledge.SetActive(false);

        // Turn off pillow flow
        // ปิดหมอน
        if (pillow) pillow.SetActive(false);
        if (pillowDropZone) pillowDropZone.SetActive(false);

        // Start at Rest
    // =================================================
    // STEP 1 & 2: Patient Interaction (Turn & Inspect)
        selectedWoundGrade = 0;
        examTriggerGO = null;

        ApplyPositionVisual(StageState.Idle);
        HideAllWounds();

        state = StageState.Idle;}
    // =================================================

    // =========================
    // Patient click transitions
    // =========================
    // ฟังก์ชันนี้ผูกกับปุ่มกดที่ตัวคนไข้ (เช่น Invisible Button หรือ Collider)
    public void OnPatientClicked()
    {
        if (IsUIBlockingInput) return;

        if (state == StageState.Idle)
        {
            ApplyPositionVisual(StageState.PatientTurned);
            state = StageState.PatientTurned;
            return;
        }

        if (state == StageState.PatientTurned)
        {
            EnterInspect();
            return;
        }

        // Beyond this, ignore patient clicks by your spec
    }

    private void EnterInspect()
    {
        ApplyPositionVisual(StageState.InspectReady);

        // Choose wound (and set examTriggerGO)
        if (rerollEveryEnterInspect || examTriggerGO == null || selectedWoundGrade == 0)
            RollRandomWound();

        state = StageState.InspectReady;
    }

    private void ApplyPositionVisual(StageState target)
    {
        bool rest    = target == StageState.Idle;
        bool lateral = target == StageState.PatientTurned;
        bool inspect = target == StageState.InspectReady;

        if (restPositionGO)    restPositionGO.SetActive(rest);
        if (lateralPositionGO) lateralPositionGO.SetActive(lateral);
        if (inspectPositionGO) inspectPositionGO.SetActive(inspect);

        if (!inspect)
        {
            // Ensure no wound is clickable/visible outside Inspect
            examTriggerGO = null;
            selectedWoundGrade = 0;
            HideAllWounds();
        }
        if (IsUIBlockingInput) return;

        // ถ้ายังนอนหงาย -> ให้พลิกตัว (Lateral)
        if (state == StageState.Idle)
        {
            ApplyPositionVisual(StageState.PatientTurned);
            state = StageState.PatientTurned;
            Debug.Log("Patient Turned (Lateral)");
            return;
        }

        // ถ้าพลิกตัวแล้ว -> ให้เข้าโหมดส่องแผล (Inspect)
        if (state == StageState.PatientTurned)
        {
            EnterInspect();
            return;
        }
    }

    // เข้าสู่โหมดส่องแผล และสุ่มแผล
    private void EnterInspect()
    {
        ApplyPositionVisual(StageState.InspectReady);

        // ถ้ายังไม่มีแผล หรือตั้งให้สุ่มใหม่ตลอด -> สุ่มแผลเลย
        if (rerollEveryEnterInspect || examTriggerGO == null || selectedWoundGrade == 0)
        {
            RollRandomWound();
        }

        state = StageState.InspectReady;
    }

    // ฟังก์ชันจัดการเปิด/ปิด Model คนไข้ตามท่าทาง
    private void ApplyPositionVisual(StageState target)
    {
        bool rest    = target == StageState.Idle;
        bool lateral = target == StageState.PatientTurned;
        bool inspect = target == StageState.InspectReady;

        if (restPositionGO)    restPositionGO.SetActive(rest);
        if (lateralPositionGO) lateralPositionGO.SetActive(lateral);
        if (inspectPositionGO) inspectPositionGO.SetActive(inspect);

        // ถ้าไม่ใช่หน้า Inspect ให้ซ่อนแผลทิ้งไปเลย
        if (!inspect)
        {
            if (examTriggerGO) examTriggerGO.SetActive(false); 
        }
        // ถ้าหน้า Inspect ให้เปิดแผลที่สุ่มไว้
        else if (inspect && examTriggerGO)
        {
            examTriggerGO.SetActive(true);
        }
    }

    // =================================================
    // Wound Randomization Logic
    // =================================================

    private void RollRandomWound()
    {
        if (woundVariants == null || woundVariants.Length == 0) return;

        HideAllWounds(); // ปิดอันเก่าก่อน

        // สุ่ม Index
        int index = Random.Range(0, woundVariants.Length); 
        
        examTriggerGO = woundVariants[index]; // เก็บ GameObject แผลที่ได้
        selectedWoundGrade = index + 1;       // เก็บเกรด (index 0 คือ grade 1)

        if (examTriggerGO) examTriggerGO.SetActive(true);

        Debug.Log($"Rolled Wound: Grade {selectedWoundGrade}");
    }

    private void HideAllWounds()
    {
        if (woundVariants == null) return;
        foreach (var w in woundVariants)
        {
            if (w) w.SetActive(false);
        }
    }

    // =========================
    // Called by ExamTrigger.cs
    // =========================
    public void OnExamTriggerClicked(GameObject caller);
    // ฟังก์ชันนี้ให้ Script ที่ติดอยู่กับแผล (ExamTrigger) เรียกเมื่อถูกกด
    public void OnExamTriggerClicked(GameObject caller)
    {
        if (IsUIBlockingInput) return;
        if (state != StageState.InspectReady) return;

        // Only accept clicks from the chosen wound
        if (caller != examTriggerGO) return;
        if (IsUIBlockingInput) return;
        if (state != StageState.InspectReady) return;

        // ป้องกันการกดแผลผิดอัน (เผื่อมีบั๊กแสดงผลซ้อน)
        if (caller != examTriggerGO) return;

        StartGradeQuestion();
    }

    public void StartGradeQuestion()
    {
        state = StageState.GradeQuestion;
        IsUIBlockingInput = true;

        if (panelGradeQuestion) panelGradeQuestion.SetActive(true);
    }

    public void OnGradeAnsweredCorrect()
    {
        if (state != StageState.GradeQuestion) return;

        if (panelGradeQuestion) panelGradeQuestion.SetActive(false);

        state = StageState.CongratsAfterGrade;
        if (panelCongrats) panelCongrats.SetActive(true);

        Invoke(nameof(AdvanceToPillowPlacement), congratsAutoCloseSeconds);
    }

    private void AdvanceToPillowPlacement()
    {
        if (panelCongrats) panelCongrats.SetActive(false);

        state = StageState.PillowPlacement;
        IsUIBlockingInput = false;

        // === FORCE PATIENT TO REST POSITION DURING DRAG PHASE ===
        if (restPositionGO)    restPositionGO.SetActive(true);
        if (lateralPositionGO) lateralPositionGO.SetActive(false);
        if (inspectPositionGO) inspectPositionGO.SetActive(false);

        // Clear wound / exam state
        examTriggerGO = null;
        selectedWoundGrade = 0;
        HideAllWounds();

        // === ENABLE PILLOW DRAG ===
        if (pillow)
        // [IMPORTANT] บังคับคนไข้กลับท่านอนหงาย เพื่อให้วางหมอนได้
        ApplyPositionVisual(StageState.Idle); 
        
        // เคลียร์ค่าแผลต่างๆ
        examTriggerGO = null;
        selectedWoundGrade = 0;
        HideAllWounds();

        // เปิดระบบหมอน
        if (pillow)
        {
            pillow.SetActive(true);

            var dragScript = pillow.GetComponent<Draggable2D>();
            if (dragScript) dragScript.isLocked = false;
        }

        if (pillowDropZone) pillowDropZone.SetActive(true);
    }


    // =================================================
    // STEP 4 & 5: Pillow -> Knowledge -> Quiz -> Result
    // =================================================

    public void OnPillowPlacedCorrect()
    {
        if (state != StageState.PillowPlacement) return;

        // [FLOW] วางหมอนเสร็จ -> ไปใบความรู้
        state = StageState.KnowledgeSheet;
        IsUIBlockingInput = true;

        if (panelKnowledge) panelKnowledge.SetActive(true);
        
        Debug.Log("Pillow Placed -> Show Knowledge");
    }

    // ปุ่ม "ถัดไป" บนใบความรู้เรียกฟังก์ชันนี้
    public void OnKnowledgeReadConfirm()
    {
        if (state != StageState.KnowledgeSheet) return;

        if (panelKnowledge) panelKnowledge.SetActive(false);

        // [FLOW] เข้าสู่ Quiz ท้ายบท
        state = StageState.TurnOverQuestion;
        if (panelTurnOver) panelTurnOver.SetActive(true);
    }

    // ตอบ Quiz ท้ายบทครบ
    public void OnTurnOverAnsweredCorrect()
    {
        if (state != StageState.TurnOverQuestion) return;

        if (panelTurnOver) panelTurnOver.SetActive(false);
        IsUIBlockingInput = false;

        state = StageState.Complete;
        Debug.Log("Immobility Stage Complete!");

        // [FLOW] โชว์หน้า Result
        if (resultUI) resultUI.ShowResult(true);
    }
}
