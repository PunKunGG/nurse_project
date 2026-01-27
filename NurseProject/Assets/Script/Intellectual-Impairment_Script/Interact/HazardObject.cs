using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))] // บังคับให้มี Collider
public class HazardObject : MonoBehaviour
{
    [Header("Settings")]
    public Sprite fixedSprite; // รูปตอนแก้ไขแล้ว (เช่น ไฟปิด)
    
    private SpriteRenderer spriteRenderer;
    private bool isFixed = false;
    private EnvironmentStageManager manager;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        manager = FindFirstObjectByType<EnvironmentStageManager>();
        
        // บอก Manager ว่า "ฉันคือ 1 ปัญหาที่ต้องแก้"
        if(manager) manager.totalHazards++;
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
        
        // เปลี่ยนรูป
        if (fixedSprite) spriteRenderer.sprite = fixedSprite;
        
        // แจ้ง Manager
        if (manager) manager.OnHazardFixed();
        
    }
    // ส่วนที่เพิ่มสำหรับ Debugger
    public void ForceFix()
    {
        FixHazard();
    }
}