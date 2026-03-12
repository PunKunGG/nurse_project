// ImmobilityStageManager.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ImmobilityStageManager : MonoBehaviour
{
    public enum StageState
    {
        Idle,               // Rest
        PatientTurned,      // Lateral
        InspectReady,       // Inspect + chosen wound active/clickable
        GradeQuestion,
        CongratsAfterGrade,
        PillowPlacement,
        TurnOverQuestion,
        SummaryQuiz,
        Complete
    }

    [Header("State")]
    public StageState state = StageState.Idle;

    [Header("UI Panels")]
    [SerializeField] private GameObject panelGradeQuestion;
    [SerializeField] private GameObject titlePanel; // New Title Panel
    [SerializeField] private ImmobilitySummaryQuiz summaryQuiz; // Reference to the new quiz
    [SerializeField] private GameObject instructionQuizPanel; // "Click at patient" prompt
    public UniversalResultUI resultUI; // Add UniversalResultUI

    [Header("Indicators")]
    [SerializeField] private GameObject inspectArrow; // Animated arrow pointing at patient ass

    [Header("Environment")]
    [SerializeField] private GameObject bedGO;

    [Header("Patient Position GameObjects")]
    [SerializeField] private GameObject restPositionGO;
    [SerializeField] private GameObject lateralPositionGO;
    [SerializeField] private GameObject inspectPositionGO;

    [Header("Wound Variants (Grade 1–4)")]
    [Tooltip("Index 0=Grade1, 1=Grade2, 2=Grade3, 3=Grade4")]
    [SerializeField] private GameObject[] woundVariants;

    [Header("Chosen Exam Trigger")]
    [SerializeField] private GameObject examTriggerGO; // runtime-selected wound GO

    [Header("Roll behavior")]
    [SerializeField] private bool rerollEveryEnterInspect = false;

    [Header("Pillow Objects (Chest & Ankle)")]
    [SerializeField] private GameObject pillowChest;
    [SerializeField] private GameObject pillowDropZoneChest;
    [SerializeField] private GameObject pillowAnkle;
    [SerializeField] private GameObject pillowDropZoneAnkle;

    [Header("Knowledge Popup")]
    [SerializeField] private GameObject knowledgePopupPanel;

    private bool pillowChestPlaced = false;
    private bool pillowAnklePlaced = false;

    [Header("Extra UI To Hide")]
    [SerializeField] private Image[] extraImagesToHide;

    [Header("Timing")]
    [SerializeField] private float gradeCorrectDelaySeconds = 2.0f; // Time to show correct answer before moving on
    [SerializeField] private float knowledgePopupDelaySeconds = 1.0f; // Time before Knowledge Popup appears

    [Header("Visual Novel Intro")]
    [SerializeField] private VisualNovelIntro introNovel;
    [TextArea(3, 10)]
    [SerializeField] private string[] initialIntroDialogue = new string[] {
        "สวัสดีค่ะ วันนี้เรามีเคสผู้ป่วยติดเตียงที่ต้องดูแล สิ่งแรกที่คุณต้องทำคือการพลิกตัวผู้ป่วยเพื่อประเมินความเสี่ยงของการเกิดแผลกดทับนะคะ ลองคลิกที่ตัวผู้ป่วยเพื่อเริ่มกันเลยค่ะ!"
    };
    [TextArea(3, 10)]
    [SerializeField] private string[] inspectIntroDialogue = new string[] {
        "ตอนนี้ผู้ป่วยถูกพลิกตัวแล้วนะคะ ลองคลิกที่รอยแดงหรือแผลกดทับที่พบเพื่อประเมินระดับความรุนแรงดูนะคะ"
    };
    [TextArea(3, 10)]
    [SerializeField] private string[] pillowIntroDialogue = new string[] {
        "เก่งมากค่ะ! หลังจากที่เราประเมินรอยแดงเสร็จแล้ว ขั้นตอนต่อไปคือการจัดท่านอนให้ผู้ป่วยใหม่เพื่อลดแรงกดทับนะคะ ลองนำหมอนไปวางรองในจุดที่ถูกต้องดูค่ะ"
    };

    public bool IsUIBlockingInput { get; private set; }

    private int selectedWoundGrade = 0; // 1..4
    public int CurrentWoundGrade => selectedWoundGrade;

    private void Awake()
    {
        // Turn off UI
        if (panelGradeQuestion) panelGradeQuestion.SetActive(false);
        if (summaryQuiz) summaryQuiz.gameObject.SetActive(false);
        if (patientWithPillowChestGO) patientWithPillowChestGO.SetActive(false);
        if (patientWithPillowAnkleGO) patientWithPillowAnkleGO.SetActive(false);
        if (patientWithBothPillowsGO) patientWithBothPillowsGO.SetActive(false);

        // Turn off pillow flow
        if (pillowChest) pillowChest.SetActive(false);
        if (pillowDropZoneChest) pillowDropZoneChest.SetActive(false);
        if (pillowAnkle) pillowAnkle.SetActive(false);
        if (pillowDropZoneAnkle) pillowDropZoneAnkle.SetActive(false);
        if (knowledgePopupPanel) knowledgePopupPanel.SetActive(false);
        if (instructionQuizPanel) instructionQuizPanel.SetActive(false);
        if (inspectArrow) inspectArrow.SetActive(false);
        
        pillowChestPlaced = false;
        pillowAnklePlaced = false;

        // Setup Intro
        IsUIBlockingInput = true; // Block input while intro runs
        selectedWoundGrade = 0;
        examTriggerGO = null;

        HideAllWounds();

        // Ensure patient visuals and title are fully hidden until intro finishes
        if (restPositionGO) restPositionGO.SetActive(false);
        if (lateralPositionGO) lateralPositionGO.SetActive(false);
        if (inspectPositionGO) inspectPositionGO.SetActive(false);
        if (titlePanel) titlePanel.SetActive(false);

        if (introNovel != null)
        {
            introNovel.OnIntroFinished.AddListener(OnIntroFinished);
            introNovel.StartIntro(initialIntroDialogue);
        }
        else
        {
            // Fallback if no intro is assigned
            OnIntroFinished();
        }
    }

    private void OnIntroFinished()
    {
        if (introNovel != null)
            introNovel.OnIntroFinished.RemoveListener(OnIntroFinished);
            
        IsUIBlockingInput = false;

        // Apply idle state *after* the intro is completely finished
        state = StageState.Idle;
        ApplyPositionVisual(StageState.Idle);
        
        // Show Title Panel now
        if (titlePanel) titlePanel.SetActive(true);
        
        Debug.Log("Intro finished, game unblocked, showing idle stage.");
    }

    // =========================
    // Patient click transitions
    // =========================
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

        // Hide Title Panel when entering Inspect Mode
        if (titlePanel) titlePanel.SetActive(false);

        // Choose wound (and set examTriggerGO)
        if (rerollEveryEnterInspect || examTriggerGO == null || selectedWoundGrade == 0)
            RollRandomWound();

        state = StageState.InspectReady;

        // Show visual novel again with different convo
        if (introNovel != null && inspectIntroDialogue != null && inspectIntroDialogue.Length > 0)
        {
            IsUIBlockingInput = true;
            introNovel.OnIntroFinished.AddListener(OnInspectIntroFinished);
            introNovel.StartIntro(inspectIntroDialogue);
        }
    }

    private void OnInspectIntroFinished()
    {
        if (introNovel != null)
            introNovel.OnIntroFinished.RemoveListener(OnInspectIntroFinished);

        // Instead of unblocking сразу, show the instructional "quiz" panel
        if (instructionQuizPanel)
        {
            instructionQuizPanel.SetActive(true);
        }
        
        IsUIBlockingInput = false; // Allow clicking even if instruction is up

        Debug.Log("Inspect intro finished. Showing instruction.");
    }

    /// <summary>
    /// Called by the instructional quiz button to resume inspect.
    /// </summary>
    public void DismissInstructionQuiz()
    {
        if (instructionQuizPanel) instructionQuizPanel.SetActive(false);
        IsUIBlockingInput = false;
        Debug.Log("Instruction quiz dismissed, game unblocked for inspection.");
    }

    private void ApplyPositionVisual(StageState target)
    {
        bool rest    = target == StageState.Idle;
        bool lateral = target == StageState.PatientTurned;
        bool inspect = target == StageState.InspectReady;

        if (restPositionGO)    restPositionGO.SetActive(rest);
        if (lateralPositionGO) lateralPositionGO.SetActive(lateral);
        if (inspectPositionGO) inspectPositionGO.SetActive(inspect);

        if (inspectArrow) inspectArrow.SetActive(inspect);

        if (!inspect)
        {
            // Ensure no wound is clickable/visible outside Inspect
            examTriggerGO = null;
            selectedWoundGrade = 0;
            HideAllWounds();
        }
    }

    // =========================
    // Called by ExamTrigger.cs
    // =========================
    public void OnExamTriggerClicked(GameObject caller)
    {
        if (IsUIBlockingInput) { Debug.Log("[StageManager] Blocked by UI"); return; }
        if (state != StageState.InspectReady) { Debug.Log($"[StageManager] Wrong state: {state}"); return; }

        // Dismiss instruction if it's still up
        if (instructionQuizPanel) instructionQuizPanel.SetActive(false);

        // Accept click from examTriggerGO itself OR any of its children
        if (examTriggerGO == null) { Debug.Log("[StageManager] examTriggerGO is null"); return; }
        if (caller != examTriggerGO && !caller.transform.IsChildOf(examTriggerGO.transform))
        {
            Debug.Log($"[StageManager] Caller mismatch: {caller.name} vs {examTriggerGO.name}");
            return;
        }

        StartGradeQuestion();
    }

    // =========================
    // Wound randomization
    // =========================
    private void RollRandomWound()
    {
        if (woundVariants == null || woundVariants.Length == 0)
        {
            Debug.LogError("ImmobilityStageManager: woundVariants not assigned.");
            examTriggerGO = null;
            selectedWoundGrade = 0;
            return;
        }

        HideAllWounds();

        int index = Random.Range(0, woundVariants.Length); // 0..len-1
        examTriggerGO = woundVariants[index];
        selectedWoundGrade = index + 1;

        if (examTriggerGO) examTriggerGO.SetActive(true);

        Debug.Log($"Chosen wound (examTriggerGO): {examTriggerGO?.name} | Grade {selectedWoundGrade}");
    }

    private void HideAllWounds()
    {
        if (woundVariants == null) return;

        for (int i = 0; i < woundVariants.Length; i++)
            if (woundVariants[i]) woundVariants[i].SetActive(false);
    }

    // =========================
    // Existing grading flow
    // =========================
    private void HideAllVisuals()
    {
        if (restPositionGO) restPositionGO.SetActive(false);
        if (lateralPositionGO) lateralPositionGO.SetActive(false);
        if (inspectPositionGO) inspectPositionGO.SetActive(false);
        if (patientWithPillowChestGO) patientWithPillowChestGO.SetActive(false);
        if (patientWithPillowAnkleGO) patientWithPillowAnkleGO.SetActive(false);
        if (patientWithBothPillowsGO) patientWithBothPillowsGO.SetActive(false);
        if (bedGO) bedGO.SetActive(false);
        if (examTriggerGO) examTriggerGO.SetActive(false);
        if (titlePanel) titlePanel.SetActive(false);
        if (pillowChest) pillowChest.SetActive(false);
        if (pillowDropZoneChest) pillowDropZoneChest.SetActive(false);
        if (pillowAnkle) pillowAnkle.SetActive(false);
        if (pillowDropZoneAnkle) pillowDropZoneAnkle.SetActive(false);
        if (knowledgePopupPanel) knowledgePopupPanel.SetActive(false);
        if (panelGradeQuestion) panelGradeQuestion.SetActive(false);
        if (summaryQuiz) summaryQuiz.gameObject.SetActive(false);
        if (instructionQuizPanel) instructionQuizPanel.SetActive(false);
        if (inspectArrow) inspectArrow.SetActive(false);
        HideAllWounds();

        // Hide extra user-defined images
        if (extraImagesToHide != null)
        {
            foreach (var img in extraImagesToHide)
            {
                if (img != null) img.gameObject.SetActive(false);
            }
        }
    }

    public void StartGradeQuestion()
    {
        state = StageState.GradeQuestion;
        IsUIBlockingInput = true;

        HideAllVisuals();

        if (panelGradeQuestion) panelGradeQuestion.SetActive(true);
    }

    public void OnGradeAnsweredCorrect()
    {
        if (state != StageState.GradeQuestion) return;

        // Note: Keep panelGradeQuestion ACTIVE so the user sees their correct answer colored green!
        // We will transition states to prevent further interaction.
        state = StageState.CongratsAfterGrade; // We can reuse this state as "Waiting after grade"

        Invoke(nameof(AdvanceToPillowPlacement), gradeCorrectDelaySeconds);
    }

    private void AdvanceToPillowPlacement()
    {
        // Now we turn off the grade question panel
        if (panelGradeQuestion) panelGradeQuestion.SetActive(false);

        state = StageState.PillowPlacement;

        // Visual Novel Intro for Pillow Placement
        if (introNovel != null && pillowIntroDialogue != null && pillowIntroDialogue.Length > 0)
        {
            IsUIBlockingInput = true;
            introNovel.OnIntroFinished.AddListener(OnPillowIntroFinished);
            introNovel.StartIntro(pillowIntroDialogue);
        }
        else
        {
            IsUIBlockingInput = false;
        }

        // === FORCE PATIENT TO REST POSITION DURING DRAG PHASE ===
        if (restPositionGO)    restPositionGO.SetActive(true);
        if (lateralPositionGO) lateralPositionGO.SetActive(false);
        if (inspectPositionGO) inspectPositionGO.SetActive(false);
        if (bedGO) bedGO.SetActive(true); // Re-enable Bed here

        // Clear wound / exam state
        examTriggerGO = null;
        selectedWoundGrade = 0;
        HideAllWounds();

        // === ENABLE BOTH PILLOW DRAGS ===
        pillowChestPlaced = false;
        pillowAnklePlaced = false;

        if (pillowChest)
        {
            pillowChest.SetActive(true);
            var dragScript = pillowChest.GetComponent<Draggable2D>();
            if (dragScript) dragScript.isLocked = false;
        }
        if (pillowDropZoneChest) pillowDropZoneChest.SetActive(true);

        if (pillowAnkle)
        {
            pillowAnkle.SetActive(true);
            var dragScript = pillowAnkle.GetComponent<Draggable2D>();
            if (dragScript) dragScript.isLocked = false;
        }
        if (pillowDropZoneAnkle) pillowDropZoneAnkle.SetActive(true);
    }

    private void OnPillowIntroFinished()
    {
        if (introNovel != null)
            introNovel.OnIntroFinished.RemoveListener(OnPillowIntroFinished);

        IsUIBlockingInput = false;
        Debug.Log("Pillow placement intro finished, game unblocked for dragging.");
    }


    [Header("Post-Pillow Visual")]
    [SerializeField] private GameObject patientWithPillowChestGO;
    [SerializeField] private GameObject patientWithPillowAnkleGO;
    [SerializeField] private GameObject patientWithBothPillowsGO;
    [SerializeField] private float postPillowDelaySeconds = 3.0f;

    public void OnPillowPlacedCorrect(GameObject triggeredZone)
    {
        if (state != StageState.PillowPlacement) return;

        if (triggeredZone == pillowDropZoneChest)
        {
            pillowChestPlaced = true;
            Debug.Log("Chest pillow placed correctly.");
            if (pillowChest) pillowChest.SetActive(false);
            if (pillowDropZoneChest) pillowDropZoneChest.SetActive(false);
        }
        else if (triggeredZone == pillowDropZoneAnkle)
        {
            pillowAnklePlaced = true;
            Debug.Log("Ankle pillow placed correctly.");
            if (pillowAnkle) pillowAnkle.SetActive(false);
            if (pillowDropZoneAnkle) pillowDropZoneAnkle.SetActive(false);
        }

        UpdatePillowVisuals();

        if (pillowChestPlaced && pillowAnklePlaced)
        {
            Debug.Log("Both pillows placed correctly. Waiting to show knowledge popup.");
            IsUIBlockingInput = true; // Block clicking while waiting
            StartCoroutine(ShowKnowledgePopupDelayed());
        }
    }

    private System.Collections.IEnumerator ShowKnowledgePopupDelayed()
    {
        yield return new WaitForSeconds(knowledgePopupDelaySeconds);
        IsUIBlockingInput = false;

        if (knowledgePopupPanel) knowledgePopupPanel.SetActive(true);
        else StartSummaryQuiz();
    }

    private void UpdatePillowVisuals()
    {
        if (restPositionGO) restPositionGO.SetActive(false);
        if (patientWithPillowChestGO) patientWithPillowChestGO.SetActive(false);
        if (patientWithPillowAnkleGO) patientWithPillowAnkleGO.SetActive(false);
        if (patientWithBothPillowsGO) patientWithBothPillowsGO.SetActive(false);

        if (pillowChestPlaced && pillowAnklePlaced)
        {
            if (patientWithBothPillowsGO) patientWithBothPillowsGO.SetActive(true);
        }
        else if (pillowChestPlaced)
        {
            if (patientWithPillowChestGO) patientWithPillowChestGO.SetActive(true);
            else if (restPositionGO) restPositionGO.SetActive(true);
        }
        else if (pillowAnklePlaced)
        {
            if (patientWithPillowAnkleGO) patientWithPillowAnkleGO.SetActive(true);
            else if (restPositionGO) restPositionGO.SetActive(true);
        }
        else
        {
            if (restPositionGO) restPositionGO.SetActive(true);
        }
    }

    public void OnKnowledgePopupClosed()
    {
        if (knowledgePopupPanel) knowledgePopupPanel.SetActive(false);
        
        Debug.Log($"Knowledge popup closed. Waiting {postPillowDelaySeconds}s before summary.");
        Invoke(nameof(StartSummaryQuiz), postPillowDelaySeconds);
    }

    // Deprecated / Unused now
    // public void OnTurnOverAnsweredCorrect()
    // {
    // }

    private void StartSummaryQuiz()
    {
        state = StageState.SummaryQuiz;
        IsUIBlockingInput = true; // Quiz is modal

        HideAllVisuals();

        if (summaryQuiz)
        {
            summaryQuiz.gameObject.SetActive(true);
            summaryQuiz.Show();
        }
        else
        {
            Debug.LogWarning("SummaryQuiz reference missing on Manager. Completing immediately.");
            CompleteStage();
        }
    }

    // Called optionally by SummaryQuiz when "Finish" is clicked?
    // Or just stay in quiz. But lets have a method just in case.
    public void CompleteStage()
    {
        state = StageState.Complete;
        IsUIBlockingInput = false;
        Debug.Log("Immobility stage complete. Showing Result...");
        if (resultUI)
        {
            resultUI.ShowResult(
                true,
                "<color=green>ภารกิจสำเร็จ!</color>",
                "คุณได้เรียนรู้การดูแลผู้ป่วยติดเตียงและการป้องกันแผลกดทับอย่างถูกต้อง",
                "ไปด่านต่อไป >>" 
            );
        }
        else
        {
            SceneManager.LoadScene("Insomnia (NF)");
        }
    }
}