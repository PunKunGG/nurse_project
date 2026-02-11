using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))] // บังคับให้มี Collider
public class HazardObject : MonoBehaviour
{
    [Header("Settings")]
    public Sprite fixedSprite; // รูปตอนแก้ไขแล้ว (เช่น ไฟปิด)
    
    [Header("Visuals (Multiple)")]
    [Tooltip("Objects to show when hazard is NOT fixed")]
    public GameObject[] hazardVisuals; 
    [Tooltip("Objects to show when hazard IS fixed")]
    public GameObject[] fixedVisuals;
    [Tooltip("Part of the object to enable when hovered (Outline)")]
    public GameObject[] outlineVisuals;

    [Header("Feedback")]
    [Tooltip("Existing Object in scene to enable (Correct Mark)")]
    public GameObject correctFeedbackObject;

    private SpriteRenderer spriteRenderer;
    private bool isFixed = false;
    private bool isHovered = false;
    private EnvironmentStageManager manager;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        manager = FindFirstObjectByType<EnvironmentStageManager>();
        
        // บอก Manager ว่า "ฉันคือ 1 ปัญหาที่ต้องแก้"
        if(manager) manager.totalHazards++;
        
        // Ensure feedback object is hidden initially
        if (correctFeedbackObject) correctFeedbackObject.SetActive(false);

        UpdateVisuals();
    }

    void OnMouseEnter()
    {
        isHovered = true;
        UpdateVisuals();
    }

    void OnMouseExit()
    {
        isHovered = false;
        UpdateVisuals();
    }

    void OnMouseDown()
    {
        // ถ้าแก้ไปแล้ว หรือยังไม่ถึงช่วง Gameplay (Quiz ยังไม่จบ) ห้ามกด
        // (เช็คแบบง่ายคือดูว่า QuizPanel ปิดไปหรือยัง)
        if (isFixed) return;
        if (manager.quizPanel.activeSelf) return; // ถ้า Quiz ยังเปิดอยู่ ห้ามกดของ

        FixHazard();
    }

    void FixHazard()
    {
        isFixed = true;
        
        // เปลี่ยนรูป (Legacy support)
        if (fixedSprite && spriteRenderer != null) spriteRenderer.sprite = fixedSprite;
        
        // แจ้ง Manager
        if (manager) manager.OnHazardFixed();
        
        UpdateVisuals();
        ShowFeedback();
    }
    
    // ส่วนที่เพิ่มสำหรับ Debugger
    public void ForceFix()
    {
        FixHazard();
    }

    private void ShowFeedback()
    {
        if (correctFeedbackObject)
        {
            correctFeedbackObject.SetActive(true);
            StartCoroutine(AnimateFloatAndFade(correctFeedbackObject));
        }
    }

    private System.Collections.IEnumerator AnimateFloatAndFade(GameObject targetObj)
    {
        float duration = 1.0f;
        float time = 0;
        
        Vector3 startPos = targetObj.transform.position;
        Vector3 endPos = startPos + Vector3.up * 1.0f; // Float up by 1 unit

        SpriteRenderer sr = targetObj.GetComponent<SpriteRenderer>();
        Color startColor = sr ? sr.color : Color.white;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            
            // Move up
            targetObj.transform.position = Vector3.Lerp(startPos, endPos, t);
            
            // Fade out
            if (sr) sr.color = Color.Lerp(startColor, endColor, t);
            
            yield return null;
        }
        
        if (sr) sr.color = endColor;
        targetObj.SetActive(false); // Disable after animation
    }

    private void UpdateVisuals()
    {
        // Logic: Show outline ONLY if hovered AND not fixed AND user can interact (quiz panel closed)
        bool canInteract = !isFixed && (manager == null || !manager.quizPanel.activeSelf);
        bool showOutline = isHovered && canInteract;

        // Apply multiple visuals
        SetActiveAll(hazardVisuals, !isFixed);
        SetActiveAll(fixedVisuals, isFixed);
        SetActiveAll(outlineVisuals, showOutline);
    }

    private static void SetActiveAll(GameObject[] gos, bool active)
    {
        if (gos == null) return;
        for (int i = 0; i < gos.Length; i++)
        {
            if (gos[i] != null) gos[i].SetActive(active);
        }
    }
}