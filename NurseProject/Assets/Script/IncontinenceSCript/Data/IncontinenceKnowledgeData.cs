// IncontinenceKnowledgeData.cs — ScriptableObject เก็บข้อมูลความรู้ประเภท Incontinence ทั้ง 7 ชนิด
using UnityEngine;
using System;

/// <summary>
/// เก็บคำอธิบายสั้น ๆ ของ Incontinence แต่ละประเภท
/// ใช้เป็น reference ให้ผู้เล่นอ่านก่อนตอบ
/// สร้างได้จาก: Assets → Create → Incontinence → Knowledge Data
/// </summary>
[CreateAssetMenu(fileName = "IncontinenceKnowledge", menuName = "Incontinence/Knowledge Data")]
public class IncontinenceKnowledgeData : ScriptableObject
{
    [Serializable]
    public class TypeInfo
    {
        public string typeName;         // ชื่อ เช่น "Stress UI"
        [TextArea(2, 8)]
        public string description;      // คำอธิบายลักษณะอาการ
    }

    [Header("ข้อมูลความรู้ 7 ประเภท (เรียงตาม enum IncontinenceType)")]
    [Tooltip("Stress, Urge, Overflow, Functional, Mixed(S+U), Mixed(U+F), Mixed(S+F)")]
    public TypeInfo[] types = new TypeInfo[7]
    {
        new TypeInfo { typeName = "Stress UI",                   description = "ปัสสาวะเล็ดเมื่อมีแรงดันในช่องท้องเพิ่ม เช่น ไอ จาม หัวเราะ ยกของหนัก" },
        new TypeInfo { typeName = "Urge UI",                     description = "ปวดปัสสาวะเฉียบพลันรุนแรง กลั้นไม่อยู่ มักเกี่ยวกับกล้ามเนื้อกระเพาะปัสสาวะบีบตัวมากเกิน" },
        new TypeInfo { typeName = "Overflow UI",                 description = "กระเพาะปัสสาวะเต็มแต่ถ่ายไม่หมด ปัสสาวะไหลซึมตลอด ท้องน้อยตึง" },
        new TypeInfo { typeName = "Functional UI",               description = "ระบบทางเดินปัสสาวะปกติ แต่มีข้อจำกัดทางร่างกายหรือจิตใจที่ทำให้ไปห้องน้ำไม่ทัน" },
        new TypeInfo { typeName = "Mixed (Stress + Urge)",       description = "มีอาการทั้ง Stress และ Urge ร่วมกัน — ปัสสาวะเล็ดเมื่อออกแรง และปวดปัสสาวะกะทันหัน" },
        new TypeInfo { typeName = "Mixed (Urge + Functional)",   description = "มีอาการทั้ง Urge และ Functional ร่วมกัน — ปวดปัสสาวะเฉียบพลัน ร่วมกับข้อจำกัดในการเคลื่อนไหว" },
        new TypeInfo { typeName = "Mixed (Stress + Functional)", description = "มีอาการทั้ง Stress และ Functional ร่วมกัน — ปัสสาวะเล็ดเมื่อออกแรง ร่วมกับข้อจำกัดทางร่างกาย" },
    };
}
