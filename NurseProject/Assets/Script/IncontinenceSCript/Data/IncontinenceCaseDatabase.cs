// IncontinenceCaseDatabase.cs — ScriptableObject เก็บรายการเคสทั้งหมด
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ฐานข้อมูลเคส Incontinence ทั้งหมด
/// สร้างได้จาก: Assets → Create → Incontinence → Case Database
/// </summary>
[CreateAssetMenu(fileName = "IncontinenceCaseDatabase", menuName = "Incontinence/Case Database")]
public class IncontinenceCaseDatabase : ScriptableObject
{
    [Header("รายการเคสทั้งหมด")]
    public List<IncontinenceCaseData> cases = new List<IncontinenceCaseData>();

    /// <summary>ดึงเคสตาม index</summary>
    public IncontinenceCaseData GetCase(int index)
    {
        if (cases == null || cases.Count == 0) return null;
        index = Mathf.Clamp(index, 0, cases.Count - 1);
        return cases[index];
    }

    /// <summary>สุ่มเคส</summary>
    public IncontinenceCaseData GetRandomCase()
    {
        if (cases == null || cases.Count == 0) return null;
        return cases[Random.Range(0, cases.Count)];
    }

    /// <summary>จำนวนเคสทั้งหมด</summary>
    public int Count => cases != null ? cases.Count : 0;
}
