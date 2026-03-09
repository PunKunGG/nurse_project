// IncontinenceCaseData.cs — ScriptableObject สำหรับเก็บข้อมูลเคสผู้ป่วย Incontinence แต่ละเคส
using UnityEngine;
using System.Collections.Generic;

// --- Enums ---

/// <summary>ประเภทภาวะกลั้นปัสสาวะไม่อยู่ (7 ตัวเลือก)</summary>
public enum IncontinenceType
{
    Stress,                 // Stress UI
    Urge,                   // Urge UI
    Overflow,               // Overflow UI
    Functional,             // Functional UI
    MixedStressUrge,        // Mixed (Stress + Urge)
    MixedUrgeFunctional,    // Mixed (Urge + Functional)
    MixedStressFunctional   // Mixed (Stress + Functional)
}

/// <summary>ตัวเลือกการพยาบาล (10 ข้อ)</summary>
public enum ManagementOption
{
    Kegel,          // Kegel exercise
    Knack,          // Knack technique
    BladderTrain,   // Bladder Training
    Prompted,       // Prompted voiding
    TimedVoiding,   // Timed Voiding
    CIC,            // Clean Intermittent Catheterization
    CoreGait,       // Core & Gait training
    BedMobility,    // Bed Mobility
    EnvMod,         // Environmental Modification
    MedReview       // Medication Review
}

/// <summary>
/// ScriptableObject เก็บข้อมูลเคสผู้ป่วย 1 เคส
/// สร้างได้จาก: Assets → Create → Incontinence → Case Data
/// </summary>
[CreateAssetMenu(fileName = "NewIncontinenceCase", menuName = "Incontinence/Case Data")]
public class IncontinenceCaseData : ScriptableObject
{
    [Header("ข้อมูลเคส")]
    public string caseId;           // รหัสเคส เช่น "CASE_01"
    public string title;            // ชื่อเคส เช่น "คุณยายสมศรี"

    [Header("ผู้ป่วย (Animated Prefab)")]
    public GameObject patientPrefab;   // Prefab ตัวผู้ป่วยที่ใช้แสดงในฉาก

    [TextArea(5, 15)]
    public string narrativeText;    // เนื้อหาเคส (อายุ/เพศ/อาการ/บริบท)

    [Header("เฉลย Stage 2: ประเภท Incontinence")]
    public IncontinenceType correctType;

    [Header("โจทย์ / คำสั่งสำหรับหน้า Type และ Management")]
    public string typeQuestion = "จากตัวเลือกต่อไปนี้ เลือกประเภทของ Urinary Incontinence";
    public string managementQuestion = "จากตัวเลือกต่อไปนี้ เลือกวิธีการพยาบาลและการฝึกที่เหมาะสมกับผู้ป่วย";

    [Header("เฉลย Stage 3: การพยาบาลที่ถูกต้อง (เลือกหลายข้อ)")]
    public List<ManagementOption> correctManagementOptions = new List<ManagementOption>();

    [Header("คำอธิบายเฉลย (Optional)")]
    [TextArea(2, 5)]
    public string rationaleType;        // เหตุผลว่าทำไมเป็นประเภทนี้
    [TextArea(2, 5)]
    public string rationaleManagement;  // เหตุผลของการพยาบาลที่เลือก

    [Header("คำใบ้ (Hint สำหรับหน้า Management)")]
    [TextArea(3, 10)]
    public string[] hintDialogue;       // ข้อความคำใบ้ที่จะโชว์ใน Visual Novel
}
