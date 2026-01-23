using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InanitionStageManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject instructionPanel; // แผ่นป้ายสอนเล่นตอนเริ่ม
    public UniversalResultUI resultUI;  // ตัว Result ครอบจักรวาลเจ้าเดิม
    public GameObject[] heartIcons;     // ลากรูปหัวใจ 3 ดวงมาใส่ในนี้ (Array)

    [Header("Game Settings")]
    public int maxLives = 3;
    
    // ตัวแปรภายใน
    private int currentLives;
    private int foundCount = 0;
    private int totalTargets = 0;
    private bool isGameActive = false;

    void Start()
    {
        // เริ่มมาเปิดหน้าสอนก่อน ปิดเกมไว้
        if (instructionPanel) instructionPanel.SetActive(true);
        if (resultUI) resultUI.gameObject.SetActive(false);

        currentLives = maxLives;
        UpdateHeartUI();
    }

    // ฟังก์ชันนี้ให้ปุ่ม "Next" ในหน้า Instruction กดเพื่อเริ่มเกม
    public void StartGameplay()
    {
        if (instructionPanel) instructionPanel.SetActive(false);
        isGameActive = true;
        
        // นับจำนวนของที่ต้องหาทั้งหมดในฉากอัตโนมัติ
        InanitionTarget[] targets = FindObjectsOfType<InanitionTarget>();
        totalTargets = targets.Length;
        Debug.Log($"Game Start! Total targets to find: {totalTargets}");
    }

    // --- Logic เมื่อเจอของถูก ---
    public void OnTargetFound()
    {
        if (!isGameActive) return;

        foundCount++;
        Debug.Log($"Found! {foundCount}/{totalTargets}");

        // เช็คชนะ
        if (foundCount >= totalTargets)
        {
            EndGame(true);
        }
    }

    // --- Logic เมื่อกดผิด (กดโดนพื้นหลัง) ---
    public void OnMissClick()
    {
        if (!isGameActive) return;

        currentLives--;
        Debug.Log("Wrong! Lives left: " + currentLives);
        
        UpdateHeartUI();

        // เช็คแพ้
        if (currentLives <= 0)
        {
            EndGame(false);
        }
    }

    private void UpdateHeartUI()
    {
        // วนลูปเปิด/ปิดหัวใจตามจำนวนชีวิตที่เหลือ
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (heartIcons[i])
            {
                heartIcons[i].SetActive(i < currentLives);
            }
        }
    }

    private void EndGame(bool isWin)
    {
        isGameActive = false;
        if (resultUI)
        {
            resultUI.ShowResult(isWin);
        }
    }
}