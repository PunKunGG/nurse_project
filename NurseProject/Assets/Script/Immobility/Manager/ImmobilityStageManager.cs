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
    public UniversalResultUI resultUI; // Add UniversalResultUI

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

    [Header("Pillow Objects")]
    [SerializeField] private GameObject pillow;
    [SerializeField] private GameObject pillowDropZone;

    [Header("Extra UI To Hide")]
    [SerializeField] private Image[] extraImagesToHide;

    [Header("Timing")]
    [SerializeField] private float gradeCorrectDelaySeconds = 2.0f; // Time to show correct answer before moving on

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

        if (patientWithPillowGO) patientWithPillowGO.SetActive(false);

        // Turn off pillow flow
        if (pillow) pillow.SetActive(false);
        if (pillowDropZone) pillowDropZone.SetActive(false);

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

        IsUIBlockingInput = false;
        Debug.Log("Inspect intro finished, game unblocked for inspection.");
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
    }

    // =========================
    // Called by ExamTrigger.cs
    // =========================
    public void OnExamTriggerClicked(GameObject caller)
    {
        if (IsUIBlockingInput) { Debug.Log("[StageManager] Blocked by UI"); return; }
        if (state != StageState.InspectReady) { Debug.Log($"[StageManager] Wrong state: {state}"); return; }

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
        if (patientWithPillowGO) patientWithPillowGO.SetActive(false);
        if (bedGO) bedGO.SetActive(false);
        if (examTriggerGO) examTriggerGO.SetActive(false);
        if (titlePanel) titlePanel.SetActive(false);
        if (pillow) pillow.SetActive(false);
        if (pillowDropZone) pillowDropZone.SetActive(false);
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

        // === ENABLE PILLOW DRAG ===
        if (pillow)
        {
            pillow.SetActive(true);

            var dragScript = pillow.GetComponent<Draggable2D>();
            if (dragScript) dragScript.isLocked = false;
        }

        if (pillowDropZone) pillowDropZone.SetActive(true);
    }

    private void OnPillowIntroFinished()
    {
        if (introNovel != null)
            introNovel.OnIntroFinished.RemoveListener(OnPillowIntroFinished);

        IsUIBlockingInput = false;
        Debug.Log("Pillow placement intro finished, game unblocked for dragging.");
    }


    [Header("Post-Pillow Visual")]
    [SerializeField] private GameObject patientWithPillowGO;
    [SerializeField] private float postPillowDelaySeconds = 3.0f;

    public void OnPillowPlacedCorrect()
    {
        if (state != StageState.PillowPlacement) return;

        // Visual feedback: Show patient with pillow
        if (restPositionGO) restPositionGO.SetActive(false);
        if (patientWithPillowGO) patientWithPillowGO.SetActive(true);

        // Hide the draggable pillow so it doesn't double up visuals
        if (pillow) pillow.SetActive(false);
        if (pillowDropZone) pillowDropZone.SetActive(false); // Hide trigger too

        Debug.Log($"Pillow placed correctly. Waiting {postPillowDelaySeconds}s before summary.");

        // Delay 
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