// PatientHistoryWidget.cs — ปุ่มเปิด/ปิดประวัติผู้ป่วย (Drawer Animation)
// แบบลิ้นชักเลื่อนจากซ้ายไปขวา กดเปิดอ่าน กดอีกทีปิด
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Widget ย่อ/ขยายประวัติผู้ป่วย — แบบ Drawer (ลิ้นชัก)
/// - Collapsed: ปุ่มเล็กมุมซ้ายจอ "📋 ประวัติผู้ป่วย"
/// - Expanded: Panel เลื่อนออกจากซ้ายไปขวา (slide animation)
/// </summary>
public class PatientHistoryWidget : MonoBehaviour
{
    [Header("Tab Button (ปุ่มเปิด/ปิด)")]
    [Tooltip("ปุ่มที่โผล่มุมจอตลอด — กดเพื่อ toggle panel")]
    [SerializeField] private Button toggleButton;
    [SerializeField] private TextMeshProUGUI toggleButtonLabel;

    [Header("Expanded Panel (Drawer)")]
    [Tooltip("Panel ที่มี ScrollView — จะถูก animate เลื่อนซ้าย-ขวา")]
    [SerializeField] private RectTransform expandedPanel;

    [Header("Content (ข้างใน Panel)")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI narrativeText;


    [Header("Animation Settings")]
    [Tooltip("ระยะเวลา animation (วินาที)")]
    [SerializeField] private float slideDuration = 0.35f;
    [Tooltip("Ease curve — ใช้ AnimationCurve เพื่อ smooth")]
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Labels")]
    [SerializeField] private string collapsedLabel = "📋 ประวัติผู้ป่วย";
    [SerializeField] private string expandedLabel  = "✕ ปิด";

    // Internal state
    private bool isExpanded = false;
    private bool isAnimating = false;
    private Coroutine slideCoroutine;

    /// <summary>ตำแหน่ง X เปิดเต็ม (ตำแหน่งปกติของ panel)</summary>
    private float openPosX;
    /// <summary>ตำแหน่ง X ซ่อน (เลื่อนออกไปซ้ายจนหมด)</summary>
    private float closedPosX;
    private bool initialized = false;

    // ============================================================
    //  LIFECYCLE
    // ============================================================

    private void Awake()
    {
        // ผูก toggle
        if (toggleButton)
        {
            toggleButton.onClick.RemoveAllListeners();
            toggleButton.onClick.AddListener(Toggle);
        }


        InitPositions();

        // เริ่มต้น collapsed (ไม่มี animation)
        SetCollapsedImmediate();
    }

    /// <summary>คำนวณตำแหน่ง open/closed จาก RectTransform</summary>
    private void InitPositions()
    {
        if (expandedPanel == null || initialized) return;
        initialized = true;

        // ตำแหน่งปกติ = ตำแหน่ง "เปิด"
        openPosX = expandedPanel.anchoredPosition.x;

        // ตำแหน่งซ่อน = เริ่มจากตำแหน่ง toggle button
        if (toggleButton != null)
        {
            RectTransform btnRect = toggleButton.GetComponent<RectTransform>();
            closedPosX = btnRect.anchoredPosition.x;
        }
        else
        {
            // fallback: เลื่อนไปซ้ายเท่ากับความกว้าง panel
            closedPosX = openPosX - expandedPanel.rect.width;
        }
    }

    // ============================================================
    //  PUBLIC API
    // ============================================================

    /// <summary>ตั้งค่าเนื้อหาประวัติผู้ป่วย</summary>
    public void SetHistory(string title, string narrative)
    {
        if (titleText)     titleText.text     = title;
        if (narrativeText) narrativeText.text = narrative;
    }

    /// <summary>แสดง widget (ปุ่ม tab) — เริ่มต้น collapsed</summary>
    public void Show()
    {
        gameObject.SetActive(true);
        InitPositions();
        SetCollapsedImmediate();
    }

    /// <summary>ซ่อน widget ทั้งหมด</summary>
    public void Hide()
    {
        StopSlide();
        gameObject.SetActive(false);
    }

    /// <summary>สลับเปิด/ปิด</summary>
    public void Toggle()
    {
        if (isAnimating) return; // กัน double-click ระหว่าง animation
        if (isExpanded) Collapse();
        else            Expand();
    }

    /// <summary>ขยาย panel (slide ซ้าย → ขวา)</summary>
    public void Expand()
    {
        if (expandedPanel == null) return;
        InitPositions();

        isExpanded = true;
        if (toggleButtonLabel) toggleButtonLabel.text = expandedLabel;
        expandedPanel.gameObject.SetActive(true);

        StartSlide(closedPosX, openPosX);
    }

    /// <summary>ย่อ panel (slide ขวา → ซ้าย)</summary>
    public void Collapse()
    {
        if (expandedPanel == null) return;
        InitPositions();

        isExpanded = false;
        if (toggleButtonLabel) toggleButtonLabel.text = collapsedLabel;

        StartSlide(openPosX, closedPosX, hideOnComplete: true);
    }

    // ============================================================
    //  ANIMATION (Coroutine)
    // ============================================================

    private void StartSlide(float fromX, float toX, bool hideOnComplete = false)
    {
        StopSlide();
        slideCoroutine = StartCoroutine(SlideRoutine(fromX, toX, hideOnComplete));
    }

    private void StopSlide()
    {
        if (slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
            slideCoroutine = null;
        }
        isAnimating = false;
    }

    private IEnumerator SlideRoutine(float fromX, float toX, bool hideOnComplete)
    {
        isAnimating = true;
        float elapsed = 0f;
        Vector2 pos = expandedPanel.anchoredPosition;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            float curveT = slideCurve.Evaluate(t);

            pos.x = Mathf.LerpUnclamped(fromX, toX, curveT);
            expandedPanel.anchoredPosition = pos;
            yield return null;
        }

        // Snap สุดท้าย
        pos.x = toX;
        expandedPanel.anchoredPosition = pos;

        if (hideOnComplete)
            expandedPanel.gameObject.SetActive(false);

        isAnimating = false;
        slideCoroutine = null;
    }

    // ============================================================
    //  HELPERS
    // ============================================================

    /// <summary>ตั้งตำแหน่ง collapsed ทันทีไม่มี animation</summary>
    private void SetCollapsedImmediate()
    {
        isExpanded = false;
        if (toggleButtonLabel) toggleButtonLabel.text = collapsedLabel;

        if (expandedPanel != null)
        {
            Vector2 pos = expandedPanel.anchoredPosition;
            pos.x = closedPosX;
            expandedPanel.anchoredPosition = pos;
            expandedPanel.gameObject.SetActive(false);
        }
    }
}
