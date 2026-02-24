// IncontinenceLevelController.cs — ตัวควบคุมหลักของภารกิจ Incontinence
// แทนที่ IncontinenceStageManager.cs เดิม
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// State Machine: CaseIntro → ChooseType → ChooseManagement → Completed
/// เล่นเคสเรียงลำดับจากเคส 1 ถึง N (ตาม CaseDatabase)
/// มอบหมาย Stage 2 ให้ IncontinenceTypePanel, Stage 3 ให้ IncontinenceManagementPanel
/// แสดง UniversalResultUI ตอนจบแต่ละเคส แล้วไปเคสถัดไปจนครบ
/// </summary>
public class IncontinenceLevelController : MonoBehaviour
{
    public enum Stage { CaseIntro, ChooseType, ChooseManagement, Completed }

    [Header("Case Database")]
    [SerializeField] private IncontinenceCaseDatabase caseDatabase;

    [Header("Patient Spawn Point")]
    [Tooltip("ตำแหน่งที่จะ spawn ตัวผู้ป่วย — ทุกเคสจะแสดง ณ จุดเดียวกัน")]
    [SerializeField] private Transform patientSpawnPoint;

    [Header("UI Panels")]
    [SerializeField] private GameObject casePanelObj;        // Stage 1 panel
    [SerializeField] private GameObject typePanelObj;        // Stage 2 panel
    [SerializeField] private GameObject managementPanelObj;  // Stage 3 panel

    [Header("Stage 1: Case Intro")]
    [SerializeField] private TextMeshProUGUI caseNarrativeText;
    [SerializeField] private TextMeshProUGUI caseTitleText;
    [SerializeField] private Button continueButton;

    [Header("Stage 2: Type Selection")]
    [SerializeField] private IncontinenceTypePanel typePanel;

    [Header("Stage 3: Management Selection")]
    [SerializeField] private IncontinenceManagementPanel managementPanel;

    [Header("Result")]
    [SerializeField] private UniversalResultUI resultUI;

    [Header("Patient History Widget (ประวัติผู้ป่วยแบบย่อ/ขยาย)")]
    [SerializeField] private PatientHistoryWidget historyWidget;

    [Header("Knowledge Widget (ความรู้ประเภท Incontinence)")]
    [SerializeField] private IncontinenceKnowledgeWidget knowledgeWidget;

    [Header("Events")]
    public UnityEvent<Stage> OnStageChanged;
    public UnityEvent OnCaseCompleted;

    // State
    public Stage CurrentStage { get; private set; }
    public int CurrentCaseIndex { get; private set; }
    private IncontinenceCaseData activeCase;
    private GameObject spawnedPatient;

    // ============================================================
    //  LIFECYCLE
    // ============================================================

    private void Start()
    {
        // ผูกปุ่ม Continue
        if (continueButton)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueClicked);
        }

        // เริ่มเคสแรก (index 0)
        CurrentCaseIndex = 0;
        LoadCase(CurrentCaseIndex);
        GoToStage(Stage.CaseIntro);
    }

    // ============================================================
    //  CASE LOADING
    // ============================================================

    private void LoadCase(int index)
    {
        if (caseDatabase == null)
        {
            Debug.LogError("[IncontinenceLevelController] CaseDatabase not assigned!");
            return;
        }

        activeCase = caseDatabase.GetCase(index);

        if (activeCase == null)
        {
            Debug.LogError($"[IncontinenceLevelController] No case at index {index}!");
            return;
        }

        Debug.Log($"[IncontinenceLevelController] Loaded case {index + 1}/{caseDatabase.Count}: {activeCase.caseId} — {activeCase.title}");

        // Spawn ตัวผู้ป่วยตามเคส
        SpawnPatient();
    }

    // ============================================================
    //  STAGE NAVIGATION
    // ============================================================

    private void GoToStage(Stage newStage)
    {
        CurrentStage = newStage;

        // ซ่อนทุก panel ก่อน
        if (casePanelObj)       casePanelObj.SetActive(false);
        if (typePanelObj)       typePanelObj.SetActive(false);
        if (managementPanelObj) managementPanelObj.SetActive(false);

        // แสดง history widget เฉพาะ Stage 2 และ 3 (ซ่อนตอน Intro และ Completed)
        bool showHistory = (newStage == Stage.ChooseType || newStage == Stage.ChooseManagement);
        if (historyWidget)
        {
            if (showHistory) historyWidget.Show();
            else             historyWidget.Hide();
        }

        // แสดง knowledge widget เฉพาะ Stage 2 (ChooseType) — ให้ผู้เล่นอ่านความรู้ก่อนเลือก
        if (knowledgeWidget)
        {
            if (newStage == Stage.ChooseType) knowledgeWidget.Show();
            else                              knowledgeWidget.Hide();
        }

        switch (newStage)
        {
            case Stage.CaseIntro:
                SetupCaseIntro();
                if (casePanelObj)  casePanelObj.SetActive(true);
                break;

            case Stage.ChooseType:
                SetupTypeStage();
                if (typePanelObj) typePanelObj.SetActive(true);
                break;

            case Stage.ChooseManagement:
                SetupManagementStage();
                if (managementPanelObj) managementPanelObj.SetActive(true);
                break;

            case Stage.Completed:
                ShowResult();
                break;
        }

        OnStageChanged?.Invoke(newStage);
        Debug.Log($"[IncontinenceLevelController] Stage → {newStage}");
    }

    // ============================================================
    //  STAGE 1: CASE INTRO
    // ============================================================

    private void SetupCaseIntro()
    {
        if (activeCase == null) return;

        if (caseTitleText)     caseTitleText.text = activeCase.title;
        if (caseNarrativeText) caseNarrativeText.text = activeCase.narrativeText;

        // เตรียมข้อมูลให้ widget (จะแสดงตอนเข้า Stage 2+)
        if (historyWidget) historyWidget.SetHistory(activeCase.title, activeCase.narrativeText);
    }

    private void OnContinueClicked()
    {
        if (CurrentStage != Stage.CaseIntro) return;
        GoToStage(Stage.ChooseType);
    }

    // ============================================================
    //  STAGE 2: CHOOSE TYPE  (IncontinenceTypePanel เป็นคนจัดการ)
    // ============================================================

    private void SetupTypeStage()
    {
        if (activeCase == null) return;
        if (typePanel)  typePanel.Setup(this, activeCase.correctType);
    }

    /// <summary>เรียกโดย IncontinenceTypePanel เมื่อผู้เล่นตอบถูก</summary>
    public void OnTypeAnsweredCorrectly()
    {
        if (CurrentStage != Stage.ChooseType) return;
        GoToStage(Stage.ChooseManagement);
    }

    // ============================================================
    //  STAGE 3: CHOOSE MANAGEMENT  (IncontinenceManagementPanel เป็นคนจัดการ)
    // ============================================================

    private void SetupManagementStage()
    {
        if (activeCase == null) return;
        if (managementPanel) managementPanel.Setup(this, activeCase.correctManagementOptions, activeCase.hintDialogue);
    }

    /// <summary>เรียกโดย IncontinenceManagementPanel เมื่อผู้เล่นตอบถูกครบ</summary>
    public void OnManagementAnsweredCorrectly()
    {
        if (CurrentStage != Stage.ChooseManagement) return;
        GoToStage(Stage.Completed);
    }

    // ============================================================
    //  COMPLETED — RESULT + NEXT CASE
    // ============================================================

    private void ShowResult()
    {
        OnCaseCompleted?.Invoke();

        bool isLastCase = (CurrentCaseIndex >= caseDatabase.Count - 1);

        if (resultUI)
        {
            if (isLastCase)
            {
                // --- เคสสุดท้าย: แสดงผลรวมสำเร็จ ---
                resultUI.ShowResult(
                    true,
                    "<color=green>ผ่านทุกเคสสำเร็จ!</color>",
                    $"คุณผ่านครบทั้ง {caseDatabase.Count} เคสเรียบร้อยแล้ว",
                    "จบด่าน >>"
                );
            }
            else
            {
                // --- ยังมีเคสถัดไป ---
                int nextDisplay = CurrentCaseIndex + 2; // แสดงเลขเคสถัดไป (1-based)
                resultUI.ShowResult(
                    true,
                    "<color=green>ประเมินและวางแผนสำเร็จ!</color>",
                    $"เคสที่ {CurrentCaseIndex + 1}/{caseDatabase.Count} สำเร็จ!",
                    $"ไปเคสที่ {nextDisplay} >>",
                    () => AdvanceToNextCase()
                );
            }
        }
        else
        {
            Debug.LogWarning("[IncontinenceLevelController] UniversalResultUI not assigned!");
        }
    }

    /// <summary>โหลดเคสถัดไปแล้วกลับไปหน้า CaseIntro</summary>
    private void AdvanceToNextCase()
    {
        CurrentCaseIndex++;

        if (CurrentCaseIndex >= caseDatabase.Count)
        {
            Debug.Log("[IncontinenceLevelController] All cases completed!");
            return;
        }

        // ซ่อน result panel
        if (resultUI && resultUI.panelRoot) resultUI.panelRoot.SetActive(false);

        // โหลดเคสใหม่แล้วเริ่ม Stage 1
        LoadCase(CurrentCaseIndex);
        GoToStage(Stage.CaseIntro);

        Debug.Log($"[IncontinenceLevelController] Advanced to case {CurrentCaseIndex + 1}/{caseDatabase.Count}");
    }

    // ============================================================
    //  PATIENT SPAWN
    // ============================================================

    /// <summary>Instantiate ตัวผู้ป่วยจาก prefab ของเคสปัจจุบัน ณ ตำแหน่ง patientSpawnPoint</summary>
    private void SpawnPatient()
    {
        // ลบตัวเก่า
        if (spawnedPatient != null) Destroy(spawnedPatient);

        if (activeCase == null || activeCase.patientPrefab == null || patientSpawnPoint == null)
            return;

        spawnedPatient = Instantiate(
            activeCase.patientPrefab,
            patientSpawnPoint.position,
            patientSpawnPoint.rotation,
            patientSpawnPoint              // parent ไว้ใต้ spawn point
        );

        Debug.Log($"[IncontinenceLevelController] Spawned patient: {spawnedPatient.name}");
    }

    // ============================================================
    //  DEV TOOLS
    // ============================================================

    [ContextMenu("Force Complete")]
    public void ForceComplete()
    {
        GoToStage(Stage.Completed);
    }

    [ContextMenu("Restart Current Case")]
    public void RestartCurrentCase()
    {
        LoadCase(CurrentCaseIndex);
        GoToStage(Stage.CaseIntro);
    }

    [ContextMenu("Restart All (From Case 1)")]
    public void RestartAll()
    {
        CurrentCaseIndex = 0;
        LoadCase(CurrentCaseIndex);
        GoToStage(Stage.CaseIntro);
    }
}
