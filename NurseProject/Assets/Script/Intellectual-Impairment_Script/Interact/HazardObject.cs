using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))] // บังคับให้มี Collider
public class HazardObject : MonoBehaviour
{
    [Header("Settings")]
    public bool useFadeEffect = false; // Toggle to enable/disable fade out
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
    public string itemName;
    [TextArea(3, 10)]
    public string knowledgeMessage;

    private SpriteRenderer spriteRenderer;
    private bool isFixed = false;
    private bool isHovered = false;
    private IntellectualImpairmentStageManager manager;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        manager = FindFirstObjectByType<IntellectualImpairmentStageManager>();
        
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
        
        if (manager)
        {
            manager.OnHazardFixed();
            if (manager.knowledgePopup != null)
            {
                manager.knowledgePopup.Show(itemName, knowledgeMessage);
            }
        }
        
        if (useFadeEffect)
        {
            // Ensure Fixed Visuals (e.g. checkmark) are visible briefly so they can fade out too
            SetActiveAll(fixedVisuals, true);
            StartFadeOutSequence(); // New Fade Logic
        }
        else
        {
            UpdateVisuals(); // Instant update
        }
        
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
        
        // MODE 1: FADE OUT ENABLED (and IS fixed)
        // We DO NOT disable visuals here. We let the FadeOut coroutine handle it.
        if (useFadeEffect && isFixed)
        {
            // Do nothing, let coroutine running from FixHazard take care of it.
        }
        // MODE 2: NORMAL (Not fixed OR Fade disabled)
        else
        {
            if (!isFixed) 
            {
                SetActiveAll(hazardVisuals, true);
                SetActiveAll(fixedVisuals, false); // Normally hidden when not fixed
                SetActiveAll(outlineVisuals, showOutline);
            }
            else
            {
                // Fixed, and Instant Mode (or just fixed state update)
                SetActiveAll(hazardVisuals, false);
                SetActiveAll(fixedVisuals, true);
                SetActiveAll(outlineVisuals, showOutline);
            }
        }
    }
    
    private void StartFadeOutSequence()
    {
        // Fade out ALL groups
        FadeGroup(hazardVisuals);
        FadeGroup(fixedVisuals);
        FadeGroup(outlineVisuals);
    }

    private void FadeGroup(GameObject[] group)
    {
        if (group != null)
        {
            foreach (var obj in group)
            {
                if(obj != null) 
                {
                    // Ensure it's active so the coroutine runs (it halts on inactive objects)
                    obj.SetActive(true); 
                    StartCoroutine(FadeOutObject(obj));
                }
            }
        }
    }

    private System.Collections.IEnumerator FadeOutObject(GameObject obj)
    {
        float duration = 1.0f;
        float time = 0;
        
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        
        // Setup initial alpha
        Color startColor = sr ? sr.color : Color.white;
        float startAlpha = cg ? cg.alpha : (sr ? startColor.a : 1f);
        
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, t);

            if (sr) 
            {
                sr.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            }
            if (cg)
            {
                cg.alpha = newAlpha;
            }
            
            yield return null;
        }

        // Finalize
        if (sr) sr.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        if (cg) cg.alpha = 0f;
        
        obj.SetActive(false);
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