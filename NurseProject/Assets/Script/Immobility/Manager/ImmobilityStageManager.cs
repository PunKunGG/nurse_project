using UnityEngine;

public class ImmobilityStageManager : MonoBehaviour
{
    public enum StageState
    {
        Idle,
        GradeQuestion,
        CongratsAfterGrade,
        PillowPlacement,
        TurnOverQuestion,
        Complete
    }

    [Header("State")]
    [SerializeField] private StageState state = StageState.Idle;

    [Header("UI Panels")]
    [SerializeField] private GameObject panelGradeQuestion;
    [SerializeField] private GameObject panelCongrats;
    [SerializeField] private GameObject panelTurnOver;

    [Header("Patient Phase Objects")]
    [SerializeField] private GameObject patientDummy;
    [SerializeField] private GameObject pillow;
    [SerializeField] private GameObject pillowDropZone;

    [Header("Timing")]
    [SerializeField] private float congratsAutoCloseSeconds = 1.0f;

    // Optional: lock other gameplay input when UI is open
    public bool IsUIBlockingInput { get; private set; }

    private void Awake()
    {
        SetAllOff();
        state = StageState.Idle;
    }

    private void SetAllOff()
    {
        if (panelGradeQuestion) panelGradeQuestion.SetActive(false);
        if (panelCongrats) panelCongrats.SetActive(false);
        if (panelTurnOver) panelTurnOver.SetActive(false);

        if (patientDummy) patientDummy.SetActive(false);
        if (pillow) pillow.SetActive(false);
        if (pillowDropZone) pillowDropZone.SetActive(false);

        IsUIBlockingInput = false;
    }

    public void StartGradeQuestion()
    {
        if (state != StageState.Idle) return;

        state = StageState.GradeQuestion;
        IsUIBlockingInput = true;

        if (panelGradeQuestion) panelGradeQuestion.SetActive(true);
    }

    public void OnGradeAnsweredCorrect()
    {
        if (state != StageState.GradeQuestion) return;

        // Close question UI
        if (panelGradeQuestion) panelGradeQuestion.SetActive(false);

        // Show congrats
        state = StageState.CongratsAfterGrade;
        if (panelCongrats) panelCongrats.SetActive(true);

        Invoke(nameof(AdvanceToPillowPlacement), congratsAutoCloseSeconds);
    }

    private void AdvanceToPillowPlacement()
    {
        if (panelCongrats) panelCongrats.SetActive(false);

        state = StageState.PillowPlacement;
        IsUIBlockingInput = false;

        if (patientDummy) patientDummy.SetActive(true);
        if (pillow) pillow.SetActive(true);
        if (pillowDropZone) pillowDropZone.SetActive(true);
    }

    public void OnPillowPlacedCorrect()
    {
        if (state != StageState.PillowPlacement) return;

        state = StageState.TurnOverQuestion;
        IsUIBlockingInput = true;

        if (panelTurnOver) panelTurnOver.SetActive(true);
    }

    public void OnTurnOverAnsweredCorrect()
    {
        if (state != StageState.TurnOverQuestion) return;

        if (panelTurnOver) panelTurnOver.SetActive(false);
        IsUIBlockingInput = false;

        state = StageState.Complete;

        // Optional: you can show a final congrats UI here
        Debug.Log("Immobility stage complete.");
    }
}
