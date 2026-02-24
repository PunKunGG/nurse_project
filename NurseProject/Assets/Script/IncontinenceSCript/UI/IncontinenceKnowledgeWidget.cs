// IncontinenceKnowledgeWidget.cs — ปุ่มเปิดดูความรู้ประเภท Incontinence (KnowledgePopup style)
// ผู้เล่นกดเปิดอ่าน กดอีกทีปิด — แสดงข้อมูลทั้ง 7 ประเภท
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Text;

/// <summary>
/// Widget ปุ่มเปิดความรู้ — คล้าย KnowledgePopupUI ของ Instability
/// กดปุ่ม "📖 ความรู้" → แสดง Panel พร้อม fade+scale animation
/// ข้างในมี ScrollView แสดงประเภท Incontinence ทั้ง 7 ชนิด
/// </summary>
public class IncontinenceKnowledgeWidget : MonoBehaviour
{
    [Header("Toggle Button")]
    [SerializeField] private Button toggleButton;
    [SerializeField] private TextMeshProUGUI toggleButtonLabel;

    [Header("Popup Panel")]
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private RectTransform panelRect;

    [Header("Content")]
    [SerializeField] private TextMeshProUGUI contentText;

    [Header("Data")]
    [SerializeField] private IncontinenceKnowledgeData knowledgeData;

    [Header("Animation")]
    [SerializeField] private float animDuration = 0.25f;
    [SerializeField] private float startScale = 0.85f;

    [Header("Labels")]
    [SerializeField] private string closedLabel = "📖 ความรู้";
    [SerializeField] private string openLabel   = "✕ ปิด";

    private bool isOpen = false;
    private Coroutine animCoroutine;

    // ============================================================
    //  LIFECYCLE
    // ============================================================

    private void Awake()
    {
        if (toggleButton)
        {
            toggleButton.onClick.RemoveAllListeners();
            toggleButton.onClick.AddListener(Toggle);
        }

        // เริ่มต้นปิด
        HideInstant();
    }

    private void Start()
    {
        // สร้างเนื้อหาจาก ScriptableObject
        BuildContent();
    }

    // ============================================================
    //  PUBLIC API
    // ============================================================

    /// <summary>แสดง widget ทั้งหมด (ปุ่ม)</summary>
    public void Show()
    {
        gameObject.SetActive(true);
        HideInstant();
    }

    /// <summary>ซ่อน widget ทั้งหมด</summary>
    public void Hide()
    {
        StopAnim();
        HideInstant();
        gameObject.SetActive(false);
    }

    /// <summary>สลับเปิด/ปิด</summary>
    public void Toggle()
    {
        if (isOpen) ClosePopup();
        else        OpenPopup();
    }

    /// <summary>เปิด popup (fade + scale in)</summary>
    public void OpenPopup()
    {
        isOpen = true;
        if (toggleButtonLabel) toggleButtonLabel.text = openLabel;

        if (panelCanvasGroup)
        {
            panelCanvasGroup.gameObject.SetActive(true);
            StopAnim();
            animCoroutine = StartCoroutine(Animate(show: true));
        }
    }

    /// <summary>ปิด popup (fade + scale out)</summary>
    public void ClosePopup()
    {
        isOpen = false;
        if (toggleButtonLabel) toggleButtonLabel.text = closedLabel;

        if (panelCanvasGroup)
        {
            StopAnim();
            animCoroutine = StartCoroutine(Animate(show: false));
        }
    }

    // ============================================================
    //  CONTENT
    // ============================================================

    private void BuildContent()
    {
        if (knowledgeData == null || contentText == null) return;

        StringBuilder sb = new StringBuilder();
        foreach (var info in knowledgeData.types)
        {
            sb.AppendLine($"<b><color=#FFD700>■ {info.typeName}</color></b>");
            sb.AppendLine(info.description);
            sb.AppendLine();
        }

        contentText.text = sb.ToString().TrimEnd();
    }

    // ============================================================
    //  ANIMATION (fade + scale เหมือน KnowledgePopupUI)
    // ============================================================

    private void StopAnim()
    {
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }
    }

    private void HideInstant()
    {
        isOpen = false;
        if (toggleButtonLabel) toggleButtonLabel.text = closedLabel;

        if (panelCanvasGroup)
        {
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
        }
        if (panelRect)
            panelRect.localScale = Vector3.one * startScale;

        if (panelCanvasGroup)
            panelCanvasGroup.gameObject.SetActive(false);
    }

    private IEnumerator Animate(bool show)
    {
        if (panelCanvasGroup == null || panelRect == null)
        {
            if (!show && panelCanvasGroup) panelCanvasGroup.gameObject.SetActive(false);
            yield break;
        }

        float fromA = show ? 0f : 1f;
        float toA   = show ? 1f : 0f;
        Vector3 fromS = show ? Vector3.one * startScale : Vector3.one;
        Vector3 toS   = show ? Vector3.one : Vector3.one * startScale;

        panelCanvasGroup.alpha = fromA;
        panelRect.localScale = fromS;

        if (show)
        {
            panelCanvasGroup.interactable = true;
            panelCanvasGroup.blocksRaycasts = true;
        }

        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);

            panelCanvasGroup.alpha = Mathf.Lerp(fromA, toA, t);
            panelRect.localScale = Vector3.Lerp(fromS, toS, t);
            yield return null;
        }

        panelCanvasGroup.alpha = toA;
        panelRect.localScale = toS;

        if (!show)
        {
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
            panelCanvasGroup.gameObject.SetActive(false);
        }

        animCoroutine = null;
    }
}
