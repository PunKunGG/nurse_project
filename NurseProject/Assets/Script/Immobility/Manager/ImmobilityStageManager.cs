// ImmobilityStageManager.cs
using UnityEngine;

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
    [SerializeField] private GameObject panelCongrats;
    [SerializeField] private GameObject panelTurnOver;
    [SerializeField] private GameObject titlePanel; // New Title Panel
    [SerializeField] private ImmobilitySummaryQuiz summaryQuiz; // Reference to the new quiz

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

    [Header("Pillow Objects")]
    [SerializeField] private GameObject pillow;
    [SerializeField] private GameObject pillowDropZone;

    [Header("Timing")]
    [SerializeField] private float congratsAutoCloseSeconds = 1.0f;

    public bool IsUIBlockingInput { get; private set; }

    private int selectedWoundGrade = 0; // 1..4
    public int CurrentWoundGrade => selectedWoundGrade;

    private void Awake()
    {
        // Turn off UI
        if (panelGradeQuestion) panelGradeQuestion.SetActive(false);
        if (panelCongrats) panelCongrats.SetActive(false);
        if (panelTurnOver) panelTurnOver.SetActive(false);

        // Turn off pillow flow
        if (pillow) pillow.SetActive(false);
        if (pillowDropZone) pillowDropZone.SetActive(false);

        // Start at Rest
        IsUIBlockingInput = false;
        selectedWoundGrade = 0;
        examTriggerGO = null;

        ApplyPositionVisual(StageState.Idle);
        HideAllWounds();

        state = StageState.Idle;
        
        // Ensure Title Panel is active at start if assigned
        if (titlePanel) titlePanel.SetActive(true);
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
        if (IsUIBlockingInput) return;
        if (state != StageState.InspectReady) return;

        // Only accept clicks from the chosen wound
        if (caller != examTriggerGO) return;

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
        {
            pillow.SetActive(true);

            var dragScript = pillow.GetComponent<Draggable2D>();
            if (dragScript) dragScript.isLocked = false;
        }

        if (pillowDropZone) pillowDropZone.SetActive(true);
    }


    public void OnPillowPlacedCorrect()
    {
        if (state != StageState.PillowPlacement) return;

        // SKIP TurnOverQuestion -> Go straight to SummaryQuiz
        StartSummaryQuiz();
    }

    // Deprecated / Unused now
    public void OnTurnOverAnsweredCorrect()
    {
        // if (state != StageState.TurnOverQuestion) return;
        // if (panelTurnOver) panelTurnOver.SetActive(false);
        // StartSummaryQuiz();
    }

    private void StartSummaryQuiz()
    {
        state = StageState.SummaryQuiz;
        IsUIBlockingInput = true; // Quiz is modal

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
        Debug.Log("Immobility stage complete.");
    }
}
