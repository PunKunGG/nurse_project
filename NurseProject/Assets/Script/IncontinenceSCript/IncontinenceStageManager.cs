using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class IncontinenceStageManager : MonoBehaviour
{
    // --- ส่วนเก็บข้อมูลเคส (Data Structure) ---
    [System.Serializable]
    public class CaseProfile
    {
        public string caseName; // ตั้งชื่อเคส (เช่น "คุณยายสมศรี") ไว้ดูเอง
        [TextArea(5, 10)] 
        public string patientInfo; // ข้อความยาวๆ ที่จะโชว์ในหน้าแรก

        [Header("Correct Answers")]
        [Tooltip("ลำดับปุ่ม Diagnosis ที่ถูก (0-6)")]
        public int correctDiagnosisIndex; 
        
        [Tooltip("ลำดับปุ่ม Intervention ที่ถูก (0-9) เลือกได้หลายข้อ")]
        public List<int> correctInterventionIndices; 
    }

    public enum StageState
    {
        ReadingCase, Diagnosis, Intervention, Complete
    }

    [Header("State")]
    public StageState state = StageState.ReadingCase;

    [Header("--- Case Database (ใส่ 5 เคสตรงนี้) ---")]
    public CaseProfile[] allCases; // Array สำหรับใส่ 5 เคส
    private CaseProfile activeCase; // เคสที่สุ่มได้รอบนี้

    [Header("UI Panels")]
    public GameObject panelCase;
    public GameObject panelDiagnosis;
    public GameObject panelIntervention;
    public UniversalResultUI resultUI;

    [Header("UI Elements Reference")]
    public TextMeshProUGUI caseInfoText; // text ในหน้าแรกที่จะเปลี่ยนไปเรื่อยๆ
    public Button[] diagnosisButtons;    // ปุ่มโรค 7 ปุ่ม
    public Button[] interventionButtons; // ปุ่มการพยาบาล 10 ปุ่ม (Master List)
    
    [Header("Feedbacks")]
    public TextMeshProUGUI diagnosisFeedback;
    public TextMeshProUGUI interventionFeedback;

    [Header("Colors")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public Color defaultColor = Color.white;

    // ตัวแปรภายใน
    private int foundInterventions = 0;
    private List<int> currentCorrectInterventions; // เก็บเฉลยของรอบนี้

    void Start()
    {
        // 1. สุ่มเคส
        PickRandomCase();

        // 2. Setup ปุ่มต่างๆ
        SetupButtons();

        // 3. เริ่มแสดงผล
        ShowPanel(StageState.ReadingCase);
    }

    void PickRandomCase()
    {
        if (allCases.Length == 0) return;

        int randomIndex = Random.Range(0, allCases.Length);
        activeCase = allCases[randomIndex];

        // เอาข้อความจากเคสที่สุ่มได้ ไปใส่ใน Text UI
        if (caseInfoText) caseInfoText.text = activeCase.patientInfo;

        // ดึงเฉลย Intervention มาเก็บไว้
        currentCorrectInterventions = activeCase.correctInterventionIndices;
        
        Debug.Log($"Selected Case: {activeCase.caseName}");
    }

    void SetupButtons()
    {
        // Setup Diagnosis Buttons
        for (int i = 0; i < diagnosisButtons.Length; i++)
        {
            int index = i;
            diagnosisButtons[i].onClick.AddListener(() => OnDiagnosisSelected(index));
        }

        // Setup Intervention Buttons (ทั้ง 10 ปุ่ม)
        for (int i = 0; i < interventionButtons.Length; i++)
        {
            int index = i;
            interventionButtons[i].onClick.AddListener(() => OnInterventionClicked(interventionButtons[index], index));
        }
    }

    // --- STEP 1: Reading Case ---
    public void OnCaseReadConfirm()
    {
        ShowPanel(StageState.Diagnosis);
    }

    // --- STEP 2: Diagnosis ---
    void OnDiagnosisSelected(int index)
    {
        if (state != StageState.Diagnosis) return;

        // Reset สี
        foreach (var btn in diagnosisButtons) btn.image.color = defaultColor;

        // เช็คคำตอบกับ activeCase
        if (index == activeCase.correctDiagnosisIndex)
        {
            diagnosisButtons[index].image.color = correctColor;
            if(diagnosisFeedback) diagnosisFeedback.text = "Correct Diagnosis!";
            Invoke(nameof(GoToIntervention), 1.0f);
        }
        else
        {
            diagnosisButtons[index].image.color = wrongColor;
            if(diagnosisFeedback) diagnosisFeedback.text = "Incorrect diagnosis for this patient.";
        }
    }

    void GoToIntervention()
    {
        ShowPanel(StageState.Intervention);
    }

    // --- STEP 3: Intervention ---
    void OnInterventionClicked(Button btn, int btnIndex)
    {
        if (state != StageState.Intervention) return;

        // เช็คว่า Index ของปุ่มที่กด อยู่ในลิสต์เฉลยของเคสนี้ไหม?
        if (currentCorrectInterventions.Contains(btnIndex))
        {
            // ถูก
            btn.image.color = correctColor;
            btn.interactable = false;
            foundInterventions++;

            CheckInterventionComplete();
        }
        else
        {
            // ผิด
            btn.image.color = wrongColor;
            if(interventionFeedback) interventionFeedback.text = "Not suitable for this case.";
        }
    }

    void CheckInterventionComplete()
    {
        // ถ้าหาเจอครบตามจำนวนเฉลยของเคสนั้นๆ
        if (foundInterventions >= currentCorrectInterventions.Count)
        {
            Invoke(nameof(FinishStage), 1.0f);
        }
    }

    void FinishStage()
    {
        ShowPanel(StageState.Complete);
    }

    void ShowPanel(StageState newState)
    {
        state = newState;
        if(panelCase) panelCase.SetActive(false);
        if(panelDiagnosis) panelDiagnosis.SetActive(false);
        if(panelIntervention) panelIntervention.SetActive(false);
        if(resultUI) resultUI.gameObject.SetActive(false);

        switch (newState)
        {
            case StageState.ReadingCase: if(panelCase) panelCase.SetActive(true); break;
            case StageState.Diagnosis: if(panelDiagnosis) panelDiagnosis.SetActive(true); break;
            case StageState.Intervention: if(panelIntervention) panelIntervention.SetActive(true); break;
            case StageState.Complete: if(resultUI) resultUI.ShowResult(true); break;
        }
    }
}