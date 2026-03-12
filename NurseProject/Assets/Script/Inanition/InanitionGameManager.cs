using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class InanitionGameManager : MonoBehaviour
{
    public enum GameState { Intro, Playing, Win, Lose }
    
    [Header("Game Settings")]
    public int maxHearts = 3;
    public int requiredCorrect = 5;
    
    [Header("UI References")]
    public HeartsUI heartsUI;
    public TextMeshProUGUI progressText; // Text: "Correct: 0/5"
    public UniversalResultUI resultUI;   // For Win/Lose messages
    public KnowledgePopupUI knowledgePopup;

    [Header("Visual Novel Intro")]
    [SerializeField] private VisualNovelIntro introNovel;
    [TextArea(3, 10)]
    [SerializeField] private string[] introDialogue = new string[] {
        "สวัสดีค่ะ นี่คือด่าน Inanition หรือภาวะขาดสารอาหาร สังเกตและค้นหาปัจจัยเสี่ยงในห้องนี้ที่อาจส่งผลกระทบต่อโภชนาการของผู้ป่วยให้ครบนะคะ!"
    };
    
    [Header("Scene References")]
    // Optional: Assign all hotspots here to easily reset them, 
    // or FindObjectsOfType if dynamic.
    public InanitionHotspot[] allHotspots;

    // State
    public GameState CurrentState { get; private set; }
    private int currentHearts;
    private int correctFoundCount;
    
    // Track found IDs to prevent duplicates (though Hotspot script also handles it)
    private HashSet<InanitionHotspot> foundHotspots = new HashSet<InanitionHotspot>();

    // Property for other scripts to check
    public bool IsPlaying => CurrentState == GameState.Playing;

    void Start()
    {
        // Auto-find hotspots if not assigned
        if (allHotspots == null || allHotspots.Length == 0)
        {
            allHotspots = FindObjectsOfType<InanitionHotspot>();
        }

        CurrentState = GameState.Intro;

        if (introNovel != null)
        {
            introNovel.OnIntroFinished.AddListener(OnIntroFinished);
            introNovel.StartIntro(introDialogue);
        }
        else
        {
            StartGame();
        }
    }

    private void OnIntroFinished()
    {
        if (introNovel != null)
            introNovel.OnIntroFinished.RemoveListener(OnIntroFinished);
            
        StartGame();
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        currentHearts = maxHearts;
        correctFoundCount = 0;
        foundHotspots.Clear();

        // Reset UI
        if (heartsUI) heartsUI.UpdateHearts(currentHearts);
        UpdateProgressUI();
        if (progressText) progressText.gameObject.SetActive(true);

        // Reset Hotspots
        foreach (var hotspot in allHotspots)
        {
            if (hotspot) hotspot.ResetHotspot();
        }
        
        Debug.Log("Inanition Game Started. Interaction Unlocked.");
    }

    public void OnHotspotClicked(InanitionHotspot hotspot)
    {
        if (CurrentState != GameState.Playing) return;
        if (foundHotspots.Contains(hotspot)) return;

        if (hotspot.isCorrect)
        {
            // --- CORRECT ---
            Debug.Log($"Found Correct Risk Factor: {hotspot.name}");
            
            // Mark visual
            hotspot.MarkAsFound();
            foundHotspots.Add(hotspot);

            if (knowledgePopup != null)
            {
                knowledgePopup.Show(hotspot.itemName, hotspot.knowledgeMessage);
            }
            
            // Score
            correctFoundCount++;
            UpdateProgressUI();

            // Check Win
            if (correctFoundCount >= requiredCorrect)
            {
                GameOver(true);
            }
        }
        else
        {
            // --- WRONG (Decoy) ---
            Debug.Log($"Wrong Click (Decoy): {hotspot.name}");
            
            // Deduct Heart
            currentHearts--;
            if (heartsUI) heartsUI.UpdateHearts(currentHearts);

            // Check Lose
            if (currentHearts <= 0)
            {
                GameOver(false);
            }
        }
    }

    private void UpdateProgressUI()
    {
        if (progressText)
        {
            progressText.text = $"ปัจจัยเสี่ยงที่พบ: {correctFoundCount}/{requiredCorrect}";
        }
    }

    private void GameOver(bool isWin)
    {
        CurrentState = isWin ? GameState.Win : GameState.Lose;
        
        if (resultUI)
        {
            if (isWin)
            {
                resultUI.ShowResult(
                    true,
                    "<color=green>ยอดเยี่ยม!</color>",
                    "ยินดีด้วย! คุณสามารถค้นพบปัจจัยเสี่ยงที่เกี่ยวข้องกับภาวะ Inanition ได้ครบถ้วน",
                    "ไปด่านต่อไป >>"
                );
            }
            else
            {
                resultUI.ShowResult(
                    false,
                    "<color=red>เสียใจด้วยน้าา</color>",
                    "ดูเหมือนปัจจัยเสี่ยงบางอย่างจะซ่อนอยู่ ผู้สูงอายุยังรอให้คุณช่วยเหลือ ลองเริ่มใหม่อีกครั้งนะคะ!",
                    "ลองอีกครั้ง"
                );
            }
        }
        else
        {
            Debug.LogWarning("UniversalResultUI not assigned!");
        }

        // ซ่อน Progress Text เพื่อความสวยงาม
        if(progressText) progressText.gameObject.SetActive(false);
    }
}
