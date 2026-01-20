using UnityEngine;

public class ImmobilityStageManager : MonoBehaviour
{
    public enum StageState
    {
        Idle,
        PatientTurned,      // *เพิ่มสถานะ: พลิกตัวแล้ว
        GradeQuestion,
        CongratsAfterGrade,
        PillowPlacement,
        TurnOverQuestion,
        Complete
    }

    [Header("State")]
    public StageState state = StageState.Idle; // เปลี่ยนเป็น public เพื่อให้ Debug ง่ายขึ้น

    [Header("UI Panels")]
    [SerializeField] private GameObject panelGradeQuestion;
    [SerializeField] private GameObject panelCongrats;
    [SerializeField] private GameObject panelTurnOver;

    [Header("Patient Phase Objects")]
    [SerializeField] private GameObject patientDummy;
    [SerializeField] private SpriteRenderer patientRenderer; // *เพิ่ม: เพื่อเปลี่ยนรูป
    [SerializeField] private Sprite lateralSprite;           // *เพิ่ม: รูปนอนตะแคง
    [SerializeField] private GameObject woundClickArea;      // *เพิ่ม: ตัว Collider ของแผล (ลูกของ Patient)
    
    [Header("Pillow Objects")]
    [SerializeField] private GameObject pillow;
    [SerializeField] private GameObject pillowDropZone;

    [Header("Timing")]
    [SerializeField] private float congratsAutoCloseSeconds = 1.0f;

    public bool IsUIBlockingInput { get; private set; }

    private void Awake()
    {
        SetAllOff();
        state = StageState.Idle;
    }

    private void Start()
    {
        // เริ่มเกม: ให้เห็นคนไข้ แต่ซ่อนแผลและหมอน
        if (patientDummy) patientDummy.SetActive(true);
        if (woundClickArea) woundClickArea.SetActive(false);
    }

    private void SetAllOff()
    {
        if (panelGradeQuestion) panelGradeQuestion.SetActive(false);
        if (panelCongrats) panelCongrats.SetActive(false);
        if (panelTurnOver) panelTurnOver.SetActive(false);

        // เราไม่ปิด patientDummy ตรงนี้ เพราะต้องให้เห็นตลอด มั้ยนะถ้าปิดเดี๊ยวแก้ให้
        if (pillow) pillow.SetActive(false);
        if (pillowDropZone) pillowDropZone.SetActive(false);
        if (woundClickArea) woundClickArea.SetActive(false);

        IsUIBlockingInput = false;
    }

    // ขั้นแรก : คลิกที่ตัวคนไข้เพื่อพลิกตัว 
    public void OnPatientClicked()
    {
        if (state != StageState.Idle) return; // กดได้แค่ตอนเริ่ม

        // เปลี่ยนรูปเป็นนอนตะแคง
        if (patientRenderer && lateralSprite) 
        {
            patientRenderer.sprite = lateralSprite;
        }

        // เปิดให้กดแผลได้ , interact ได้
        if (woundClickArea) woundClickArea.SetActive(true);

        state = StageState.PatientTurned;
        Debug.Log("Patient Turned: Ready to inspect wound.");
    }

    // ขั้นสอง : คลิกที่แผลเพื่อเริ่มตอบคำถาม 
    public void OnWoundClicked()
    {
        if (state != StageState.PatientTurned) return;

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

        // เปิดหมอนและจุดวาง
        if (pillow) 
        {
            pillow.SetActive(true);
            // สั่งปลดล็อคหมอน (ถ้า Script Draggable มีตัวแปร isLocked)
            var dragScript = pillow.GetComponent<Draggable2D>();
            if (dragScript) dragScript.isLocked = false;
        }
        if (pillowDropZone) pillowDropZone.SetActive(true);
    }

    // -ขั้นสอง : วางหมอนเสร็จ
    public void OnPillowPlacedCorrect()
    {
        if (state != StageState.PillowPlacement) return;

        state = StageState.TurnOverQuestion;
        IsUIBlockingInput = true;

        if (panelTurnOver) panelTurnOver.SetActive(true);
        Debug.Log("Pillow Placed Correctly -> Question 2");
    }

    public void OnTurnOverAnsweredCorrect()
    {
        if (state != StageState.TurnOverQuestion) return;

        if (panelTurnOver) panelTurnOver.SetActive(false);
        IsUIBlockingInput = false;

        state = StageState.Complete;
        Debug.Log("Immobility stage complete.");
    }
}